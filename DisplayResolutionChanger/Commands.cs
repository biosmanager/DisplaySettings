using System;
using System.Security;
using System.Text.Json;

namespace DisplaySettingsChanger
{
    public static class Commands
    {
        private const string ENV_VAR = "DSC_DISPLAY_SETTINGS";

        public static void StoreResolution(int deviceID)
        {
            var displaySettings = DisplaySettings.GetCurrentDisplaySettings(deviceID);
            var jsonString = JsonSerializer.Serialize(displaySettings);

            try
            {
                Environment.SetEnvironmentVariable(ENV_VAR, jsonString, EnvironmentVariableTarget.User);
                Console.WriteLine($"Stored display settings: {displaySettings.Mode.Width}x{displaySettings.Mode.Height} @ {displaySettings.Mode.RefreshRate} Hz");
            }
            catch (SecurityException e)
            {
                Console.Error.WriteLine($"Insufficient permissions to write environment variable in order to store current display settings! {e}");
            }
        }

        public static void RestoreResolution()
        {
            try
            {
                var jsonString = Environment.GetEnvironmentVariable(ENV_VAR, EnvironmentVariableTarget.User);
                var displaySettings = JsonSerializer.Deserialize<DisplaySettings>(jsonString);
                SetResolution(displaySettings.DisplayIndex, displaySettings.Mode.Width, displaySettings.Mode.Height, displaySettings.Mode.RefreshRate);

                // Delete environment variable
                Environment.SetEnvironmentVariable(ENV_VAR, null, EnvironmentVariableTarget.User);

                Console.WriteLine($"Restored display settings.");
            }
            catch (SecurityException e)
            {
                Console.Error.WriteLine($"Insufficient permissions to read/write environment variable in order to restore display settings! {e}");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("No stored display settings available.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Could not deserialize stored display settings from environment variable. {e}");
            }
        }

        public static void SetResolution(int displayIndex, int? width = null, int? height = null, int? refreshRate = null, int? bitDepth = null, int? positionX = null, int? positionY = null)
        {
            var result = DisplaySettings.ChangeDisplaySettings(displayIndex, width, height, refreshRate, bitDepth, positionX, positionY);

            if (result == DISP_CHANGE.SUCCESSFUL)
            {
                Console.WriteLine($"Display {displayIndex} set to {width}x{height} @ {refreshRate} Hz");
            }
            else
            {
                Console.Error.WriteLine(result switch
                {
                    DISP_CHANGE.BADDUALVIEW => "Bad dual view.",
                    DISP_CHANGE.BADFLAGS => "Invalid set of flags passed.",
                    DISP_CHANGE.BADMODE => "The requested graphics mode is not supported.",
                    DISP_CHANGE.BADPARAM => "Invalid parameter passed.",
                    DISP_CHANGE.FAILED => "Display driver failed requested graphics mode.",
                    DISP_CHANGE.NOTUPDATED => "Unable to write display settings to registry.",
                    DISP_CHANGE.RESTART => "You must restart your computer to apply the requested graphics mode.",
                    _ => "Unknown error occured."
                });
            }
        }

        public static void GetCurrentDisplaySettings(int displayIndex, bool doJsonFormatting = false)
        {
            var displaySettings = DisplaySettings.GetCurrentDisplaySettings(displayIndex);

            if (doJsonFormatting)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                Console.WriteLine(JsonSerializer.Serialize(displaySettings, options));
            }
            else
            {
                Console.Write($"Display {displayIndex}:\n" +
                          $"  Resolution: \t\t{displaySettings.Mode.Width}x{displaySettings.Mode.Height}\n" +
                          $"  Refresh rate: \t{displaySettings.Mode.RefreshRate} Hz\n" +
                          $"  Color bit depth: \t{displaySettings.Mode.BitDepth} bit\n" +
                          $"  Desktop position: \t({displaySettings.DesktopPosition.X}, {displaySettings.DesktopPosition.Y})\n");
            }
        }

        public static void EnumerateModes(int displayIndex, bool doJsonFormatting = false)
        {
            var info = DisplayInformation.GetAdapterAndDisplayInformation(displayIndex);
            var modes = DisplaySettings.EnumerateAllDisplayModes(displayIndex);

            if (doJsonFormatting)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var serializationObject = new
                {
                    DisplayInformation = info,
                    GraphicsModes = modes
                };
                Console.WriteLine(JsonSerializer.Serialize(serializationObject, options));
            }
            else
            {
                Console.Write($"Display {displayIndex}:\n" +
                              $"  Adapter name: \t{info.AdapterName}\n" +
                              $"  Adapter description: \t{info.AdapterDescription}\n");
                Console.WriteLine($"Graphics modes:");
                for (int modeIndex = 0; modeIndex < modes.Length; modeIndex++)
                {
                    var mode = modes[modeIndex];
                    var colorDepthName = mode.BitDepth switch
                    {
                        24 => " (High Color)",
                        32 => " (True Color)",
                        _ => ""
                    };
                    Console.WriteLine($"  {modeIndex}: \t{mode.Width}x{mode.Height}@{mode.RefreshRate}, {mode.BitDepth} bit{colorDepthName}");
                }
            }
        }

        public static void EnumerateDisplays(bool doOnlyListAttached = false, bool doJsonFormatting = false)
        {
            var displayInformations = DisplayInformation.EnumerateAllDisplays(doOnlyListAttached);

            if (doJsonFormatting)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                Console.WriteLine(JsonSerializer.Serialize(displayInformations, options));
            }
            else
            {
                for (int displayIndex = 0; displayIndex < displayInformations.Length; displayIndex++)
                {
                    var info = displayInformations[displayIndex];
                    Console.Write($"Display {displayIndex}:\n" +
                                  $"  Adapter name: \t{info.AdapterName}\n" +
                                  $"  Adapter description: \t{info.AdapterDescription}\n" +
                                  $"  Adapter state: \t{info.AdapterStateFlags}\n" +
                                  $"  Monitors:\n");
                    foreach (var monitor in info.Monitors)
                    {
                        Console.Write($"    Monitor {monitor.MonitorIndex}:\n" +
                                      $"      Name: \t\t{monitor.Name}\n" +
                                      $"      Description: \t{monitor.Description}\n" +
                                      $"      State: \t\t{monitor.StateFlags}\n" +
                                      $"      Interface name: \t{monitor.InterfaceName}\n");
                    }
                }
            }
        }
    }
}