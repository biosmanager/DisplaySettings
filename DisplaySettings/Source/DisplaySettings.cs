using System;
using System.Collections.Generic;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    public sealed class DisplaySettings
    {
        public sealed class DisplaySettingsChangedResult
        {
            public enum ChangeStatus : int
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

            public ChangeStatus Status { get; private set; }
            public string Description { get; private set; }

            public DisplaySettingsChangedResult(ChangeStatus status)
            {
                Status = status;
                switch (Status)
                {
                    case ChangeStatus.SUCCESSFUL:
                        Description = "Display settings changed successfully.";
                        break;
                    case ChangeStatus.RESTART:
                        Description = "You must restart your computer to apply the requested graphics mode.";
                        break;
                    case ChangeStatus.BADDUALVIEW:
                        Description = "Bad dual view.";
                        break;
                    case ChangeStatus.BADFLAGS:
                        Description = "Invalid set of flags passed.";
                        break;
                    case ChangeStatus.BADMODE:
                        Description = "The requested graphics mode is not supported.";
                        break;
                    case ChangeStatus.BADPARAM:
                        Description = "Invalid parameter passed.";
                        break;
                    case ChangeStatus.FAILED:
                        Description = "Display driver failed requested graphics mode.";
                        break;
                    case ChangeStatus.NOTUPDATED:
                        Description = "Unable to write display settings to registry.";
                        break;
                    default:
                        Description = "Unknown error occured.";
                        break;
                };
            }

            public DisplaySettingsChangedResult(int status) : this((ChangeStatus)status)
            {
            }
        }

        public enum SettingsType : int
        {
            Current = ENUM_CURRENT_SETTINGS,
            Registry = ENUM_REGISTRY_SETTINGS
        }

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


        public static DisplaySettingsChangedResult ChangeDisplaySettings(DisplaySettings displaySettings)
        {
            displaySettings.DisplayIndex = Math.Max(displaySettings.DisplayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            EnumDisplayDevices(null, (uint)displaySettings.DisplayIndex, ref d, 1);

            if (0 != EnumDisplaySettingsEx(d.DeviceName, ENUM_CURRENT_SETTINGS, ref dm, 0))
            {
                dm.dmPelsWidth = (uint)displaySettings.Mode.Width;
                dm.dmPelsHeight = (uint)displaySettings.Mode.Height;
                dm.dmDisplayFrequency = (uint)displaySettings.Mode.RefreshRate;
                dm.dmBitsPerPel = (uint)displaySettings.Mode.BitDepth;
                dm.dmPosition.x = displaySettings.DesktopPosition.X;
                dm.dmPosition.y = displaySettings.DesktopPosition.Y;

                int iRet = ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);

                if (iRet == (int)DisplaySettingsChangedResult.ChangeStatus.FAILED)
                {
                    return new DisplaySettingsChangedResult(iRet);
                }
                else
                {
                    iRet = ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                    return new DisplaySettingsChangedResult(iRet);
                }
            }
            else
            {
                return new DisplaySettingsChangedResult(DisplaySettingsChangedResult.ChangeStatus.BADMODE);
            }
        }

        public static DisplaySettings GetDisplaySettings(int displayIndex, SettingsType type = SettingsType.Current)
        {
            displayIndex = Math.Max(displayIndex, 0);

            DISPLAY_DEVICE d = DISPLAY_DEVICE.Create();
            DEVMODE dm = DEVMODE.Create();

            EnumDisplayDevices(null, (uint)displayIndex, ref d, 1);
            var isAttached = ((DisplayInformation.DisplayDeviceStateFlags)d.StateFlags).HasFlag(DisplayInformation.DisplayDeviceStateFlags.AttachedToDesktop);
            EnumDisplaySettingsEx(d.DeviceName, (int)type, ref dm, 0);

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
            for (int iModeNum = 0; EnumDisplaySettingsEx(d.DeviceName, iModeNum, ref dm, 0) != 0; iModeNum++)
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

            EnumDisplayDevices(null, (uint)displayIndex, ref d, 1);

            for (int iModeNum = 0; EnumDisplaySettingsEx(d.DeviceName, iModeNum, ref dm, 0) != 0; iModeNum++)
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