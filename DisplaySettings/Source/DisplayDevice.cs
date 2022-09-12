using System;

namespace DisplaySettings
{
    public abstract class DisplayDevice
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


        public uint Index { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DisplayDeviceStateFlags StateFlags { get; protected set; }
        public bool IsAttached {
            get
            {
                return StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop);
            }
        }
    }
}