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
        public List<List<Mesh>> Geometry { get; set; }
        public List<List<string>> Text { get; set; }
        public int Steps { get; set; }
        public int MaxSteps { get; set; }

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
            for (int i = 0; i < this.ExtentsX; i++)
            {
                List<Cell> row = new List<Cell>();
                List<bool> uncertain = new List<bool>();
                for (int j = 0; j < this.ExtentsY; j++)
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

        public bool Propogate(int x, int y)
        {
            this.Steps += 1;
            if (this.Steps > this.MaxSteps) return false;

            return true;
        }

        public void Compute()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to collapse each cell in the grid
        /// and compute the final geometry.
        /// </summary>
        public void GetGeometry()
        {
            for (int i = 0; i < this.ExtentsX; i++)
            {
                for (int j = 0; j < this.ExtentsY; j++)
                {
                    Cell cell = this.Matrix[i][j];
                    int possibilities = cell.Modules.Count;
                    if (possibilities == 1)
                    {
                        if (cell.Collapse(out Mesh geo))
                        {
                            this.Geometry[i][j] = geo;
                            this.Uncertain[i][j] = false;
                        }
                    }
                    else
                    {
                        // TODO: Implement Z as third position.
                        this.Text[i][j] = String.Format("X:{0}, Y:{1}, P:{2}",
                            (i * this.SizeX).ToString(),
                            (j * this.SizeY).ToString(),
                            possibilities.ToString());
                    }
                }
            }
        }
    }

    public class Cell
    {
        public Grid GridInstance { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<double[]> Neighbours { get; set; }
        public List<Module> Modules { get; set; }
        public bool Certain { get; set; }

        public Cell(Grid grid, int x, int y, List<Module> modules)
        {
            this.GridInstance = grid;
            this.X = x;
            this.Y = y;
            this.Modules = modules;

            List<double[]> nCoords = GetNeighbours();

            for(int i = 0; i < nCoords.Count; i++)
            {
                double[] nC = nCoords[i];

                // Skip the neighbour if there is none.
                if (double.IsNaN(nC[0]) && double.IsNaN(nC[1])) continue;
                int nX = (int)nC[0];
                int nY = (int)nC[1];

                Cell neighbour = this.GridInstance.Matrix[nX][nY];
                List<Edge> nEdges = neighbour.GetBorder(i);
                List<Edge> cEdges = this.GridInstance.Matrix[x][y].GetBorder(i);

                /*
                 *  #First update based on neighbours. keep only edges_cell that are in edges_neighour
                    #print ['N', 'E', 'S', 'W'][i], 'cell:', edges_cell, 'neighbour:', edges_neighour
                */

            }
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
        public List<double[]> GetNeighbours()
        {
            int x = this.X;
            int y = this.Y;

            List<double> nX = new List<double>();
            List<double> nY = new List<double>();

            if (y < this.GridInstance.ExtentsY - 1)
            {
                nX.Add(x); nY.Add(y + 1);
            }
            else
            {
                nX.Add(double.NaN); nY.Add(double.NaN);
            }
            if (x < this.GridInstance.ExtentsX - 1)
            {
                nX.Add(x + 1); nY.Add(y);
            }
            else
            {
                nX.Add(double.NaN); nY.Add(double.NaN);
            }
            if (y > 0)
            {
                nX.Add(x); nY.Add(y - 1);
            }
            else
            {
                nX.Add(double.NaN); nY.Add(double.NaN);
            }
            if (x > 0)
            {
                nX.Add(x - 1); nY.Add(y);
            }
            else
            {
                nX.Add(double.NaN); nY.Add(double.NaN);
            }

            List<double[]> coords = new List<double[]>();
            for(int i = 0; i < nX.Count; i++)
            {
                double[] c = { nX[i], nY[i] };
                coords.Add(c);
            }

            return coords;
        }

        /// <summary>
        /// Attempt to collapse the cell.
        /// </summary>
        /// <returns></returns>
        public bool Collapse(out Mesh meshOut)
        {
            try
            {
                var rand = new Random();
                // Choose a random module from the remaining modules.
                Module mod = this.Modules[(int)Util.Remap(rand.NextDouble(), 0, 1, 0, this.Modules.Count - 1)];

                Mesh geo = mod.Geometry.DuplicateMesh();

                // TODO: Implement 3D positioning.
                Point3d pt = new Point3d(this.X * this.GridInstance.SizeX, this.Y * this.GridInstance.SizeY, 0);

                Vector3d vec = new Vector3d(pt - mod.Origin);
                Transform xForm = Transform.Translation(vec);
                geo.Transform(xForm);

                meshOut = geo;
                return true;
            }
            catch { meshOut = null; return false; }
        }
    }
}
