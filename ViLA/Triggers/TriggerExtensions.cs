using System.Diagnostics.CodeAnalysis;
using Configuration;

namespace ViLA.Triggers
{
    public static class TriggerExtensions
    {
        public static bool TryGetLongTrigger(this Trigger from, [MaybeNullWhen(false)] out LongTrigger trigger)
        {
            trigger = null;
            if (from.Value is not long longValue) return false;

            trigger = new LongTrigger(from.Id, longValue, from.Comparator);
            return true;
        }

        public static bool TryGetStringTrigger(this Trigger from, [MaybeNullWhen(false)] out StringTrigger trigger)
        {
            trigger = null;
            if (from.Value is not string stringValue) return false;

            trigger = new StringTrigger(from.Id, stringValue, from.Comparator);
            return true;
        }

        public static bool TryGetDoubleTrigger(this Trigger from, [MaybeNullWhen(false)] out DoubleTrigger trigger)
        {
            trigger = null;
            if (from.Value is not double doubleValue) return false;

            trigger = new DoubleTrigger(from.Id, doubleValue, from.Comparator);
            return true;
        }

        public static bool TryGetBoolTrigger(this Trigger from, [MaybeNullWhen(false)] out BoolTrigger trigger)
        {
            trigger = null;
            if (from.Value is not bool boolValue) return false;

            trigger = new BoolTrigger(from.Id, boolValue, from.Comparator);
            return true;
        }

        public static bool TryGetBasicTrigger(this Trigger from, [MaybeNullWhen(false)] out BasicTrigger trigger)
        {
            trigger = null;
            if (from.Value is not null) return false;

            trigger = new BasicTrigger(from.Id);
            return true;
        }
    }
}