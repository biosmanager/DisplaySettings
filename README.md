# Display Settings 
[![.NET Core](https://github.com/biosmanager/DisplaySettingsChanger/workflows/.NET%20Core/badge.svg)](https://github.com/biosmanager/DisplaySettings/actions)
[![Nuget](https://img.shields.io/nuget/v/DisplaySettings)](https://www.nuget.org/packages/DisplaySettings/)

The swiss army knife to change or retrieve display settings with a library or a CLI tool on Windows.
The library targets **.NET Standard 2.0** and is therefore compatible with **.NET Framework 4.6.1 and .NET Core 2.0 or higher**.

## Features

* Set display mode (resolution, refresh rate, bit depth) and position in multi-monitor desktop configuration
* Get display settings
* Enumerate displays and available modes
* Write and read settings to/from JSON either standard input/output or files

## Projects

| Project | Description | Target framework |
|---------|-------------|------------------|
| DisplaySettings | Library to change or retrieve display information and settings. | .NET Standard 2.0 |
| DisplaySettings.Cli | Command line interface tool to do the same. | .NET Core 3.1 |

## CLI

You can use the command line interface tool to change or retrieve display information/settings. This requires that you have the **.NET Core 3.1 x64 runtime** installed.

### Examples

Set the resolution of the primary display:
```
DisplaySettings set -d primary -w 1920 -h 1080
```

Save the current settings of display 2 and 3 to a file:
```
DisplaySettings get -d 2,3 -j -f settings.json
```

Write the current settings of all attached displays as JSON to standard output:
```
DisplaySettings get -d attached -j
```

### Usage

```
set         Set display settings. Any present option will override the respective current display setting. For
            example, you can just use "-r 60" to only change the refresh rate of a display while leaving any other
            setting (resolution, bit depth, etc.) untouched.

get         Get current display settings.

displays    Enumerate all displays.

modes       Enumerate all graphics modes of a display.

help        Display more information on a specific command.

version     Display version information.
```

### Commands

#### set

```
-d, --displays       (Default: primary) The display(s) of interest. Can be either a comma-separated sequence of
                    display indices (>= 0), "all", "attached" or "primary".

-w, --width          Width in pixels.

-h, --height         Height in pixels.

-r, --refreshRate    Refresh rate in Hertz. A value of 0 or 1 indicates the default refresh rate of the display.

-b, --bitDepth       Bits per pixel for all channels including alpha.

--positionX          X position of the display in the multi-monitor desktop configuration. The primary display has (0,
                    0).

--positionY          Y position of the display in the multi-monitor desktop configuration. The primary display has (0,
                    0).

-j, --json           Read display settings formatted as JSON from a file.If other options are present, they will
                    override the respective settings from the JSON settings. When overriding the display(s) of
                    interest, the number of displays must match the number of displaysin the JSON string.

-f, --file           Path to JSON file. If this option is ommited, read from standard input instead.

--help               Display this help screen.

--version            Display version information.
```

#### get

```
-d, --displays    (Default: primary) The display(s) of interest. Can be either a comma-separated sequence of display
                indices (>= 0), "all", "attached" or "primary".

-j, --json        Write display settings formatted as JSON to a file.

-f, --file        Path to JSON file. If this option is ommited, write to standard output instead.

--help            Display this help screen.

--version         Display version information.
```

#### modes 

```
-d, --displays    (Default: primary) The display(s) of interest. Can be either a comma-separated sequence of display
                indices (>= 0), "all", "attached" or "primary".

-j, --json        Write modes formatted as JSON to a file.If other options are present, they will override the
                respective settings from the JSON settings. When overriding the display(s) of interest, the number
                of displays must match the number of displaysin the JSON string.

-f, --file        Path to JSON file. If this option is ommited, write to standard output instead.

--help            Display this help screen.

--version         Display version information.
```

## Features not yet implemented

* Extended/clone configuration
* Display orientation
* Attach/detach displays
* Set/retrieve settings via DDC/CI (turn display on/off, brightness, etc.)
* Set the maximum resolution available on a display
