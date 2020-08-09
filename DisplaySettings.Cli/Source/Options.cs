using CommandLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DisplaySettings.Cli
{
    static class Options
    {
        [Verb("set", HelpText = "Set display settings. Any present option will override the respective current display setting. For example, " +
                                "you can just use \"-r 60\" to only change the refresh rate of a display while leaving any other setting " +
                                "(resolution, bit depth, etc.) untouched.")]
        public class SetOptions
        {
            [Option('d', "displays", Default = new string[] { "primary" }, Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\", \"attached\" or \"primary\".")]
            public IEnumerable<string> Displays { get; set; }

            [Option('w', "width", Required = false, HelpText = "Width in pixels.")]
            public int? Width { get; set; } = null;

            [Option('h', "height", Required = false, HelpText = "Height in pixels.")]
            public int? Height { get; set; } = null;

            [Option('r', "refreshRate", Required = false, HelpText = "Refresh rate in Hertz. A value of 0 or 1 indicates the default refresh rate of the display.")]
            public int? RefreshRate { get; set; } = null;

            [Option('b', "bitDepth", Required = false, HelpText = "Bits per pixel for all channels including alpha.")]
            public int? BitDepth { get; set; } = null;

            [Option("positionX", Required = false, HelpText = "X position of the display in the multi-monitor desktop configuration. The primary display has (0, 0).")]
            public int? PositionX { get; set; } = null;

            [Option("positionY", Required = false, HelpText = "Y position of the display in the multi-monitor desktop configuration. The primary display has (0, 0).")]
            public int? PositionY { get; set; } = null;

            [Option('j', "json", Required = false, HelpText = "Read display settings formatted as JSON from a file." +
                                                              "If other options are present, they will override the respective settings from the JSON settings. When overriding " +
                                                              "the display(s) of interest, the number of displays must match the number of displays" +
                                                              "in the JSON string.")]
            public bool DoJsonFormatting { get; set; } = false;

            [Option('f', "file", Required = false, HelpText = "Path to JSON file. If this option is ommited, read from standard input instead. ")]
            public string JsonFilePath { get; set; } = null;
        }

        [Verb("get", HelpText = "Get current display settings.")]
        public class GetOptions
        {
            [Option('d', "displays", Default = new string[] { "primary" }, Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\", \"attached\" or \"primary\".")]
            public IEnumerable<string> Displays { get; set; }

            [Option('j', "json", Required = false, HelpText = "Write display settings formatted as JSON to a file.")]
            public bool DoJsonFormatting { get; set; } = false;

            [Option('f', "file", Required = false, HelpText = "Path to JSON file. If this option is ommited, write to standard output instead.")]
            public string JsonFilePath { get; set; } = null;
        }

        [Verb("displays", HelpText = "Enumerate all displays.")]
        public class DisplaysOptions
        {
            [Option('a', "attached", Required = false, HelpText = "Only list displays that are attached to the desktop.")]
            public bool DoOnlyListAttached { get; set; } = false;

            [Option('j', "json", Required = false, HelpText = "Write display information formatted as JSON to a file.")]
            public bool DoJsonFormatting { get; set; } = false;

            [Option('f', "file", Required = false, HelpText = "Path to JSON file. If this option is ommited, write to standard output instead.")]
            public string JsonFilePath { get; set; } = null;
        }

        [Verb("modes", HelpText = "Enumerate all graphics modes of a display.")]
        public class ModesOptions
        {
            [Option('d', "displays", Default = new string[] { "primary" }, Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\", \"attached\" or \"primary\".")]
            public IEnumerable<string> Displays { get; set; }

            [Option('j', "json", Required = false, HelpText = "Write modes formatted as JSON to a file." +
                                                              "If other options are present, they will override the respective settings from the JSON settings. When overriding " +
                                                              "the display(s) of interest, the number of displays must match the number of displays" +
                                                              "in the JSON string.")]
            public bool DoJsonFormatting { get; set; } = false;

            [Option('f', "file", Required = false, HelpText = "Path to JSON file. If this option is ommited, write to standard output instead.")]
            public string JsonFilePath { get; set; } = null;
        }
    }
}
