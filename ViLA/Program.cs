using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            const bool pluginsEnabled = true;

            var plugins = new List<PluginBase.PluginBase>();
            if (pluginsEnabled)
            {
                await foreach (var plugin in LoadPlugins(cfg.CheckUpdates, cfg.CheckPrerelease))
                {
                    plugins.Add(plugin);
                }
            }

            var devices = DeviceCommunicator.AllConnectedVirpilDevices(loggerFactory).ToList();

            foreach (var device in devices)
            {
                _log.LogInformation("Detected device with PID {Device:x4}", device.PID);
            }

            var r = new Runner(devices, cfg.Devices, plugins, loggerFactory.CreateLogger<Runner>());

            await r.Start(loggerFactory);

            await Task.Delay(-1);
        }

        private static async IAsyncEnumerable<PluginBase.PluginBase> LoadPlugins(bool checkUpdates = true, bool checkPrerelease = false)
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

        private static async Task CheckPluginVersion(PluginManifest pluginManifest, bool checkPrerelease)
        {
            if (pluginManifest.Releases is null || pluginManifest.Version is null) return;

            if (Version.TryParse(pluginManifest.Version, out var currentVersion))
            {
                var client = new RestClient();
                client.UseNewtonsoftJson();
                var request = new RestRequest(pluginManifest.Releases);
                var response = await client.GetAsync<List<GithubReleaseResponse>>(request);

                var latestRelevantRelease = response.FirstOrDefault(r => !r.Draft && (checkPrerelease || !r.Prerelease));
                if (latestRelevantRelease is not null)
                {
                    // we need a version to work with, otherwise we can't really know if it's newer
                    if (Version.TryParse(latestRelevantRelease.TagName, out var latestVersion) || Version.TryParse(latestRelevantRelease.Name, out latestVersion))
                    {
                        if (latestVersion.CompareTo(currentVersion) > 0)
                        {
                            _log.LogWarning(
                                "A newer version of {Plugin} is available! Consider downloading it here {PluginReleaseUrl}",
                                pluginManifest.Entrypoint, latestRelevantRelease.HtmlUrl);
                        }
                        else
                        {
                            _log.LogDebug("{Plugin} v{Version} is up to date", pluginManifest.Entrypoint, latestVersion);
                        }
                    }
                }
            }
            else
            {
                _log.LogWarning("Unable to parse version for {Plugin}, skipping update check...", pluginManifest.Entrypoint);
            }
        }
    }
}