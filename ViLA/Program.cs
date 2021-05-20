using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ViLA.Configuration;
using ViLA.Triggers;
using Virpil.Communicator;

namespace ViLA
{
    public class Program
    {
        private static ILogger<Program> _log = null!;

        public static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(c => c.SetMinimumLevel(LogLevel.Information).AddConsole());
            _log = loggerFactory.CreateLogger<Program>();

            var json = await File.ReadAllTextAsync("Configuration/config.json");
            var cfg = JsonConvert.DeserializeObject<Config>(json);

            if (cfg is null)
            {
                _log.LogCritical("Config file is empty or error loading config file, exiting...");
                return;
            }

            const bool pluginsEnabled = true;

            var plugins = new List<PluginBase.PluginBase>();
            if (pluginsEnabled)
            {
                await foreach (var plugin in LoadPlugins())
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

        private static async IAsyncEnumerable<PluginBase.PluginBase> LoadPlugins()
        {
            if (!Directory.Exists("./Plugins")) Directory.CreateDirectory("./Plugins");
            var manifests = Directory.EnumerateFiles("./Plugins", "manifest.txt", SearchOption.AllDirectories);

            foreach (var manifest in manifests)
            {
                var assemblyName = await File.ReadAllTextAsync(manifest);
                var dllPath = Path.Combine(Directory.GetParent(manifest)!.FullName, assemblyName);
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
    }

    public class DeviceAction<T> where T : TriggerBase
    {
        public ushort Device { get; }
        public T Trigger { get; }
        public string Color { get; }
        public Target Target { get; }

        public DeviceAction(string color, T trigger, Target target, ushort device)
        {
            Color = color;
            Trigger = trigger;
            Target = target;
            Device = device;
        }
    }
}