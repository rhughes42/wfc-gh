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

        public void Compute()
        {
            throw new NotImplementedException();
        }

        public void GetGeometry()
        {
            throw new NotImplementedException();
        }
    }

    public class Cell
    {
        public Grid GridInstance { get; set; }
        public List<int> Coords { get; set; }
        public List<int> NeighbourCoords { get; set; }
        public List<Module> Modules { get; set; }

        public Cell()
        {

        }

        /// <summary>
        /// Return an unsorted set of edges
        /// representing the border.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<Edge> GetBorder(int index)
        {
            HashSet<Edge> edges = new HashSet<Edge>();

            foreach (Module m in this.Modules)
            {
                edges.Add(m.Edges[index]);
            }
            return edges.ToList();
        }

        public List<int> GetNeighbours()
        {
            int x = this.Coords[0];
            int y = this.Coords[1];

            List<int> coords = new List<int>();

            if (y < this.GridInstance.ExtentsY - 1)
            {
                coords.Add(x);
                coords.Add(y + 1);
            }
            if (x < this.GridInstance.ExtentsX - 1)
            {
                coords.Add(x + 1);
                coords.Add(y);
            }
            if (y > 0)
            {
                coords.Add(x);
                coords.Add(y - 1);
            }
            if (x > 0)
            {
                coords.Add(x - 1);
                coords.Add(y);
            }

            return coords;
        }

        public void Collapse()
        {
            throw new NotImplementedException();
        }
    }
}
