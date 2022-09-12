using System;
using System.Collections.Generic;

using static DisplaySettings.SafeNativeMethods;

namespace DisplaySettings
{
    /// <summary>
    /// Holds settings for a display.
    /// </summary>
    public sealed class DisplaySettings
    {
        /// <summary>
        /// Result of display settings change operation. See <see cref="ChangeDisplaySettings(DisplaySettings)"/>.
        /// </summary>
        public sealed class DisplaySettingsChangedResult
        {
            /// <summary>
            /// Possible status code of native <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see>function.
            /// </summary>
            public enum ChangeStatus : int
            {
                /// <summary>
                /// The settings change was successful.
                /// </summary>
                SUCCESSFUL = 0,
                /// <summary>
                /// The computer must be restarted for the graphics mode to work.
                /// </summary>
                RESTART = 1,
                /// <summary>
                /// The display driver failed the specified graphics mode.
                /// </summary>
                FAILED = -1,
                /// <summary>
                /// The graphics mode is not supported.
                /// </summary>
                BADMODE = -2,
                /// <summary>
                /// Unable to write settings to the registry.
                /// </summary>
                NOTUPDATED = -3,
                /// <summary>
                /// An invalid set of flags was passed in.
                /// </summary>
                BADFLAGS = -4,
                /// <summary>
                /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
                /// </summary>
                BADPARAM = -5,
                /// <summary>
                /// The settings change was unsuccessful because the system is DualView capable.
                /// </summary>
                BADDUALVIEW = -6
            }

            /// <summary>
            /// Return status reported by <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see>.
            /// </summary>
            public ChangeStatus Status { get; private set; }
            /// <summary>
            /// Description of the result.
            /// </summary>
            public string Description { get; private set; }


            internal DisplaySettingsChangedResult(ChangeStatus status)
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

            internal DisplaySettingsChangedResult(int status) : this((ChangeStatus)status)
            {
            }
        }

        /// <summary>
        /// Type of the display settings to report, either the currently set or those stored in the registry. See <see cref="GetDisplaySettings(int, SettingsType)"/>.
        /// </summary>
        public enum SettingsType : uint
        {
            /// <summary>
            /// Use current settings.
            /// </summary>
            Current = ENUM_CURRENT_SETTINGS,
            /// <summary>
            /// Use settings stored in registry.
            /// </summary>
            Registry = ENUM_REGISTRY_SETTINGS
        }

        /// <summary>
        /// Graphics mode.
        /// </summary>
        public sealed class GraphicsMode
        {
            /// <summary>
            /// Index of the mode.
            /// </summary>
            public int Index { get; set; }
            /// <summary>
            /// Width of the resolution in pixels.
            /// </summary>
            public uint Width { get; set; }
            /// <summary>
            /// Height of the resolution in pixels.
            /// </summary>
            public uint Height { get; set; }
            /// <summary>
            /// Vertical refresh rate in Hz.
            /// </summary>
            public uint RefreshRate { get; set; }
            /// <summary>
            /// Bit depth in total bits per pixel for all channels.
            /// </summary>
            public uint BitDepth { get; set; }
        }

        /// <summary>
        /// Position of upper left corner of a display in desktop configuration. The primary display is always at (0,0).
        /// </summary>
        public struct Position
        {
            /// <summary>
            /// X
            /// </summary>
            public int X { get; set; }
            /// <summary>
            /// Y
            /// </summary>
            public int Y { get; set; }
        }


        /// <summary>
        /// Index of the display/adapter.
        /// </summary>
        public uint DisplayIndex { get; set; }
        /// <summary>
        /// Graphics mode.
        /// </summary>
        public GraphicsMode Mode { get; set; }
        /// <summary>
        /// Position in desktop configuration.
        /// </summary>
        public Position DesktopPosition { get; set; }
        /// <summary>
        /// Whether the display/adapter is attached to the desktop.
        /// </summary>
        public bool IsAttached { get; set; }
        public bool IsPrimary { get; set; }


        /// <summary>
        /// Change settings of a display.
        /// </summary>
        /// <param name="displaySettings">The settings to apply.</param>
        /// <returns>Result of the operation including a description.</returns>
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

        /// <summary>
        /// Retrieve display settings.
        /// </summary>
        /// <param name="displayIndex">Index of the display/adapter of interest.</param>
        /// <param name="type">Whether to retrieve the currently active settings or those stored in the registry for that display/adapter.</param>
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

        /// <summary>
        /// List all graphics modes supported by a display.
        /// </summary>
        /// <param name="displayIndex">Index of the display/adapter of interest.</param>
        public static IEnumerable<GraphicsMode> EnumerateAllDisplayModes(uint displayIndex)
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