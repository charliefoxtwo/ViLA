﻿// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Virpil.Communicator;

namespace Configuration
{
    public class Config
    {
        public Dictionary<string, Device> Devices { get; set; }

        /// <summary>
        /// Appends the devices of the provided config to this config.
        /// </summary>
        /// <param name="otherConfig"></param>
        public Config Append(Config otherConfig)
        {
            foreach (var (deviceId, device) in otherConfig.Devices)
            {
                if (Devices.TryGetValue(deviceId, out var thisDevice))
                {
                    foreach (var (boardType, boardActions) in device)
                    {
                        if (Devices[deviceId].TryGetValue(boardType, out var thisBoardActions))
                        {
                            foreach (var (ledNumber, ledActions) in boardActions)
                            {
                                if (Devices[deviceId][boardType].TryGetValue(ledNumber, out var thisLedActions))
                                {
                                    thisLedActions.AddRange(ledActions);
                                }
                                else
                                {
                                    thisLedActions = ledActions;
                                }

                                Devices[deviceId][boardType][ledNumber] = thisLedActions;
                            }
                        }
                        else
                        {
                            thisBoardActions = boardActions;
                        }

                        Devices[deviceId][boardType] = thisBoardActions;
                    }
                }
                else
                {
                    thisDevice = device;
                }

                Devices[deviceId] = thisDevice;
            }

            return this;
        }

        public static Config? GetAllConfiguration()
        {
            return Directory.EnumerateFiles("Configuration", "*.json").AsParallel()
                .Select(f => JsonConvert.DeserializeObject<Config>(File.ReadAllText(f)))
                .Where(c => c != null)
                .Aggregate((s, t) => s!.Append(t!));
        }
    }

    public class Device : Dictionary<BoardType, BoardActions>
    {
    }

    public class BoardActions : Dictionary<int, List<LedAction>>
    {
    }
}