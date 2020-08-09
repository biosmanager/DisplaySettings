using CommandLine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DisplaySettingsChanger
{
    class Program
    {
        static ParserResult<object> parserResult;


        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Error.WriteLine("This tool can only run on Windows!");
                Environment.Exit(-1);
            }

            try
            {
                parserResult = Parser.Default.ParseArguments<Options.SetOptions, Options.GetOptions, Options.DisplaysOptions, Options.ModesOptions>(args)
                    .WithParsed<Options.SetOptions>(options =>
                    {
                        // Remove empty or whitespace elements, e.g. 0,  1, 3 becomes 0,1,3.
                        var displays = options.Displays.Where(s => !string.IsNullOrWhiteSpace(s));

                        Commands.SetDisplaySettings(options.Displays, options.Width, options.Height, options.RefreshRate, options.BitDepth, options.PositionX, options.PositionY, options.DoJsonFormatting, options.JsonFilePath);
                    })
                    .WithParsed<Options.GetOptions>(options =>
                    {
                        // Remove empty or whitespace elements, e.g. 0,  1, 3 becomes 0,1,3.
                        var displays = options.Displays.Where(s => !string.IsNullOrWhiteSpace(s));

                        Commands.GetDisplaySettings(displays, options.DoJsonFormatting, options.JsonFilePath);
                    })
                    .WithParsed<Options.DisplaysOptions>(options =>
                    {
                        Commands.EnumerateDisplays(options.DoOnlyListAttached, options.DoJsonFormatting, options.JsonFilePath);
                    })
                    .WithParsed<Options.ModesOptions>(options =>
                    {
                        // Remove empty or whitespace elements, e.g. 0,  1, 3 becomes 0,1,3.
                        var displays = options.Displays.Where(s => !string.IsNullOrWhiteSpace(s));

                        Commands.EnumerateModes(displays, options.DoJsonFormatting, options.JsonFilePath);
                    })
                    .WithNotParsed(errors =>
                    {
                        Console.Error.WriteLine(errors);
                    });
            }
            catch (Commands.CommandException commandException)
            {
                Console.Error.WriteLine($"Error in command \"{commandException.Command}\": {commandException.Message}");
            }

        }
    }
}
