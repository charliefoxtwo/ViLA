using Core;

namespace ViLA;

public class DeviceAction<T> where T : BaseTrigger
{
    public ushort Device { get; }
    public T Trigger { get; }
    public string Color { get; }
    public Target Target { get; }

    public DeviceAction(string color, T trigger, Target target, ushort device)
    {
        Color = color;
        Trigger = trigger;
        Target = target;
        Device = device;
    }
}
