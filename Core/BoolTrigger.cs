namespace Core;

public class BoolTrigger : BaseTrigger<bool>
{
    public BoolTrigger(string id, bool value, Comparator comparator) : base(id, value, comparator)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        if (!state.TryGetValue(Id, out bool value))
        {
            return false;
        }

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
