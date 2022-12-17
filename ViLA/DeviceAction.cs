using Core;

namespace ViLA;

public class DeviceAction<T> where T : BaseTrigger
{
    public ushort DevicePid { get; }
    public string? DeviceName { get; }
    public T Trigger { get; }
    public string Color { get; }
    public Target Target { get; }

    public DeviceAction(string color, T trigger, Target target, ushort devicePid, string? deviceName)
    {
        Color = color;
        Trigger = trigger;
        Target = target;
        DevicePid = devicePid;
        DeviceName = deviceName;
    }
}
