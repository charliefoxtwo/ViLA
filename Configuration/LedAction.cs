// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using Core;

#pragma warning disable 8618

namespace Configuration;

public class LedAction
{
    public string Color { get; set; }
    public BaseTrigger Trigger { get; set; }
}
