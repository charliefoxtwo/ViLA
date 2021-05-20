using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ViLA.Configuration;
using ViLA.Triggers;
using Virpil.Communicator;

namespace ViLA
{
    public class Runner
    {
        private readonly Dictionary<ushort, DeviceCommunicator> _devices;
        private readonly List<PluginBase.PluginBase> _plugins;

        // id => action
        private readonly Dictionary<string, List<DeviceAction<TriggerBase<long>>>> _longActions = new();
        private readonly Dictionary<string, List<DeviceAction<TriggerBase<string>>>> _stringActions = new();
        private readonly Dictionary<string, List<DeviceAction<TriggerBase<double>>>> _doubleActions = new();
        private readonly Dictionary<string, List<DeviceAction<TriggerBase<bool>>>> _boolActions = new();
        private readonly Dictionary<string, List<DeviceAction<TriggerBase>>> _basicActions = new();

        private readonly ILogger<Runner> _log;

        public Runner(IEnumerable<DeviceCommunicator> devices, Dictionary<string, List<LedAction>> deviceActions, List<PluginBase.PluginBase> plugins, ILogger<Runner> log)
        {
            _log = log;
            _plugins = plugins;
            _devices = devices.ToDictionary(d => d.PID, d => d);

            foreach (var (device, actions) in deviceActions)
            {
                var deviceShort = ushort.Parse(device, NumberStyles.HexNumber);
                foreach (var action in actions)
                {
                    try
                    {
                        if (action.Trigger.TryGetLongTrigger(out var longTrigger))
                        {
                            SetUpTrigger(_longActions, longTrigger, action, deviceShort);
                        }
                        else if (action.Trigger.TryGetStringTrigger(out var stringTrigger))
                        {
                            SetUpTrigger(_stringActions, stringTrigger, action, deviceShort);
                        }
                        else if (action.Trigger.TryGetDoubleTrigger(out var doubleTrigger))
                        {
                            SetUpTrigger(_doubleActions, doubleTrigger, action, deviceShort);
                        }
                        else if (action.Trigger.TryGetBoolTrigger(out var boolTrigger))
                        {
                            SetUpTrigger(_boolActions, boolTrigger, action, deviceShort);
                        }
                        else if (action.Trigger.TryGetBasicTrigger(out var basicTrigger))
                        {
                            SetUpTrigger(_basicActions, basicTrigger, action, deviceShort);
                        }
                        else
                        {
                            log.LogError("Skipping action for {Id} because no supported trigger configuration was found",
                                action.Trigger.Id);
                        }
                    }
                    catch (ArgumentException)
                    {
                        _log.LogError("Unsupported comparator passed for {Id}. Skipping...", action.Trigger.Id);
                    }
                }
            }
        }

        private static void SetUpTrigger<T>(IDictionary<string, List<DeviceAction<T>>> dict, T trigger,
            LedAction ledAction, ushort deviceShort) where T : TriggerBase
        {
            if (!dict.ContainsKey(ledAction.Trigger.Id))
            {
                dict[ledAction.Trigger.Id] = new List<DeviceAction<T>>();
            }

            dict[ledAction.Trigger.Id].Add(new DeviceAction<T>(ledAction.Color, trigger, ledAction.Target, deviceShort));
        }

        public async Task Start(ILoggerFactory loggerFactory)
        {
            _log.LogInformation("Starting plugins...");

            foreach (var plugin in _plugins)
            {
                plugin.SendData += LongAction;
                plugin.SendString += StringAction;
                plugin.SendFloat += DoubleAction;
                plugin.SendBool += BoolAction;
                plugin.SendTrigger += TriggerAction;
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

        private void LongAction(string id, int value)
        {
            TriggerActionForValue(_longActions, id, value);
        }

        private void StringAction(string id, string value)
        {
            TriggerActionForValue(_stringActions, id, value);
        }

        private void DoubleAction(string id, float value)
        {
            TriggerActionForValue(_doubleActions, id, value);
        }

        private void BoolAction(string id, bool value)
        {
            TriggerActionForValue(_boolActions, id, value);
        }

        private void TriggerActionForValue<T>(IReadOnlyDictionary<string, List<DeviceAction<TriggerBase<T>>>> typedActions, string id, T value)
        {
            if (!typedActions.TryGetValue(id, out var actions)) return; // nothing for this bios code, then leave

            _log.LogTrace("got {Type} data {Data} for {Id}", nameof(T), value, id);
            foreach (var action in actions.Where(action => action.Trigger.ShouldTrigger(value)))
            {
                if (!_devices.TryGetValue(action.Device, out var device)) continue;

                _log.LogDebug("Triggering {Id}", id);
                var (red, green, blue) = action.Color.ToLedPowers();
                device.SendCommand(action.Target.BoardType, action.Target.LedNumber, red, green, blue);
            }
        }

        private void TriggerAction(string id)
        {
            if (!_basicActions.TryGetValue(id, out var actions)) return; // nothing for this bios code, then leave

            _log.LogTrace("got trigger for {Id}", id);
            foreach (var action in actions)
            {
                if (!_devices.TryGetValue(action.Device, out var device)) continue;

                _log.LogDebug("Triggering {Id}", id);
                var (red, green, blue) = action.Color.ToLedPowers();
                device.SendCommand(action.Target.BoardType, action.Target.LedNumber, red, green, blue);
            }
        }
    }
}