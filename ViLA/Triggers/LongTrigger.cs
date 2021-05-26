using System.Collections.Generic;
using Configuration;

namespace ViLA.Triggers
{
    public class LongTrigger : TriggerBase<long>
    {
        public LongTrigger(string id, long value, Comparator comparator) : base(id, value, comparator)
        {
        }

        public override bool ShouldTrigger(long value)
        {
            return Comparator switch
            {
                Comparator.GreaterThan => value > Value,
                Comparator.LessThan => value < Value,
                Comparator.EqualTo => value == Value,
                Comparator.NotEqualTo => value != Value,
                Comparator.GreaterThanOrEqualTo => value >= Value,
                Comparator.LessThanOrEqualTo => value <= Value,
                _ => false,
            };
        }

        protected override HashSet<Comparator> AllowedComparators { get; } = new()
        {
            Comparator.EqualTo, Comparator.NotEqualTo, Comparator.GreaterThan, Comparator.LessThan,
            Comparator.GreaterThanOrEqualTo, Comparator.LessThanOrEqualTo,
        };
    }
}