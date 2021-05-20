using System.Collections.Generic;
using System.Text.RegularExpressions;
using Configuration;

namespace ViLA.Triggers
{
    public class StringTrigger : TriggerBase<string>
    {
        private readonly Regex? _regex;

        public StringTrigger(string id, string value, Comparator comparator) : base(id, value, comparator)
        {
            if (comparator is Comparator.RegexMatch or Comparator.RegexNoMatch)
            {
                _regex = new Regex(value);
            }
        }

        public override bool ShouldTrigger(string value)
        {
            return Comparator switch
            {
                Comparator.EqualTo => value == Value,
                Comparator.NotEqualTo => value != Value,
                Comparator.RegexMatch => _regex!.IsMatch(value),
                Comparator.RegexNoMatch => !_regex!.IsMatch(value),
                _ => false,
            };
        }

        protected override HashSet<Comparator> AllowedComparators { get; } = new()
        {
            Comparator.EqualTo, Comparator.NotEqualTo, Comparator.RegexMatch, Comparator.RegexNoMatch,
        };
    }
}