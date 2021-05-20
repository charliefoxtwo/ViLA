// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

using ViLA.Triggers;

namespace ViLA.Configuration
{
    public class LedAction
    {
        public string Color { get; set; }
        public Trigger Trigger { get; set; }
        public Target Target { get; set; }
    }
}