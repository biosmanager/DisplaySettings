using System;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DisplaySettingsChanger
{
    public static class Commands
    {
        private const string ENV_VAR = "DSC_DISPLAY_SETTINGS";

        public static void StoreResolution(int deviceID)
        {
            var displaySettings = DisplaySettings.GetDisplaySettings(deviceID);
            var jsonString = JsonSerializer.Serialize(displaySettings);

            try
            {
                Environment.SetEnvironmentVariable(ENV_VAR, jsonString, EnvironmentVariableTarget.User);
                Console.WriteLine($"Stored display settings: {displaySettings.Width}x{displaySettings.Height} @ {displaySettings.RefreshRate} Hz");
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
                SetResolution(displaySettings.Width, displaySettings.Height, displaySettings.RefreshRate, displaySettings.DisplayIndex);

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

        public static void SetResolution(int width, int height, int refreshRate, int displayIndex)
        {
            DisplaySettings.ChangeDisplaySettings(width, height, refreshRate, displayIndex);
            Console.WriteLine($"Display {displayIndex} set to {width}x{height} @ {refreshRate} Hz");
        }

        public static void EnumerateModes(int displayIndex)
        {
            var info = DisplayInformation.GetAdapterAndDisplayInformation(displayIndex);
            var modes = DisplaySettings.EnumerateAllDisplayModes(displayIndex);
            Console.Write($"Display {displayIndex}:\n" +
                          $"  Adapter: {info.adapterName} ({info.adapterDescription})\n" +
                          $"  Monitor: {info.monitorName} ({info.monitorDescription})\n" +
                          $"\n");
            Console.WriteLine($"Graphics modes of display:");
            for (int modeIndex = 0; modeIndex < modes.Length; modeIndex++)
            {
                var mode = modes[modeIndex];
                Console.WriteLine($"  {modeIndex}: \t{mode.Width}x{mode.Height}@{mode.RefreshRate}");
            }
        }
    }
}