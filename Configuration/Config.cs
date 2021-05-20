// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Configuration
{
    public class Config
    {
        public Dictionary<string, List<LedAction>> Devices { get; set; }

        /// <summary>
        /// Appends the devices of the provided config to this config.
        /// </summary>
        /// <param name="otherConfig"></param>
        public Config Append(Config otherConfig)
        {
            foreach (var (device, actions) in otherConfig.Devices)
            {
                if (Devices.TryGetValue(device, out var deviceActions))
                {
                    deviceActions.AddRange(actions);
                }
                else
                {
                    deviceActions = new List<LedAction>(actions);
                }

                Devices[device] = deviceActions;
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
}