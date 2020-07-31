using CommandLine;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DisplaySettingsChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Error.WriteLine("This tool can only run on Windows!");
                Environment.Exit(-1);
            }

            Parser.Default.ParseArguments<Options.StoreOptions, Options.RestoreOptions, Options.SetOptions, Options.GetOptions, Options.DisplaysOptions, Options.ModesOptions>(args)
                .WithParsed<Options.StoreOptions>(options => Commands.StoreResolution(options.DisplayIndex))
                .WithParsed<Options.RestoreOptions>(options => Commands.RestoreResolution())
                .WithParsed<Options.SetOptions>(options => Commands.SetResolution(options.DisplayIndex, options.Width, options.Height, options.RefreshRate, options.BitDepth, options.PositionX, options.PositionY))
                .WithParsed<Options.GetOptions>(options => Commands.GetCurrentDisplaySettings(options.DisplayIndex, options.DoJsonFormatting))
                .WithParsed<Options.DisplaysOptions>(options => Commands.EnumerateDisplays(options.DoOnlyListAttached, options.DoJsonFormatting))
                .WithParsed<Options.ModesOptions>(options => Commands.EnumerateModes(options.DisplayIndex, options.DoJsonFormatting))
                .WithNotParsed(errors => Console.Error.WriteLine(errors));
        }
    }
}
