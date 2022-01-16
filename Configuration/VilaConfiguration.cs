// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Configuration;

public class VilaConfiguration
{
    public LogLevel? LogLevel { get; set; }
    public bool CheckUpdates { get; set; } = true;
    public bool CheckPrerelease { get; set; }
    public HashSet<string> DisabledPlugins { get; set; } = new();


    /// <summary>
    /// Appends the devices of the provided config to this config.
    /// </summary>
    /// <param name="otherVilaConfiguration"></param>
    public VilaConfiguration Append(VilaConfiguration otherVilaConfiguration)
    {
        if (otherVilaConfiguration.LogLevel is not null)
        {
            LogLevel = (LogLevel) Math.Min((int) (LogLevel ?? Microsoft.Extensions.Logging.LogLevel.None),
                (int) otherVilaConfiguration.LogLevel);
        }

        if (otherVilaConfiguration.CheckPrerelease) CheckPrerelease = otherVilaConfiguration.CheckPrerelease;
        if (otherVilaConfiguration.CheckUpdates) CheckUpdates = otherVilaConfiguration.CheckUpdates;
        DisabledPlugins.UnionWith(otherVilaConfiguration.DisabledPlugins);

        return this;
    }

    public static VilaConfiguration? GetVilaConfiguration()
    {
        return Directory.EnumerateFiles("Configuration", "ViLA.json", SearchOption.AllDirectories).AsParallel()
            .Select(f => JsonConvert.DeserializeObject<VilaConfiguration>(File.ReadAllText(f)))
            .Where(c => c != null)
            .Aggregate((s, t) => s!.Append(t!));
    }
}
