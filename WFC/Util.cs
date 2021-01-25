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
        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }
    }
}
