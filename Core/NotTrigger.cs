namespace Core;

/**
 * Triggers if the child trigger does not pass.
 */
public class NotTrigger : BaseTrigger<BaseTrigger>
{
    public NotTrigger(BaseTrigger value) : base(value, Comparator.Not)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        return !Value.ShouldTrigger(state);
    }

    protected override HashSet<Comparator> AllowedComparators { get; } = new(Enum.GetValues<Comparator>());
}
