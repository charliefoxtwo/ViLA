// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using System.Collections.Generic;
using ViLA.Configuration;

namespace ViLA
{
    public class Config
    {
        public Dictionary<string, List<LedAction>> Devices { get; set; }
    }
}