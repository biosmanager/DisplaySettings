using System;
using System.Runtime.InteropServices;

namespace DisplaySettings
{
    internal class SafeNativeMethods
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            /// <summary>
            /// Creates a DISPLAY_DEVICE structure with the correct size.
            /// </summary>
            /// <returns></returns>
            public static DISPLAY_DEVICE Create()
            {
                var displayDevice = new DISPLAY_DEVICE();
                displayDevice.cb = Marshal.SizeOf(displayDevice);
                return displayDevice;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSpecVersion;
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverVersion;
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSize;
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverExtra;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmFields;
            public POINTL dmPosition;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayOrientation;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFixedOutput;
            [MarshalAs(UnmanagedType.I2)]
            public short dmColor;
            [MarshalAs(UnmanagedType.I2)]
            public short dmDuplex;
            [MarshalAs(UnmanagedType.I2)]
            public short dmYResolution;
            [MarshalAs(UnmanagedType.I2)]
            public short dmTTOption;
            [MarshalAs(UnmanagedType.I2)]
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmLogPixels;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmBitsPerPel;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsWidth;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsHeight;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFlags;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFrequency;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMMethod;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMIntent;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmMediaType;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmDitherType;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved1;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved2;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningWidth;
            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningHeight;

            /// <summary>
            /// Creates a DEVMODE structure with the correct size.
            /// </summary>
            /// <returns></returns>
            public static DEVMODE Create()
            {
                var dm = new DEVMODE();
                dm.dmDeviceName = new string(new char[32]);
                dm.dmFormName = new string(new char[32]);
                dm.dmSize = (ushort)Marshal.SizeOf(dm);
                return dm;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINTL
        {
            public int x;
            public int y;
        }

        [Flags]
        internal enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }

        [DllImport("user32.dll")]
        internal static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        [DllImport("user32.dll")]
        internal static extern int EnumDisplaySettingsEx(string lpszDeviceName, uint iModeNum, ref DEVMODE lpDevMode, uint dwFlags);
        [DllImport("user32.dll")]
        internal static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        internal const uint ENUM_CURRENT_SETTINGS = unchecked((uint)-1);
        internal const uint ENUM_REGISTRY_SETTINGS = unchecked((uint)-2);
    }
}