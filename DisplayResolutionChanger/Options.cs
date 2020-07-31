using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DisplaySettingsChanger
{
    static class Options
    {
        [Verb("store", HelpText = "Store current display settings.")]
        public class StoreOptions
        {
            [Option('i', "displayIndex", Required = false, HelpText = "The index value of the display of interest. Default is 0.")]
            public int DisplayIndex { get; set; } = 0;
        }

        [Verb("restore", HelpText = "Retrieve and restore previously stored display settings.")]
        public class RestoreOptions
        {
        }

        [Verb("set", HelpText = "Set display settings.")]
        public class SetOptions
        {
            [Option('i', "displayIndex", Required = false, HelpText = "The index value of the display of interest. Default is 0.")]
            public int DisplayIndex { get; set; } = 0;

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
        }

        [Verb("get", HelpText = "Get current display settings.")]
        public class GetOptions
        {
            [Option('i', "displayIndex", Required = false, HelpText = "The index value of the display of interest. Default is 0.")]
            public int DisplayIndex { get; set; } = 0;

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
            [Option('i', "displayIndex", Required = false, HelpText = "The index value of the display of interest. Default is 0.")]
            public int DisplayIndex { get; set; } = 0;

            [Option('j', "json", Required = false, HelpText = "Format as JSON.")]
            public bool DoJsonFormatting { get; set; } = false;
        }

    }
}
