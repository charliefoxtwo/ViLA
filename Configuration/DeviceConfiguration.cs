// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Newtonsoft.Json;
using Virpil.Communicator;

namespace Configuration;

public class DeviceConfiguration
{
    public BaseTrigger? Trigger { get; set; }
    public Dictionary<string, Device>? Devices { get; set; }

    public static Dictionary<string, DeviceConfiguration?> GetDeviceConfigurations()
    {
        return Directory.EnumerateFiles("Configuration", "*.json", SearchOption.AllDirectories)
            .Select(f => new KeyValuePair<string, DeviceConfiguration?>(f.Split('/').Last(), JsonConvert.DeserializeObject<DeviceConfiguration>(File.ReadAllText(f))))
            .Where(kvp => kvp.Key != "ViLA.json")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}

public class Device : Dictionary<BoardType, BoardActions>
{
}

public class BoardActions : Dictionary<int, List<LedAction>>
{
}
