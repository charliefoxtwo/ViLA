// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;

namespace Configuration
{
    public abstract class TriggerBase<T> : TriggerBase
    {
        protected T Value { get; }
        protected Comparator Comparator { get; }
        protected abstract HashSet<Comparator> AllowedComparators { get; }

        /// <summary>
        /// Attempts to construct a new Trigger based on the provided parameters. Throws if an unsupported comparator is passed.
        /// </summary>
        /// <param name="id">The trigger id</param>
        /// <param name="value">The value to trigger on</param>
        /// <param name="comparator">The comparator to use when comparing a value to the trigger value</param>
        /// <exception cref="ArgumentException">Thrown when an unsupported comparator is passed</exception>
        protected TriggerBase(string id, T value, Comparator comparator) : base(id)
        {
            Value = value;
            Comparator = comparator;

            if (!AllowedComparators.Contains(Comparator))
            {
                throw new ArgumentException("Comparator not supported", nameof(comparator));
            }
        }

        public abstract bool ShouldTrigger(T value);
    }

    public abstract class TriggerBase
    {
        public string Id { get; set; }

        protected TriggerBase(string id) => Id = id;
    }
}