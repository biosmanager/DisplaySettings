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

            [Option('w', "width", Required = true, HelpText = "Width in pixels.")]
            public int Width { get; set; }

            [Option('h', "height", Required = true, HelpText = "Height in pixels.")]
            public int Height { get; set; }

            [Option('r', "refreshRate", Required = false, HelpText = "Refresh rate in Hertz. A value of 0 or 1 indicates the default refresh rate of the display. A value below 0 does not change the current refresh rate (default).")]
            public int RefreshRate { get; set; } = -1;
        }

        [Verb("enum", HelpText = "Enumerate all graphics modes of a given device.")]
        public class EnumOptions
        {
            [Option('i', "displayIndex", Required = false, HelpText = "The index value of the display of interest. Default is 0.")]
            public int DisplayIndex { get; set; } = 0;
        }
    }
}
