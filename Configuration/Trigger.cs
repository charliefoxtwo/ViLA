// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618

namespace Configuration
{
    public class Trigger : TriggerBase
    {
        public object? Value { get; set; }
        public Comparator Comparator { get; set; }

        public Trigger() : base("") { }
    }
}