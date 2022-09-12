using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    public sealed class Adapter : DisplayDevice
    {
        public IEnumerable<Monitor> Monitors { get; private set; }
        public bool IsPrimary
        {
            get
            {
                return StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice);
            }
        }


        public Adapter(uint adapterIndex)
        {
            adapterIndex = Math.Max(adapterIndex, 0);

            var device = DISPLAY_DEVICE.Create();
            EnumDisplayDevices(null, adapterIndex, ref device, 0);

            Index = adapterIndex;
            Name = device.DeviceName;
            Description = device.DeviceString;
            StateFlags = (DisplayDeviceStateFlags)device.StateFlags;
            Monitors = Monitor.EnumerateMonitors(this);
        }

        public static IEnumerable<Adapter> EnumerateAdapters(bool doOnlyListAttached = false)
        {
            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();

            for (uint adapterIndex = 0; EnumDisplayDevices(null, adapterIndex, ref d, 0); adapterIndex++)
            {
                var adapter = new Adapter(adapterIndex);

                // Skip unattached devices
                if (doOnlyListAttached && !adapter.IsAttached)
                {
                    continue;
                }

                yield return adapter;
            }
        }

        public static Adapter GetPrimaryAdapter() {
            foreach (var adapter in EnumerateAdapters(doOnlyListAttached: true))
            {
                if (adapter.IsPrimary)
                {
                    return adapter;
                }
            }

            // TODO: Is this the correct exception type? Should we throw a exception in this highly unlikely case at all?
            throw new ExternalException("No primary display attached! This should never happen.");
        }
    }
}