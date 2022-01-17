using System.Diagnostics.CodeAnalysis;

namespace Core;

public class State : Dictionary<string, dynamic>
{
    public bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        if (ContainsKey(key))
        {
            value = (T) this[key];
            return true;
        }

        return false;
    }
}
