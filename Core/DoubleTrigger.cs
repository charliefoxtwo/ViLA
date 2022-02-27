namespace Core;

public class DoubleTrigger : BaseTrigger<double>
{
    public DoubleTrigger(string id, double value, Comparator comparator) : base(id, value, comparator)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        if (!state.TryGetValue(Id!, out double value))
        {
            return false;
        }

        return Comparator switch
        {
            Comparator.GreaterThan => value > Value,
            Comparator.LessThan => value < Value,
            Comparator.EqualTo => Math.Abs(value - Value) < 0.01,
            Comparator.NotEqualTo => Math.Abs(value - Value) > 0.01,
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
