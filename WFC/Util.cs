using System;

namespace WFC
{
    /// <summary>
    /// Utility helpers used across the Grasshopper components and the WFC model types.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Converts an angle from radians to degrees.
        /// </summary>
        /// <param name="radians">An angle in radians.</param>
        /// <returns>The equivalent angle in degrees.</returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Converts an angle from degrees to radians.
        /// </summary>
        /// <param name="degrees">An angle in degrees.</param>
        /// <returns>The equivalent angle in radians.</returns>
        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Remaps a value from one range to another.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="inputMin">The inclusive lower bound of the input range.</param>
        /// <param name="inputMax">The inclusive upper bound of the input range.</param>
        /// <param name="outputMin">The inclusive lower bound of the output range.</param>
        /// <param name="outputMax">The inclusive upper bound of the output range.</param>
        /// <returns>The remapped value.</returns>
        /// <exception cref="DivideByZeroException">Thrown when <paramref name="inputMin"/> equals <paramref name="inputMax"/>.</exception>
        public static double Remap(double input, double inputMin, double inputMax, double outputMin, double outputMax)
        {
            return outputMin + (input - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
        }
    }
}
