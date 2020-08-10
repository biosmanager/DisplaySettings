using System;
using System.Collections.Generic;
using System.Text;

namespace DisplaySettings
{
    public static class Util
    {
        public static string BitDepthToName(uint bitDepth)
        {
            switch (bitDepth)
            {
                case 24:
                    return "High Color";
                case 32:
                    return "True Color";
                default:
                    return "";
            }
        }
    }
}
