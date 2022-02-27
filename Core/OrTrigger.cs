namespace Core;

/**
 * Triggers if any of the child triggers pass.
 */
public class OrTrigger : BaseTrigger<List<BaseTrigger>>
{
    public OrTrigger(IReadOnlyCollection<BaseTrigger> value) : base(value, Comparator.Or)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        return Value.Any(t => t.ShouldTrigger(state));
    }

    protected override HashSet<Comparator> AllowedComparators { get; } = new(Enum.GetValues<Comparator>());
}
