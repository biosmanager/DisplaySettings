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

        public enum SettingsType : uint
        {
            Current = ENUM_CURRENT_SETTINGS,
            Registry = ENUM_REGISTRY_SETTINGS
        }

        public sealed class GraphicsMode
        {
            public int Index { get; set; }
            public uint Width { get; set; }
            public uint Height { get; set; }
            public uint RefreshRate { get; set; }
            public uint BitDepth { get; set; }
        }

        public struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        public uint DisplayIndex { get; set; }
        public GraphicsMode Mode { get; set; }
        public Position DesktopPosition { get; set; }
        public bool IsAttached { get; set; }
        public bool IsPrimary { get; set; }


        public static DisplaySettingsChangedResult ChangeDisplaySettings(DisplaySettings displaySettings)
        {
            var adapter = new Adapter(displaySettings.DisplayIndex);

            DEVMODE dm = DEVMODE.Create();
            if (0 != EnumDisplaySettingsEx(adapter.Name, ENUM_CURRENT_SETTINGS, ref dm, 0))
            {
                dm.dmPelsWidth = displaySettings.Mode.Width;
                dm.dmPelsHeight = displaySettings.Mode.Height;
                dm.dmDisplayFrequency = displaySettings.Mode.RefreshRate;
                dm.dmBitsPerPel = displaySettings.Mode.BitDepth;
                dm.dmPosition.x = displaySettings.DesktopPosition.X;
                dm.dmPosition.y = displaySettings.DesktopPosition.Y;

                int iRet = ChangeDisplaySettingsEx(adapter.Name, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);

                if (iRet == (int)DisplaySettingsChangedResult.ChangeStatus.FAILED)
                {
                    return new DisplaySettingsChangedResult(iRet);
                }
                else
                {
                    iRet = ChangeDisplaySettingsEx(adapter.Name, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                    return new DisplaySettingsChangedResult(iRet);
                }
            }
            else
            {
                return new DisplaySettingsChangedResult(DisplaySettingsChangedResult.ChangeStatus.BADMODE);
            }
        }

        public static DisplaySettings GetDisplaySettings(uint displayIndex, SettingsType type = SettingsType.Current)
        {
            displayIndex = Math.Max(displayIndex, 0);


            var adapter = new Adapter(displayIndex);

            DEVMODE dm = DEVMODE.Create();
            EnumDisplaySettingsEx(adapter.Name, (uint)type, ref dm, 0);

            var displaySettings = new DisplaySettings()
            {
                DisplayIndex = displayIndex,
                Mode = new GraphicsMode
                {
                    Width = dm.dmPelsWidth,
                    Height = dm.dmPelsHeight,
                    RefreshRate = dm.dmDisplayFrequency,
                    BitDepth = dm.dmBitsPerPel
                },
                DesktopPosition = new Position { X = dm.dmPosition.x, Y = dm.dmPosition.y },
                IsAttached = adapter.IsAttached,
                IsPrimary = adapter.IsPrimary
            };

            // Find mode number
            displaySettings.Mode.Index = -1;
            for (uint iModeNum = 0; EnumDisplaySettingsEx(adapter.Name, iModeNum, ref dm, 0) != 0; iModeNum++)
            {
                var isModeMatch = displaySettings.Mode.Width == dm.dmPelsWidth &&
                                  displaySettings.Mode.Height == dm.dmPelsHeight &&
                                  displaySettings.Mode.RefreshRate == dm.dmDisplayFrequency &&
                                  displaySettings.Mode.BitDepth == dm.dmBitsPerPel;

                if (isModeMatch)
                {
                    displaySettings.Mode.Index = (int)iModeNum;
                    break;
                }
            }

            return displaySettings;
        }

        public static IEnumerable<GraphicsMode> EnumerateGraphicsModes(uint displayIndex)
        {
            var adapter = new Adapter(displayIndex);

            DEVMODE dm = DEVMODE.Create();
            for (uint iModeNum = 0; EnumDisplaySettingsEx(adapter.Name, iModeNum, ref dm, 0) != 0; iModeNum++)
            {
                yield return new GraphicsMode
                {
                    Index = (int)iModeNum,
                    Width = dm.dmPelsWidth,
                    Height = dm.dmPelsHeight,
                    RefreshRate = dm.dmDisplayFrequency,
                    BitDepth = dm.dmBitsPerPel
                };
            }
        }
    }
}