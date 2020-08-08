// Based on https://github.com/timmui/ScreenResolutionChanger/blob/master/C%23%20Script/Set-ScreenResolutionEx.cs

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace DisplaySettingsChanger
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
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

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
        public UInt16 dmSpecVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSize;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverExtra;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmFields;
        public POINTL dmPosition;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayOrientation;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFixedOutput;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmColor;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmDuplex;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmYResolution;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmTTOption;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmLogPixels;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmBitsPerPel;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsWidth;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsHeight;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFlags;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFrequency;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMMethod;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMIntent;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmMediaType;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDitherType;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved1;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved2;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningWidth;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningHeight;

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
        public Int32 x;
        public Int32 y;
    }

    [Flags]
    public enum DISP_CHANGE : int
    {
        SUCCESSFUL = 0,
        RESTART = 1,
        FAILED = -1,
        BADMODE = -2,
        NOTUPDATED = -3,
        BADFLAGS = -4,
        BADPARAM = -5,
        BADDUALVIEW = -6
    }

    // Flags based on wingdi.h from Windows SDK 10.0.16299.0
    [Flags]
    public enum DisplayDeviceStateFlags : int
    {
        AttachedToDesktop = 0x00000001,
        MultiDriver       = 0x00000002,
        PrimaryDevice     = 0x00000004,
        MirroringDriver   = 0x00000008,
        VgaCompatible     = 0x00000010,
        Removable         = 0x00000020,
        AccDriver         = 0x00000040,
        ModesPruned       = 0x08000000,
        RdpUdd            = 0x01000000,
        Remote            = 0x04000000,
        Disconnect        = 0x02000000,
        TSCompatible      = 0x00200000,
        UnsafeModesOn     = 0x00080000
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

    internal class User32
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        public const int ENUM_CURRENT_SETTINGS = -1;
    }


    public sealed class DisplayInformation
    {
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
            User32.EnumDisplayDevices(null, (uint)displayIndex, ref d, 0);
            displayInfo.AdapterName = d.DeviceName;
            displayInfo.AdapterDescription = d.DeviceString;
            displayInfo.AdapterStateFlags = d.StateFlags;

            var monitors = new List<Monitor>();
            for (uint monitorIndex = 0; User32.EnumDisplayDevices(displayInfo.AdapterName, monitorIndex, ref d, 1); monitorIndex++)
            {
                monitors.Add(new Monitor
                {
                    MonitorIndex = (int)monitorIndex,
                    Name = d.DeviceName,
                    Description = d.DeviceString,
                    StateFlags = d.StateFlags,
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

            for (uint adapterIndex = 0; User32.EnumDisplayDevices(null, adapterIndex, ref d, 0); adapterIndex++)
            {
                // Skip unattached devices
                if (doOnlyListAttached && !d.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                {
                    continue;
                }

                var adapterName = d.DeviceName;
                var adapterDescription = d.DeviceString;
                var adapterFlags = d.StateFlags;

                var monitors = new List<Monitor>();
                for (uint monitorIndex = 0; User32.EnumDisplayDevices(adapterName, monitorIndex, ref d, 1); monitorIndex++)
                {
                    monitors.Add(new Monitor
                    {
                        MonitorIndex = (int)monitorIndex,
                        Name = d.DeviceName,
                        Description = d.DeviceString,
                        StateFlags = d.StateFlags,
                        InterfaceName = d.DeviceID
                    });
                }

                displayInformations.Add(new DisplayInformation
                {
                    DisplayIndex = (int)adapterIndex,
                    AdapterName = adapterName,
                    AdapterDescription = adapterDescription,
                    AdapterStateFlags = adapterFlags,
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

    public sealed class DisplaySettings
    {
        public sealed class GraphicsMode
        {
            public int Index { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int RefreshRate { get; set; }
            public int BitDepth { get; set; }
        }

        public struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public int DisplayIndex { get; set; }
        public GraphicsMode Mode { get; set; }
        public Position DesktopPosition { get; set; }
        public bool IsAttached { get; set; }

        public static DISP_CHANGE ChangeDisplaySettings(DisplaySettings displaySettings)
        {
            displaySettings.DisplayIndex = Math.Max(displaySettings.DisplayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            User32.EnumDisplayDevices(null, (uint)displaySettings.DisplayIndex, ref d, 1);

            if (0 != User32.EnumDisplaySettingsEx(d.DeviceName, User32.ENUM_CURRENT_SETTINGS, ref dm, 0))
            {
                dm.dmPelsWidth = (uint)displaySettings.Mode.Width;
                dm.dmPelsHeight = (uint)displaySettings.Mode.Height;
                dm.dmDisplayFrequency = (uint)displaySettings.Mode.RefreshRate;
                dm.dmBitsPerPel = (uint)displaySettings.Mode.BitDepth;
                dm.dmPosition.x = displaySettings.DesktopPosition.X;
                dm.dmPosition.y = displaySettings.DesktopPosition.Y;

                int iRet = User32.ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);

                if (iRet == (int)DISP_CHANGE.FAILED)
                {
                    return (DISP_CHANGE)iRet;
                }
                else
                {
                    iRet = User32.ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                    return (DISP_CHANGE)iRet;
                }
            }
            else
            {
                return DISP_CHANGE.BADMODE;
            }
        }

        public static DisplaySettings GetCurrentDisplaySettings(int displayIndex)
        {
            displayIndex = Math.Max(displayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            User32.EnumDisplayDevices(null, (uint)displayIndex, ref d, 1);
            var isAttached = d.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop);
            User32.EnumDisplaySettingsEx(d.DeviceName, User32.ENUM_CURRENT_SETTINGS, ref dm, 0);

            var displaySettings = new DisplaySettings()
            {
                DisplayIndex = displayIndex,
                Mode = new GraphicsMode
                {
                    Width = (int)dm.dmPelsWidth,
                    Height = (int)dm.dmPelsHeight,
                    RefreshRate = (int)dm.dmDisplayFrequency,
                    BitDepth = (int)dm.dmBitsPerPel
                },
                DesktopPosition = new Position { X = dm.dmPosition.x, Y = dm.dmPosition.y },
                IsAttached = isAttached
            };

            // Find mode number
            displaySettings.Mode.Index = -1;
            for (int iModeNum = 0; User32.EnumDisplaySettingsEx(d.DeviceName, iModeNum, ref dm, 0) != 0; iModeNum++)
            {
                var isModeMatch = displaySettings.Mode.Width == dm.dmPelsWidth &&
                                  displaySettings.Mode.Height == dm.dmPelsHeight &&
                                  displaySettings.Mode.RefreshRate == dm.dmDisplayFrequency &&
                                  displaySettings.Mode.BitDepth == dm.dmBitsPerPel;

                if (isModeMatch)
                {
                    displaySettings.Mode.Index = iModeNum;
                    break;
                }
            }

            return displaySettings;
        }

        public static GraphicsMode[] EnumerateAllDisplayModes(int displayIndex)
        {
            var displayModes = new List<GraphicsMode>();

            displayIndex = Math.Max(displayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            User32.EnumDisplayDevices(null, (uint)displayIndex, ref d, 1);

            for (int iModeNum = 0; User32.EnumDisplaySettingsEx(d.DeviceName, iModeNum, ref dm, 0) != 0; iModeNum++)
            {
                displayModes.Add(new GraphicsMode
                {
                    Index = iModeNum,
                    Width = (int)dm.dmPelsWidth,
                    Height = (int)dm.dmPelsHeight,
                    RefreshRate = (int)dm.dmDisplayFrequency,
                    BitDepth = (int)dm.dmBitsPerPel
                });
            }

            return displayModes.ToArray();
        }
    }
}