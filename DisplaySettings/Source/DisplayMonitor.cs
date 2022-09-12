using System.Collections.Generic;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    public sealed class Monitor : DisplayDevice
    {
        public string InterfaceName { get; private set; }


        public static IEnumerable<Monitor> EnumerateMonitors(Adapter adapter)
        {
            var device = DISPLAY_DEVICE.Create();

            for (uint monitorIndex = 0; EnumDisplayDevices(adapter.Name, monitorIndex, ref device, 1); monitorIndex++)
            {
                yield return new Monitor
                {
                    Index = monitorIndex,
                    Name = device.DeviceName,
                    Description = device.DeviceString,
                    StateFlags = (DisplayDeviceStateFlags)device.StateFlags,
                    InterfaceName = device.DeviceID
                };
            }
        }
    }
}