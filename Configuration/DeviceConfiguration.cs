// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Virpil.Communicator;

namespace Configuration;

public class DeviceConfiguration
{
    public BaseTrigger? Trigger { get; set; }
    public Dictionary<string, Device>? Devices { get; set; }

    public static IEnumerable<DeviceConfiguration> GetDeviceConfigurations(ILogger<DeviceConfiguration> log)
    {
        foreach (var filePath in Directory.EnumerateFiles("Configuration", "*.json", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(filePath);
            if (fileName == "ViLA.json") continue;

            DeviceConfiguration? data;
            try
            {
                data = JsonConvert.DeserializeObject<DeviceConfiguration>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                log.LogWarning("Error while parsing config file [{File}], skipping...", fileName);
                log.LogDebug("Error: {Error}", ex);
                continue;
            }

            if (data is not null) yield return data;
        }
    }
}

public class Device : Dictionary<BoardType, BoardActions>
{
}

public class BoardActions : Dictionary<int, List<LedAction>>
{
}
