using CommandLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DisplaySettingsChanger
{
    static class Options
    {
        [Verb("set", HelpText = "Set display settings. Any present option will override the respective current display setting. For example, " +
                                "you can just use \"-r 60\" to only change the refresh rate of a display while leaving any other setting " +
                                "(resolution, bit depth, etc.) untouched.")]
        public class SetOptions
        {
            [Option('d', "displays", Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\" or \"primary\" (default).")]
            public IEnumerable<string> Displays { get; set; } = new string[] { "primary" };

            [Option('w', "width", Required = false, HelpText = "Width in pixels.")]
            public int? Width { get; set; } = null;

            [Option('h', "height", Required = false, HelpText = "Height in pixels.")]
            public int? Height { get; set; } = null;

            [Option('r', "refreshRate", Required = false, HelpText = "Refresh rate in Hertz. A value of 0 or 1 indicates the default refresh rate of the display.")]
            public int? RefreshRate { get; set; } = null;

            [Option('b', "bitDepth", Required = false)]
            public int? BitDepth { get; set; } = null;

            [Option("positionX", Required = false)]
            public int? PositionX { get; set; } = null;

            [Option("positionY", Required = false)]
            public int? PositionY { get; set; } = null;

            [Option('j', "json", Required = false, HelpText = "Read display settings formatted as JSON string. If other options are present, " +
                                                              "they will override the respective settings from the JSON settings. When overriding " +
                                                              "the display(s) of interest, the number of displays must match the number of displays" + 
                                                              "in the JSON string.")]
            public bool DoReadJson { get; set; } = false;
        }

        [Verb("get", HelpText = "Get current display settings.")]
        public class GetOptions
        {
            [Option('d', "displays", Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\" or \"primary\" (default).")]
            public IEnumerable<string> Displays { get; set; } = new string[] { "primary" };

            [Option('j', "json", Required = false, HelpText = "Format as JSON.")]
            public bool DoJsonFormatting { get; set; } = false;
        }

        [Verb("displays", HelpText = "Enumerate all displays.")]
        public class DisplaysOptions
        {
            [Option('a', "attached", Required = false, HelpText = "Only list displays that are attached to the desktop.")]
            public bool DoOnlyListAttached { get; set; } = false;

            [Option('j', "json", Required = false, HelpText = "Format as JSON.")]
            public bool DoJsonFormatting { get; set; } = false;
        }

        [Verb("modes", HelpText = "Enumerate all graphics modes of a display.")]
        public class ModesOptions
        {
            [Option('d', "displays", Separator = ',', Required = false, HelpText = "The display(s) of interest. Can be either a comma-separated sequence of " +
                                                                  "display indices (>= 0), \"all\" or \"primary\" (default).")]
            public IEnumerable<string> Displays { get; set; } = new string[] { "primary" };

            [Option('j', "json", Required = false, HelpText = "Format as JSON.")]
            public bool DoJsonFormatting { get; set; } = false;
        }
    }
}
