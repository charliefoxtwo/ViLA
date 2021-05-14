using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Virpil.Communicator;

namespace ViLA
{
    public class Runner
    {
        private readonly Dictionary<ushort, DeviceCommunicator> _devices;
        private readonly List<PluginBase.PluginBase> _plugins;
        private readonly Dictionary<string, List<DeviceAction>> _actions = new();
        private readonly ILogger<Runner> _log;

        public Runner(IEnumerable<DeviceCommunicator> devices, Dictionary<string, List<Action>> deviceActions, List<PluginBase.PluginBase> plugins, ILogger<Runner> log)
        {
            _log = log;
            _plugins = plugins;
            _devices = devices.ToDictionary(d => d.PID, d => d);

            foreach (var (device, actions) in deviceActions)
            {
                var deviceShort = ushort.Parse(device, NumberStyles.HexNumber);
                foreach (var action in actions)
                {
                    if (!_actions.ContainsKey(action.Trigger.Id))
                    {
                        _actions[action.Trigger.Id] = new List<DeviceAction>();
                    }
                    _actions[action.Trigger.Id].Add(new DeviceAction(action, deviceShort));
                }
            }
        }

        public async Task Start(ILoggerFactory loggerFactory)
        {
            _log.LogInformation("Starting plugins...");

            foreach (var plugin in _plugins)
            {
                plugin.SendData += Action;
                plugin.LoggerFactory = loggerFactory;

                _log.LogDebug("Starting plugin {Plugin}", plugin.GetType().Name);

                var result = await plugin.Start();

                if (result)
                {
                    _log.LogDebug("Started successfully");
                }
                else
                {
                    _log.LogError("Error encountered during start up. Skipping...");
                }
            }

            _log.LogInformation("Plugins started");
        }

        private void Action(string id, int value)
        {
            if (!_actions.TryGetValue(id, out var actions)) return; // nothing for this bios code, then leave

            _log.LogDebug("got data for {Id}", id);
            foreach (var action in actions.Where(action => action.Action.Trigger.ShouldTrigger(value)))
            {
                if (!_devices.TryGetValue(action.Device, out var device)) continue;

                _log.LogDebug("got relevant data for {Id}, sending...", id);
                var (red, green, blue) = action.Action.Color.ToLedPowers();
                device.SendCommand(action.Action.Target.BoardType, action.Action.Target.LedNumber, red, green, blue);
            }
        }
    }
}