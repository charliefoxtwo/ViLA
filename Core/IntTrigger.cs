namespace Core;

public class IntTrigger : BaseTrigger<int>
{
    public IntTrigger(string id, int value, Comparator comparator) : base(id, value, comparator)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        if (!state.TryGetValue(Id, out int value))
        {
            return false;
        }

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
