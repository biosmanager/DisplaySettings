using System;
using System.Collections.Generic;
using System.Text;

namespace DisplaySettings
{
    /// <summary>
    /// Holds various utility methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Get name for commonly used bit depths.
        /// </summary>
        /// <param name="bitDepth">Bit depth in bits per pixel for all channels.</param>
        /// <returns><c>High Color</c> for 24 bit, <c>True Color</c> for 32 bit, empty string for all other bit depths.</returns>
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
