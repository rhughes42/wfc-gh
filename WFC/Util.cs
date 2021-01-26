using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFC
{
    /// <summary>
    /// Simple helper function class.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Convert from Radians to Degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Remap a number from an input range
        /// to an output range.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputMin"></param>
        /// <param name="inputMax"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Remap(double input, double inputMin, double inputMax, double min, double max)
        {
            return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
        }
    }
}
