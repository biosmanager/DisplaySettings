using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using CommandLine;
using Extensions;

namespace DisplaySettings.Cli
{
    public static class Commands
    {
        public class CommandException : Exception
        {
            public string Command { get; private set; }

            public CommandException() : this("", "", null)
            {
            }

            public CommandException(string command) : this(command, "", null)
            {
            }

            public CommandException(string command, string message) : this(command, message, null)
            {
            }

            public CommandException(string command, string message, Exception innerException) : base(message, innerException)
            {
                Command = command;
            }
        }

        internal class DisplayInformationWithModes
        {
            public Adapter Adapter { get; set; }
            public DisplaySettings.GraphicsMode[] Modes { get; set; }
        }


        public static void SetDisplaySettings(IEnumerable<string> displays, uint? width = null, uint? height = null, uint? refreshRate = null, uint? bitDepth = null, int? positionX = null, int? positionY = null, bool doJsonFormatting = false, string jsonFilePath = null)
        {
            var commandName = "set";

            ValidateJsonOptions(commandName, doJsonFormatting, jsonFilePath);

            uint[] displayIndices;
            try
            {
                displayIndices = ParseDisplays(displays);
            }
            catch (ArgumentException e)
            {
                throw new CommandException(commandName, e.Message, e);
            }
            var hasDisplaySpecified = displays.Any();

            // Build list of display settings that will be applied
            var displaysSettings = new List<DisplaySettings>();
            if (doJsonFormatting)
            {
                // Read from standard input if option is not present or whitespace/empty, otherwise read from specified file.
                StreamReader streamReader;
                var doReadStdin = string.IsNullOrWhiteSpace(jsonFilePath);
                if (doReadStdin)
                {
                    streamReader = new StreamReader(Console.OpenStandardInput());
                }
                else
                {
                    try
                    {
                        streamReader = new StreamReader(jsonFilePath);
                    }
                    catch (IOException)
                    {
                        Console.Error.WriteLine($"Could not open JSON file {jsonFilePath} for reading.");
                        return;
                    }
                }

                // Read in whole JSON.
                string jsonString;
                try
                {
                    var sb = new StringBuilder();
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                    jsonString = sb.ToString();
                }
                catch (IOException)
                {
                    if (doReadStdin)
                    {
                        Console.Error.WriteLine("Error while reading JSON from standard input.");
                    }
                    else
                    {
                        Console.Error.WriteLine($"Error while reading JSON from {jsonFilePath}.");
                    }

                    return;
                }
                finally
                {
                    streamReader.Dispose();
                }

                // Deserialize JSON object from read string.
                try
                {
                    displaysSettings = JsonSerializer.Deserialize<List<DisplaySettings>>(jsonString);

                    if (!displaysSettings.Any())
                    {
                        Console.Error.WriteLine("JSON array is empty.");
                        return;
                    }

                    if (hasDisplaySpecified && displaysSettings.Count != displayIndices.Length)
                    {
                        Console.Error.WriteLine("Number of displays of interest must match the number of displays in the JSON string!");
                        return;
                    }
                }
                catch (JsonException e)
                {
                    Console.Error.WriteLine($"Could not deserialize display settings from given JSON string. {e}");
                    return;
                }
            } else
            {
                foreach (var displayIndex in displayIndices)
                {
                    displaysSettings.Add(DisplaySettings.GetDisplaySettings(displayIndex));
                }
            }

            // Set each display setting.
            foreach ((var displaySettings, var index) in displaysSettings.WithIndex())
            {
                // If additional options are set, the override the corresponding setting for each display.

                // Map display indices from from file to overrides.
                displaySettings.DisplayIndex = hasDisplaySpecified ? displayIndices[index] : displaySettings.DisplayIndex;

                if (width != null)
                {
                    displaySettings.Mode.Width = (uint)width;
                }
                if (height != null)
                {
                    displaySettings.Mode.Height = (uint)height;
                }
                if (refreshRate != null)
                {
                    displaySettings.Mode.RefreshRate = (uint)refreshRate;
                }
                if (bitDepth != null)
                {
                    displaySettings.Mode.BitDepth = (uint)bitDepth;
                }
                if (positionX != null)
                {
                    var desktopPosition = displaySettings.DesktopPosition;
                    desktopPosition.X = (int)positionX;
                    displaySettings.DesktopPosition = desktopPosition;
                }
                if (positionY != null)
                {
                    var desktopPosition = displaySettings.DesktopPosition;
                    desktopPosition.Y = (int)positionY;
                    displaySettings.DesktopPosition = desktopPosition;
                }
            }

            foreach (var displaySettings in displaysSettings)
            {
                var result = DisplaySettings.ChangeDisplaySettings(displaySettings);

                var colorDepthName = Util.BitDepthToName(displaySettings.Mode.BitDepth);
                if (result.Status == DisplaySettings.DisplaySettingsChangedResult.ChangeStatus.SUCCESSFUL)
                {
                    Console.WriteLine($"Display {displaySettings.DisplayIndex}: Set to {displaySettings.Mode.Width}x{displaySettings.Mode.Height} @ {displaySettings.Mode.RefreshRate} Hz, {displaySettings.Mode.BitDepth} bit{(colorDepthName == "" ? "" : $" ({colorDepthName})")}");
                }
                else if (result.Status == DisplaySettings.DisplaySettingsChangedResult.ChangeStatus.RESTART)
                {
                    Console.WriteLine($"Display {displaySettings.DisplayIndex}: {result.Description}");
                }
                else if (result.Status == DisplaySettings.DisplaySettingsChangedResult.ChangeStatus.BADMODE)
                {
                    throw new CommandException("set", $"Display {displaySettings.DisplayIndex}: The requested graphics mode {displaySettings.Mode.Width}x{displaySettings.Mode.Height} @ {displaySettings.Mode.RefreshRate} Hz, {displaySettings.Mode.BitDepth} bit{(colorDepthName == "" ? "" : $" ({colorDepthName})")} is not supported.");
                }
                else
                {
                    throw new CommandException("set", $"Display {displaySettings.DisplayIndex}: {result.Description}");
                }
            }
        }

