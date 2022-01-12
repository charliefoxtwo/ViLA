// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using Newtonsoft.Json;

namespace Core;

public abstract class BaseTrigger<T> : BaseTrigger
{
    /// <summary>
    /// Attempts to construct a new Trigger based on the provided parameters. Throws if an unsupported comparator is passed.
    /// </summary>
    /// <param name="id">The trigger id</param>
    /// <param name="value">The value to trigger on</param>
    /// <param name="comparator">The comparator to use when comparing a value to the trigger value</param>
    /// <exception cref="ArgumentException">Thrown when an unsupported comparator is passed</exception>
    protected BaseTrigger(string id, T value, Comparator comparator) : base(id, value, comparator)
    {
    }

    protected BaseTrigger(BaseTrigger value, Comparator comparator) : base(value, comparator) { }

    protected BaseTrigger(IReadOnlyCollection<BaseTrigger> values, Comparator comparator) : base(values, comparator) { }

    public T Value => (T) ValueObject;
}

[JsonConverter(typeof(TriggerConverter))]
public abstract class BaseTrigger
{
    public string? Id { get; }
    public IReadOnlyList<string> TriggerStrings { get; }
    protected object ValueObject { get; }
    public Comparator Comparator { get; }
    protected abstract HashSet<Comparator> AllowedComparators { get; }

    /// <summary>
    /// Attempts to construct a new Trigger based on the provided parameters. Throws if an unsupported comparator is passed.
    /// </summary>
    /// <param name="id">The trigger id</param>
    /// <param name="value">The value to trigger on</param>
    /// <param name="comparator">The comparator to use when comparing a value to the trigger value</param>
    /// <exception cref="ArgumentException">Thrown when an unsupported comparator is passed</exception>
    protected BaseTrigger(string id, object value, Comparator comparator) : this(new List<string> { id }, value, comparator)
    {
        Id = id;
    }

    protected BaseTrigger(IReadOnlyCollection<BaseTrigger> values, Comparator comparator) : this(
        values.SelectMany(v => v.TriggerStrings).Distinct().ToList(), values, comparator)
    {
    }

    protected BaseTrigger(BaseTrigger value, Comparator comparator) : this(value.TriggerStrings, value, comparator)
    {
    }

    private BaseTrigger(IReadOnlyList<string> triggerStrings, object value, Comparator comparator)
    {
        TriggerStrings = triggerStrings;
        Comparator = comparator;
        ValueObject = value;

        if (!AllowedComparators.Contains(Comparator))
        {
            throw new ArgumentException("Comparator not supported", nameof(comparator));
        }
    }

    public abstract bool ShouldTrigger(State allValues);
}
