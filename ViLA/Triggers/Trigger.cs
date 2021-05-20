// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618
using System.Diagnostics.CodeAnalysis;

namespace ViLA.Triggers
{
    public class Trigger : TriggerBase
    {
        public object? Value { get; set; }
        public Comparator Comparator { get; set; }

        public Trigger() : base("") { }

        public bool TryGetLongTrigger([MaybeNullWhen(false)] out LongTrigger trigger)
        {
            trigger = null;
            if (Value is not long longValue) return false;

            trigger = new LongTrigger(Id, longValue, Comparator);
            return true;
        }

        public bool TryGetStringTrigger([MaybeNullWhen(false)] out StringTrigger trigger)
        {
            trigger = null;
            if (Value is not string stringValue) return false;

            trigger = new StringTrigger(Id, stringValue, Comparator);
            return true;
        }

        public bool TryGetDoubleTrigger([MaybeNullWhen(false)] out DoubleTrigger trigger)
        {
            trigger = null;
            if (Value is not double doubleValue) return false;

            trigger = new DoubleTrigger(Id, doubleValue, Comparator);
            return true;
        }

        public bool TryGetBoolTrigger([MaybeNullWhen(false)] out BoolTrigger trigger)
        {
            trigger = null;
            if (Value is not bool boolValue) return false;

            trigger = new BoolTrigger(Id, boolValue, Comparator);
            return true;
        }

        public bool TryGetBasicTrigger([MaybeNullWhen(false)] out BasicTrigger trigger)
        {
            trigger = null;
            if (Value is not null) return false;

            trigger = new BasicTrigger(Id);
            return true;
        }
    }
}