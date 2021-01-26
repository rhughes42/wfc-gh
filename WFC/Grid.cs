using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFC
{
    /// <summary>
    /// Our grid computation class.
    /// </summary>
    public class Grid
    {
        public int ExtentsX { get; set; }
        public int ExtentsY { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public List<Module> Modules { get; set; }

        /// <summary>
        /// Default (empty) constructor.
        /// </summary>
        public Grid()
        {
            this.ExtentsX = 10;
            this.ExtentsY = 10;
            this.SizeX = 6;
            this.SizeY = 6;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="extX"></param>
        /// <param name="extY"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="modules"></param>
        public Grid(int extX, int extY, int sizeX, int sizeY, List<Module> modules)
        {
            this.ExtentsX = extX;
            this.ExtentsY = extY;
            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.Modules = modules;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Propogate()
        {
            throw new NotImplementedException();
        }

        public void GetGeometry()
        {
            throw new NotImplementedException();
        }
    }
}
