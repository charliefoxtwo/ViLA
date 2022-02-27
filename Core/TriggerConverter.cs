using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core;

public class TriggerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(BaseTrigger);
    }

    private static object TriggerForComparator(Comparator comparator, string id, JToken? value)
    {
        return value?.Type switch
        {
            JTokenType.Integer => new DoubleTrigger(id, value.Value<int>(), comparator),
            JTokenType.Float => new DoubleTrigger(id, value.Value<double>(), comparator),
            JTokenType.String => new StringTrigger(id, value.Value<string>()!, comparator),
            JTokenType.Boolean => new BoolTrigger(id, value.Value<bool>(), comparator),
            _ => throw new ArgumentOutOfRangeException(nameof(value),
                $"unsupported type {value?.Type} for comparator {comparator}"),
        };
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var comparator = Enum.Parse<Comparator>(jsonObject
                .Property(nameof(BaseTrigger<object>.Comparator), StringComparison.InvariantCultureIgnoreCase)
                ?.Value.Value<string>() ?? nameof(Comparator.None), true);
        var id = jsonObject
                .Property(nameof(BaseTrigger<object>.Id), StringComparison.InvariantCultureIgnoreCase)
                ?.Value.Value<string>() ?? string.Empty;
        var v1 = jsonObject.Property(nameof(BaseTrigger<object>.Value), StringComparison.InvariantCultureIgnoreCase);
        var value = v1?.Value;
        return comparator switch
        {
            Comparator.None => new BasicTrigger(id),
            Comparator.GreaterThan or Comparator.LessThan or Comparator.GreaterThanOrEqualTo
                or Comparator.LessThanOrEqualTo or Comparator.EqualTo or Comparator.NotEqualTo => TriggerForComparator(comparator, id, value),
            Comparator.RegexMatch or Comparator.RegexNoMatch => new StringTrigger(id, value!.Value<string>()!, comparator),
            Comparator.And => new AndTrigger(value!.ToObject<List<BaseTrigger>>()!),
            Comparator.Or => new OrTrigger(value!.ToObject<List<BaseTrigger>>()!),
            Comparator.Xor => new XorTrigger(value!.ToObject<List<BaseTrigger>>()!),
            Comparator.Not => new NotTrigger(value!.ToObject<BaseTrigger>()!),
            _ => throw new Exception("Unknown comparator " + comparator),
        };
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}
