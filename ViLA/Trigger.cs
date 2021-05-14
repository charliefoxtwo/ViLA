// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable 8618
using System;

namespace ViLA
{
    public class Trigger
    {
        public string Id { get; set; } = null!;
        public int Value { get; set; }
        public Comparator Comparator { get; set; }

        public bool ShouldTrigger(int value)
        {
            return Comparator switch
            {
                Comparator.GreaterThan => value > Value,
                Comparator.LessThan => value < Value,
                Comparator.EqualTo => value == Value,
                Comparator.NotEqualTo => value != Value,
                Comparator.GreaterThanOrEqualTo => value >= Value,
                Comparator.LessThanOrEqualTo => value <= Value,
                _ => throw new ArgumentOutOfRangeException(nameof(Comparator))
            };
        }
    }
}