        public static void GetDisplaySettings(IEnumerable<string> displays, bool doJsonFormatting = false, string jsonFilePath = null)
        {
            var commandName = "get";

            ValidateJsonOptions(commandName, doJsonFormatting, jsonFilePath);

            uint[] displayIndices;
            try
            {
                displayIndices = ParseDisplays(displays);
            }
            catch (ArgumentException e)
            {
                throw new CommandException(commandName, e.Message, e);
            }

            var displaysSettings = new List<DisplaySettings>();
            foreach (var displayIndex in displayIndices)
            {
                displaysSettings.Add(DisplaySettings.GetDisplaySettings(displayIndex));
            }

            if (doJsonFormatting)
            {
                WriteJson(commandName, jsonFilePath, displaysSettings);
                if (!string.IsNullOrWhiteSpace(jsonFilePath))
                {
                    Console.WriteLine($"Wrote display settings to {Path.GetFullPath(jsonFilePath)}.");
                }
            }
            else
            {
                foreach (var displaySettings in displaysSettings)
                {
                    if (displaySettings.IsAttached)
                    {
                        Console.Write($"Display {displaySettings.DisplayIndex}{(displaySettings.IsPrimary ? " (Primary)" : "")}:\n" +
                                      $"  Mode index: \t\t{displaySettings.Mode.Index}\n" +
                                      $"  Resolution: \t\t{displaySettings.Mode.Width}x{displaySettings.Mode.Height}\n" +
                                      $"  Refresh rate: \t{displaySettings.Mode.RefreshRate} Hz\n" +
                                      $"  Color bit depth: \t{displaySettings.Mode.BitDepth} bit\n" +
                                      $"  Desktop position: \t({displaySettings.DesktopPosition.X}, {displaySettings.DesktopPosition.Y})\n");
                    }
                    else
                    {
                        Console.Write($"Display {displaySettings.DisplayIndex}:\n" +
                                      $"  Detached\n");
                    }
                }
            }
        }

        public static void EnumerateModes(IEnumerable<string> displays, bool doJsonFormatting = false, string jsonFilePath = null)
        {
            var commandName = "modes";

            ValidateJsonOptions(commandName, doJsonFormatting, jsonFilePath);

            uint[] displayIndices;
            try
            {
                displayIndices = ParseDisplays(displays);
            }
            catch (ArgumentException e)
            {
                throw new CommandException(commandName, e.Message, e);
            }

            var infosWithModes = new List<DisplayInformationWithModes>();
            foreach (var displayIndex in displayIndices)
            {
                infosWithModes.Add(new DisplayInformationWithModes
                {
                    Adapter = new Adapter(displayIndex),
                    Modes = DisplaySettings.EnumerateGraphicsModes(displayIndex).ToArray()
                });
            }

            if (doJsonFormatting)
            {
                WriteJson(commandName, jsonFilePath, infosWithModes);
                if (!string.IsNullOrWhiteSpace(jsonFilePath))
                {
                    Console.WriteLine($"Wrote graphics modes to {Path.GetFullPath(jsonFilePath)}.");
                }
            }
            else
            {
                foreach (var infoWithModes in infosWithModes)
                {
                    Console.Write($"Display {infoWithModes.Adapter.Index}:\n" +
                                  $"  Adapter name: \t{infoWithModes.Adapter.Name}\n" +
                                  $"  Adapter description: \t{infoWithModes.Adapter.Description}\n");
                    Console.WriteLine($"Graphics modes:");
                    for (int modeIndex = 0; modeIndex < infoWithModes.Modes.Length; modeIndex++)
                    {
                        var mode = infoWithModes.Modes[modeIndex];
                        var colorDepthName = Util.BitDepthToName(mode.BitDepth);
                        Console.WriteLine($"  {modeIndex}: \t{mode.Width}x{mode.Height} @ {mode.RefreshRate}, {mode.BitDepth} bit{(colorDepthName == "" ? "" : $" ({colorDepthName})")}");
                    }
                }
            }
        }

