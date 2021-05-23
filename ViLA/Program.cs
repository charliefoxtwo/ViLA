using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Virpil.Communicator;

namespace ViLA
{
    public class Program
    {
        private static ILogger<Program> _log = null!;

        public static async Task Main(string[] args)
        {

            var cfg = Config.GetAllConfiguration();

            if (cfg is null)
            {
                var lf = LoggerFactory.Create(c => c.SetMinimumLevel(LogLevel.Information).AddConsole());

                lf.CreateLogger<Program>().LogCritical("Config files are empty or error loading config files, exiting...");
                return;
            }

            var loggerFactory = LoggerFactory.Create(c => c.SetMinimumLevel(cfg.LogLevel ?? LogLevel.Information).AddConsole());
            _log = loggerFactory.CreateLogger<Program>();

            await CheckProgramVersion(cfg.CheckPrerelease);

            var plugins = new List<PluginBase.PluginBase>();

            await foreach (var plugin in LoadPlugins(cfg.DisabledPlugins, cfg.CheckUpdates, cfg.CheckPrerelease))
            {
                plugins.Add(plugin);
            }

            var devices = DeviceCommunicator.AllConnectedVirpilDevices(loggerFactory).ToList();

            _log.LogInformation("Detected {DeviceNumber} devices", devices.Count);
            foreach (var device in devices)
            {
                _log.LogInformation("Detected device with PID {Device:x4}", device.PID);

                try
                {
                    device.SendCommand(BoardType.Default, 1, LedPower.Zero, LedPower.Zero, LedPower.Zero);
                    _log.LogDebug("Leds reset for {Device:x4}", device.PID);
                }
                catch(Exception)
                {
                    _log.LogError("Exception when resetting leds - try restarting your application?");
                }
            }

            var r = new Runner(devices, cfg.Devices ?? new Dictionary<string, Device>(), plugins, loggerFactory.CreateLogger<Runner>());

            await r.Start(loggerFactory);

            await Task.Delay(-1, new CancellationToken());
        }

        private static async IAsyncEnumerable<PluginBase.PluginBase> LoadPlugins(IReadOnlySet<string> disabledPlugins, bool checkUpdates = true, bool checkPrerelease = false)
        {
            if (!Directory.Exists("./Plugins")) Directory.CreateDirectory("./Plugins");
            var manifests = Directory.EnumerateFiles("./Plugins", "manifest.json", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                var manifestJson = await File.ReadAllTextAsync(manifest);

                var pluginManifest = JsonConvert.DeserializeObject<PluginManifest>(manifestJson);

                if (pluginManifest is null)
                {
                    _log.LogWarning("Error loading manifest {ManifestFile}. Skipping...", manifest);
                    continue;
                }

                if (disabledPlugins.Contains(pluginManifest.Entrypoint)) continue;

                if (checkUpdates)
                {
                    await CheckPluginVersion(pluginManifest, checkPrerelease);
                }

                var dllPath = Path.Combine(Directory.GetParent(manifest)!.FullName, pluginManifest.Entrypoint);
                var loader = PluginLoader.CreateFromAssemblyFile(
                    dllPath,
                    sharedTypes: new[] { typeof(PluginBase.PluginBase), typeof(ILogger), typeof(ILoggerFactory) },
                    isUnloadable: false);

                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(PluginBase.PluginBase).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    if (Activator.CreateInstance(pluginType) is not PluginBase.PluginBase plugin)
                    {
                        _log.LogError("{ClassName} does not contain a class inheriting {BaseClassName}. Skipping...", pluginType.Name, nameof(PluginBase.PluginBase));
                        yield break;
                    }

                    _log.LogInformation("Loaded plugin {ClassName}", pluginType.Name);

                    yield return plugin;
                }
            }
        }

        private static async Task CheckProgramVersion(bool checkPrerelease)
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            const string githubUrl = "https://api.github.com/repos/charliefoxtwo/ViLA/releases";
            await CheckVersionAgainstGithub("ViLA", currentVersion, githubUrl, checkPrerelease);
        }

        private static async Task CheckPluginVersion(PluginManifest pluginManifest, bool checkPrerelease)
        {
            if (pluginManifest.Releases is null || pluginManifest.Version is null) return;

            if (Version.TryParse(pluginManifest.Version, out var currentVersion))
            {
                await CheckVersionAgainstGithub(pluginManifest.Entrypoint, currentVersion, pluginManifest.Releases,
                    checkPrerelease);
            }
            else
            {
                _log.LogWarning("Unable to parse version for {Plugin}, skipping update check...", pluginManifest.Entrypoint);
            }
        }

        private static async Task CheckVersionAgainstGithub(string name, Version currentVersion, string githubUrl, bool checkPrerelease)
        {
            var client = new RestClient();
            client.UseNewtonsoftJson();
            var request = new RestRequest(githubUrl);
            List<GithubReleaseResponse> response;
            try
            {
                response = await client.GetAsync<List<GithubReleaseResponse>>(request);
            }
            catch (JsonSerializationException ex)
            {
                // probably a github rate limit. we'll just skip for now
                return;
            }

            var latestRelevantRelease = response.FirstOrDefault(r => !r.Draft && (checkPrerelease || !r.Prerelease) &&
                                                                     (Version.TryParse(r.Name, out _) || Version.TryParse(r.TagName, out _)));
            if (latestRelevantRelease is not null)
            {
                // we need a version to work with, otherwise we can't really know if it's newer
                if (Version.TryParse(latestRelevantRelease.TagName, out var latestVersion) || Version.TryParse(latestRelevantRelease.Name, out latestVersion))
                {
                    if (latestVersion.CompareTo(currentVersion) > 0)
                    {
                        _log.LogWarning(
                            "A newer version of {Plugin} is available! Consider downloading it here {PluginReleaseUrl}",
                            name, latestRelevantRelease.HtmlUrl);
                    }
                    else
                    {
                        _log.LogDebug("{Plugin} v{Version} is up to date", name, latestVersion);
                    }
                }
            }
        }
    }
}