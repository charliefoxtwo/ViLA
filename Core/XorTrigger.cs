namespace Core;

/**
 * Triggers if at least one but not all of the child triggers pass.
 */
public class XorTrigger : BaseTrigger<List<BaseTrigger>>
{
    public XorTrigger(IReadOnlyCollection<BaseTrigger> value) : base(value, Comparator.Xor)
    {
    }

    public override bool ShouldTrigger(State state)
    {
        // consecutive XORs will just be a "OneOf", so this will ensure that only one is valid without early termination
        return Value.Where(t => t.ShouldTrigger(state)).Take(2).Count() == 1;
    }

    protected override HashSet<Comparator> AllowedComparators { get; } = new(Enum.GetValues<Comparator>());
}
