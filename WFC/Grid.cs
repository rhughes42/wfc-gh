using Rhino.Geometry;
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
        public List<List<Cell>> Matrix { get; set; }
        public List<List<bool>> Uncertain { get; set; }
        public bool Contradiction { get; set; }

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

            Initialize();
        }

        public void Initialize()
        {
            // Set up the grid and uncertainty matrices.
            for(int i = 0; i < this.ExtentsX; i++)
            {
                List<Cell> row = new List<Cell>();
                List<bool> uncertain = new List<bool>();
                for(int j = 0; j < this.ExtentsY; j++)
                {
                    Cell cell = new Cell(this, i, j, this.Modules);
                    row.Add(cell);
                    uncertain.Add(true);
                }
                this.Matrix.Add(row);
                this.Uncertain.Add(uncertain);
                row.Clear();
                uncertain.Clear();
            }
            this.Contradiction = false;
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
        public int X { get; set; }
        public int Y { get; set; }
        public int nX { get; set; }
        public int nY { get; set; }
        public List<Module> Modules { get; set; }

        public Cell(Grid grid, int x, int y, List<Module> modules)
        {
            this.GridInstance = grid;
            this.X = x;
            this.Y = y;
            this.Modules = modules;

            List<int> nCoords = GetNeighbours();
            nX = nCoords[0];
            nY = nCoords[1];
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

        /// <summary>
        /// Return the neighbouring coordinates
        /// of the cell.
        /// </summary>
        /// <returns></returns>
        public List<int> GetNeighbours()
        {
            int x = this.X;
            int y = this.Y;

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

        /// <summary>
        /// Attempt to collapse the cell.
        /// </summary>
        /// <returns></returns>
        public bool Collapse()
        {
            try
            {
                var rand = new Random();
                // Choose a random module from the remaining modules.
                Module mod = this.Modules[(int)Util.Remap(rand.NextDouble(), 0, 1, 0, this.Modules.Count - 1)];
                this.Modules.Clear();
                this.Modules.Add(mod);

                Mesh geo = mod.Geometry.DuplicateMesh();

                // TODO: Implement 3D positioning.
                Point3d pt = new Point3d(this.X * this.GridInstance.SizeX, this.Y * this.GridInstance.SizeY, 0);

                Vector3d vec = new Vector3d(pt - mod.Origin);
                Transform xForm = Transform.Translation(vec);
                geo.Transform(xForm);

                mod.Geometry = geo;

                return true;
            }
            catch { return false; }
        }
    }
}