        public static void EnumerateDisplays(bool doOnlyListAttached = false, bool doJsonFormatting = false, string jsonFilePath = null)
        {
            var commandName = "displays";

            ValidateJsonOptions(commandName, doJsonFormatting, jsonFilePath);

            var adapters = Adapter.EnumerateAdapters(doOnlyListAttached).ToArray();

            if (doJsonFormatting)
            {
                WriteJson(commandName, jsonFilePath, adapters);
                if (!string.IsNullOrWhiteSpace(jsonFilePath))
                {
                    Console.WriteLine($"Wrote display information to {Path.GetFullPath(jsonFilePath)}.");
                }
            }
            else
            {
                for (int displayIndex = 0; displayIndex < adapters.Length; displayIndex++)
                {
                    var adapter = adapters[displayIndex];
                    Console.Write($"Display {displayIndex}:\n" +
                                  $"  Adapter name: \t{adapter.Name}\n" +
                                  $"  Adapter description: \t{adapter.Description}\n" +
                                  $"  Adapter state: \t{adapter.StateFlags}\n" +
                                  $"  Monitors:\n");
                    foreach (var monitor in adapter.Monitors)
                    {
                        Console.Write($"    Monitor {monitor.Index}:\n" +
                                      $"      Name: \t\t{monitor.Name}\n" +
                                      $"      Description: \t{monitor.Description}\n" +
                                      $"      State: \t\t{monitor.StateFlags}\n" +
                                      $"      Interface name: \t{monitor.InterfaceName}\n");
                    }
                }
            }
        }


        private static void ValidateJsonOptions(string command, bool doJsonFormatting, string jsonFilePath)
        {
            if (doJsonFormatting && jsonFilePath != null && jsonFilePath.All(char.IsWhiteSpace))
            {
                throw new CommandException(command, "File path is empty.");
            }
            if (!doJsonFormatting && !string.IsNullOrWhiteSpace(jsonFilePath))
            {
                throw new CommandException(command, "The -f/--file option requires the -j/--json option.");
            }
        }

        private static uint[] ParseDisplays(IEnumerable<string> displays)
        {
            if (displays is null)
            {
                throw new ArgumentNullException("displays");
            }
            else if (displays.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("displays", "Must at least specify one display of interest.");
            }

            var displayIndices = new List<uint>();
            if (!displays.Any())
            {
                displayIndices.Add(Adapter.GetPrimaryAdapter().Index);
            }
            else if (displays.Count() == 1)
            {
                var display = displays.First().ToLowerInvariant();

                if (display == "primary")
                {
                    displayIndices.Add(Adapter.GetPrimaryAdapter().Index);
                }
                else if (display == "all")
                {
                    var adapters = Adapter.EnumerateAdapters(doOnlyListAttached: false);
                    foreach (var adapter in adapters)
                    {
                        displayIndices.Add(adapter.Index);
                    }
                }
                else if (display == "attached")
                {
                    var adapters = Adapter.EnumerateAdapters(doOnlyListAttached: true);
                    foreach (var adapter in adapters)
                    {
                        displayIndices.Add(adapter.Index);
                    }
                }
                else
                {
                    displayIndices.Add(ParseDisplayIndex(display));
                }
            }
            else
            {
                foreach (var display in displays)
                {
                    if (display == "primary")
                    {
                        displayIndices.Add(Adapter.GetPrimaryAdapter().Index);
                    }
                    else
                    {
                        displayIndices.Add(ParseDisplayIndex(display));
                    }
                }
            }

            // Check if list only contains distinct indices
            if (displayIndices.Count() != displayIndices.Distinct().Count())
            {
                throw new CommandException("set", "Display indices must be distinct!");
            }

            return displayIndices.ToArray();
        }

        private static uint ParseDisplayIndex(string displayIndex)
        {
            try
            {
                return uint.Parse(displayIndex);
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                throw new ArgumentException($"{displayIndex} is not a valid display index!", e);
            }
        }

        private static void WriteJson(string command, string jsonFilePath, object jsonObject)
        {
            // Write to standard input if option is empty, otherwise write to specified file.
            StreamWriter streamWriter;
            var doWriteStdout = string.IsNullOrWhiteSpace(jsonFilePath);
            if (doWriteStdout)
            {
                streamWriter = new StreamWriter(Console.OpenStandardOutput());
            }
            else
            {
                try
                {
                    streamWriter = new StreamWriter(jsonFilePath);
                }
                catch (Exception e)
                {
                    throw new CommandException(command, $"Could not open JSON file {jsonFilePath} for writing.", e);
                }
            }

            // Serialize
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            try
            {
                streamWriter.Write(JsonSerializer.Serialize(jsonObject, options));
            }
            catch (Exception e)
            {
                if (doWriteStdout)
                {
                    throw new CommandException(command, "Error while writing JSON to standard output.", e);
                }
                else
                {
                    throw new CommandException(command, $"Error while writing JSON to file {jsonFilePath}.");
                }
            }
            finally
            {
                streamWriter.Dispose();
            }
        }
    }
}