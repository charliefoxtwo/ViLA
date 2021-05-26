using System.Collections.Generic;
using Configuration;

namespace ViLA.Triggers
{
    public class BoolTrigger : TriggerBase<bool>
    {
        public BoolTrigger(string id, bool value, Comparator comparator) : base(id, value, comparator)
        {
        }

        public override bool ShouldTrigger(bool value)
        {
            return Comparator switch
            {
                Comparator.EqualTo => value == Value,
                Comparator.NotEqualTo => value != Value,
                _ => false,
            };
        }

        protected override HashSet<Comparator> AllowedComparators { get; } = new()
        {
            Comparator.EqualTo, Comparator.NotEqualTo,
        };
    }
}