using CommandLine;
using CommandLine.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace DisplaySettingsChanger
{
    class Program
    {
        static ParserResult<object> parserResult;


        static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Error.WriteLine("This tool can only run on Windows!");
                Environment.Exit(-1);
            }

            parserResult = Parser.Default.ParseArguments<Options.SetOptions, Options.GetOptions, Options.DisplaysOptions>(args)
                .WithParsed<Options.SetOptions>(options =>
                {
                    if (options.DoReadJson)
                    {
                        var sb = new StringBuilder();
                        string line;
                        while ((line = Console.ReadLine()) != null)
                        {
                            sb.AppendLine(line);
                        }
                        string jsonString = sb.ToString();

                        try
                        {
                            var displaySettings = JsonSerializer.Deserialize<DisplaySettings>(jsonString);
                            Commands.SetResolution(options.DisplayIndex, options.Width, options.Height, options.RefreshRate, options.BitDepth, options.PositionX, options.PositionY);
                        }
                        catch (JsonException)
                        {
                            Console.Error.WriteLine($"Could not deserialize display settings from given JSON string.");
                        }
                    }
                    else
                    {
                        Commands.SetResolution(options.DisplayIndex, options.Width, options.Height, options.RefreshRate, options.BitDepth, options.PositionX, options.PositionY);
                    }
                })
                .WithParsed<Options.GetOptions>(options =>
                {
                    int displayIndex;
                    var isIndex = Int32.TryParse(options.Display, out displayIndex);

                    if (!isIndex && options.Display == "primary")
                    {
                        // Find index of primary display
                        var displays = DisplayInformation.EnumerateAllDisplays(true);
                        foreach (var display in displays)
                        {
                            if (display.AdapterStateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
                            {
                                displayIndex = display.DisplayIndex;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Unknown display value \"{options.Display}\"!");
                        Console.Error.WriteLine(GetHelp<object>(parserResult));
                        return;
                    }

                    Commands.GetCurrentDisplaySettings(displayIndex, options.DoJsonFormatting);
                })
                .WithParsed<Options.DisplaysOptions>(options => Commands.EnumerateDisplays(options.DoOnlyListAttached, options.DoJsonFormatting))
                .WithParsed<Options.ModesOptions>(options => Commands.EnumerateModes(options.DisplayIndex, options.DoJsonFormatting))
                .WithNotParsed(errors => Console.Error.WriteLine(errors));
        }

        static string GetHelp<T>(ParserResult<T> result)
        {
            return HelpText.AutoBuild(result, h => h, e => e);
        }

        static void ParseDisplays(string[] displays)
        {
            if (displays.Length == 1)
            {
                switch (displays[0])
                {
                    case 
                }
            }
        }
    }
}
