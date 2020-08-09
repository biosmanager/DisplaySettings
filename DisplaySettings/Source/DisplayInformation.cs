using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    /// <summary>
    /// Contains information about about a display adapter and all its monitors
    /// </summary>
    public sealed class DisplayInformation
    {
        // Flags based on wingdi.h from Windows SDK 10.0.16299.0
        [Flags]
        public enum DisplayDeviceStateFlags : uint
        {
            AttachedToDesktop = 0x00000001,
            MultiDriver = 0x00000002,
            PrimaryDevice = 0x00000004,
            MirroringDriver = 0x00000008,
            VgaCompatible = 0x00000010,
            Removable = 0x00000020,
            AccDriver = 0x00000040,
            ModesPruned = 0x08000000,
            RdpUdd = 0x01000000,
            Remote = 0x04000000,
            Disconnect = 0x02000000,
            TSCompatible = 0x00200000,
            UnsafeModesOn = 0x00080000
        }

        public sealed class Monitor
        {
            public int MonitorIndex { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DisplayDeviceStateFlags StateFlags { get; set; }
            public string InterfaceName { get; set; }
        }

        public int DisplayIndex { get; set; }
        public string AdapterName { get; set; }
        public string AdapterDescription { get; set; }
        public DisplayDeviceStateFlags AdapterStateFlags { get; set; }
        public Monitor[] Monitors { get; set; }

        public static DisplayInformation GetAdapterAndDisplayInformation(int displayIndex)
        {
            displayIndex = Math.Max(displayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();

            var displayInfo = new DisplayInformation();
            displayInfo.DisplayIndex = displayIndex;
            EnumDisplayDevices(null, (uint)displayIndex, ref d, 0);
            displayInfo.AdapterName = d.DeviceName;
            displayInfo.AdapterDescription = d.DeviceString;
            displayInfo.AdapterStateFlags = (DisplayDeviceStateFlags)d.StateFlags;

            var monitors = new List<Monitor>();
            for (uint monitorIndex = 0; EnumDisplayDevices(displayInfo.AdapterName, monitorIndex, ref d, 1); monitorIndex++)
            {
                monitors.Add(new Monitor
                {
                    MonitorIndex = (int)monitorIndex,
                    Name = d.DeviceName,
                    Description = d.DeviceString,
                    StateFlags = (DisplayDeviceStateFlags)d.StateFlags,
                    InterfaceName = d.DeviceID
                });
            }
            displayInfo.Monitors = monitors.ToArray();

            return displayInfo;
        }

        public static DisplayInformation[] EnumerateAllDisplays(bool doOnlyListAttached = false)
        {
            var displayInformations = new List<DisplayInformation>();

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();

            for (uint adapterIndex = 0; EnumDisplayDevices(null, adapterIndex, ref d, 0); adapterIndex++)
            {
                // Skip unattached devices
                if (doOnlyListAttached && !((DisplayDeviceStateFlags)d.StateFlags).HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                {
                    continue;
                }

                var adapterName = d.DeviceName;
                var adapterDescription = d.DeviceString;
                var adapterFlags = d.StateFlags;

                var monitors = new List<Monitor>();
                for (uint monitorIndex = 0; EnumDisplayDevices(adapterName, monitorIndex, ref d, 1); monitorIndex++)
                {
                    monitors.Add(new Monitor
                    {
                        MonitorIndex = (int)monitorIndex,
                        Name = d.DeviceName,
                        Description = d.DeviceString,
                        StateFlags = (DisplayDeviceStateFlags)d.StateFlags,
                        InterfaceName = d.DeviceID
                    });
                }

                displayInformations.Add(new DisplayInformation
                {
                    DisplayIndex = (int)adapterIndex,
                    AdapterName = adapterName,
                    AdapterDescription = adapterDescription,
                    AdapterStateFlags = (DisplayDeviceStateFlags)adapterFlags,
                    Monitors = monitors.ToArray()
                });
            }

            return displayInformations.ToArray();
        }

        public static int FindPrimaryDisplayIndex()
        {
            var devices = EnumerateAllDisplays(true);
            foreach (var device in devices)
            {
                if (device.AdapterStateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
                {
                    return device.DisplayIndex;
                }
            }

            // TODO: Is this the correct exception type? Should we throw a exception in this highly unlikely case at all?
            throw new ExternalException("No primary display attached! This should never happen.");
        }
    }
}