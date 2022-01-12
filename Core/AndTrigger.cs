namespace Core;

/**
 * Triggers if all of the child triggers pass.
 */
public class AndTrigger : BaseTrigger<List<BaseTrigger>>
{
    public AndTrigger(IReadOnlyCollection<BaseTrigger> value) : base(value, Comparator.And)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        return Value.All(t => t.ShouldTrigger(state));
    }

    protected override HashSet<Comparator> AllowedComparators { get; } = new(Enum.GetValues<Comparator>());
}
