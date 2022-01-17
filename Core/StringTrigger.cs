using System.Text.RegularExpressions;

namespace Core;

public class StringTrigger : BaseTrigger<string>
{
    private readonly Regex? _regex;

    public StringTrigger(string id, string value, Comparator comparator) : base(id, value, comparator)
    {
        if (comparator is Comparator.RegexMatch or Comparator.RegexNoMatch)
        {
            _regex = new Regex(value);
        }
    }

    public override bool ShouldTrigger(State state)
    {
        if (!state.TryGetValue(Id!, out string? value) || value is null)
        {
            return false;
        }

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
