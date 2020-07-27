//-----------------------------------------------------------------------------
// Description: Sets resolution of a specified display through the call of
//              ScreenResolution.ChangeResolution().
//
// Author: Timothy Mui (https://github.com/timmui)
//
// Date: Jan. 7, 2015
//
// Acknowledgements: Many thanks to Andy Schneider for providing the original code
//                   for a single monitor.
//                   TechNet (https://gallery.technet.microsoft.com/ScriptCenter/2a631d72-206d-4036-a3f2-2e150f297515/)
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DisplaySettingsChanger
{
    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
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

    [Flags]
    public enum ChangeDisplaySettingsFlags : uint
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
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

    // 8-bytes structure
    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        public Int32 x;
        public Int32 y;
    }

    [Flags]
    public enum DEVMODE_FIELDS : int
    {
        DM_PELSWIDTH = 0x00080000,
        DM_PELSHEIGHT = 0x00100000,
        DM_DISPLAYFREQUENCY = 0x00400000
    }

    [Flags()]
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

    public class User32
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettingsEx(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        public const int ENUM_CURRENT_SETTINGS = -1;
    }



    public class DisplaySettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int RefreshRate { get; set; }
        public int DisplayIndex { get; set; }

        // Arguments
        // int width : Desired Width in pixels
        // int height : Desired Height in pixels
        // int deviceIDIn : DeviceID of the monitor to be changed. DeviceID starts with 0 representing your first
        //                  monitor. For Laptops, the built-in display is usually 0.

        public static string ChangeDisplaySettings(int width, int height, int refreshRate, int deviceID)
        {
            //Basic Error Check
            if (deviceID < 0)
            {
                deviceID = 0;
            }

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            // Get Device Information
            User32.EnumDisplayDevices(null, (uint)deviceID, ref d, 1);

            // Attempt to change settings
            if (0 != User32.EnumDisplaySettingsEx(d.DeviceName, User32.ENUM_CURRENT_SETTINGS, ref dm, 0))
            {

                dm.dmPelsWidth = (uint)width;
                dm.dmPelsHeight = (uint)height;
                if (refreshRate >= 0)
                {
                    dm.dmDisplayFrequency = (uint)refreshRate;
                }

                int iRet = User32.ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);

                if (iRet == (int)DISP_CHANGE.FAILED)
                {
                    return "Unable To Process Your Request. Sorry For This Inconvenience.";
                }
                else
                {
                    iRet = User32.ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                    switch (iRet)
                    {
                        case (int)DISP_CHANGE.SUCCESSFUL:
                            {
                                return "Success";
                            }
                        case (int)DISP_CHANGE.RESTART:
                            {
                                return "You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.";
                            }
                        default:
                            {
                                return "Failed To Change The Resolution.";
                            }
                    }

                }

            }
            else
            {
                return "Failed To Change The Resolution.";
            }
        }

        public static DisplaySettings GetDisplaySettings(int deviceID)
        {
            //Basic Error Check
            if (deviceID < 0)
            {
                deviceID = 0;
            }

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            // Get Device Information
            User32.EnumDisplayDevices(null, (uint)deviceID, ref d, 1);

            // Retrieve display settings
            User32.EnumDisplaySettingsEx(d.DeviceName, User32.ENUM_CURRENT_SETTINGS, ref dm, 0);

            return new DisplaySettings()
            {
                Width = (int)dm.dmPelsWidth,
                Height = (int)dm.dmPelsHeight,
                RefreshRate = (int)dm.dmDisplayFrequency,
                DisplayIndex = deviceID
            };
        }

        public static DisplaySettings[] EnumerateAllDisplayModes(int deviceID)
        {
            var displayModes = new List<DisplaySettings>();

            //Basic Error Check
            if (deviceID < 0)
            {
                deviceID = 0;
            }

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            // Get Device Information
            User32.EnumDisplayDevices(null, (uint)deviceID, ref d, 1);

            // Retrieve display settings
            for (int iModeNum = 0; User32.EnumDisplaySettingsEx(d.DeviceName, iModeNum, ref dm, 0) != 0; iModeNum++)
            {
                displayModes.Add(new DisplaySettings()
                {
                    Width = (int)dm.dmPelsWidth,
                    Height = (int)dm.dmPelsHeight,
                    RefreshRate = (int)dm.dmDisplayFrequency,
                    DisplayIndex = deviceID
                });
            }

            return displayModes.ToArray();
        }

        public static (string, string) GetDisplayAndAdapterName(int deviceID)
        {
            //Basic Error Check
            if (deviceID < 0)
            {
                deviceID = 0;
            }

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            User32.EnumDisplayDevices(null, (uint)deviceID, ref d, 1);

            return (d.DeviceName, d.DeviceString);
        }
    }
}