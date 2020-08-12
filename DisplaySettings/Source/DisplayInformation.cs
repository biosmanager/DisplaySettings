using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    /// <summary>
    /// Contains information about about a display adapter and all its monitors.
    /// </summary>
    public sealed class DisplayInformation
    {
        /// <summary>
        /// States of a of display adapter or monitor.
        /// </summary>
        /// <remarks>
        /// Flags based on wingdi.h from Windows SDK 10.0.16299.0.
        /// </remarks>
        [Flags]
        public enum DisplayDeviceStateFlags : uint
        {
            /// <summary>
            /// Device is currently attached to the desktop.
            /// </summary>
            AttachedToDesktop = 0x00000001,
            /// <summary>
            /// Not documented.
            /// </summary>
            MultiDriver = 0x00000002,
            /// <summary>
            /// Device is the primary device.
            /// </summary>
            PrimaryDevice = 0x00000004,
            /// <summary>
            /// Device is mirroring another device.
            /// </summary>
            MirroringDriver = 0x00000008,
            /// <summary>
            /// Not documented.
            /// </summary>
            VgaCompatible = 0x00000010,
            /// <summary>
            /// Not documented.
            /// </summary>
            Removable = 0x00000020,
            /// <summary>
            /// Not documented.
            /// </summary>
            AccDriver = 0x00000040,
            /// <summary>
            /// Not documented.
            /// </summary>
            ModesPruned = 0x08000000,
            /// <summary>
            /// Not documented.
            /// </summary>
            RdpUdd = 0x01000000,
            /// <summary>
            /// Not documented.
            /// </summary>
            Remote = 0x04000000,
            /// <summary>
            /// Not documented.
            /// </summary>
            Disconnect = 0x02000000,
            /// <summary>
            /// Not documented.
            /// </summary>
            TSCompatible = 0x00200000,
            /// <summary>
            /// Device has unsafe graphics modes enabled.
            /// </summary>
            UnsafeModesOn = 0x00080000
        }

        /// <summary>
        /// Monitor device
        /// </summary>
        public sealed class Monitor
        {
            /// <summary>
            /// Index on adapter. It can be -1 if the mode cannot be found in all available modes for a display.
            /// </summary>
            public int MonitorIndex { get; set; }
            /// <summary>
            /// Path on adapter.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Name of the monitor.
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// Current states of the monitor.
            /// </summary>
            public DisplayDeviceStateFlags StateFlags { get; set; }
            /// <summary>
            /// Device interface name and GUID. Can be used with SetupAPI.
            /// </summary>
            public string InterfaceName { get; set; }
        }

        /// <summary>
        /// Index of the adapter/display.
        /// </summary>
        public int DisplayIndex { get; set; }
        /// <summary>
        /// Adapter path.
        /// </summary>
        public string AdapterName { get; set; }
        /// <summary>
        /// Name of the adapter.
        /// </summary>
        public string AdapterDescription { get; set; }
        /// <summary>
        /// Current states of the adapter.
        /// </summary>
        public DisplayDeviceStateFlags AdapterStateFlags { get; set; }
        /// <summary>
        /// All monitors associated with the adapter.
        /// </summary>
        public Monitor[] Monitors { get; set; }


        /// <summary>
        /// Retrieves information about the adapter/display and its associated monitors.
        /// </summary>
        /// <param name="displayIndex">Adapter/display of interest.</param>
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

        /// <summary>
        /// Lists all displays on the system.
        /// </summary>
        /// <param name="doOnlyListAttached">Determines if only displays/adapters that are attached to the desktop should be considered.</param>
        /// <returns>Can be empty if no adapters/displays are present.</returns>
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

        /// <summary>
        /// Find index of primary display/adapter.
        /// </summary>
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