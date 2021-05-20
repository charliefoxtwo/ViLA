// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

namespace Configuration
{
    public class LedAction
    {
        public string Color { get; set; }
        public Trigger Trigger { get; set; }
        public Target Target { get; set; }
    }
}