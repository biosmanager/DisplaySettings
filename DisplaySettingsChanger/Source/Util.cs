using System;
using System.Collections.Generic;
using System.Text;

namespace DisplaySettingsChanger
{
    public static class Util
    {
        public static string BitDepthToName(int bitDepth)
        {
            return bitDepth switch
            {
                24 => "High Color",
                32 => "True Color",
                _ => ""
            };
        }
    }
}
