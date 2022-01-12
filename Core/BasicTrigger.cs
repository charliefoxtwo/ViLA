namespace Core;

public class BasicTrigger : BaseTrigger<dynamic>
{
    public BasicTrigger(string id) : base(id, false, Comparator.None)
    {
    }

    protected override HashSet<Comparator> AllowedComparators { get; } = new() { Comparator.None };

    public override bool ShouldTrigger(State _)
    {
        return true;
    }
}
