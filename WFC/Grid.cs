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
        public int Size { get; set; }

        public List<Module> Modules { get; set; }
        public List<List<Cell>> Matrix { get; set; }

        public List<List<Mesh>> Geometry { get; set; }
        public List<List<string>> Text { get; set; }
        public int Steps { get; set; }
        public int MaxSteps { get; set; }

        public int Uncertain { get; set; }
        public bool Contradiction { get; set; }

        /// <summary>
        /// Default (empty) constructor.
        /// </summary>
        public Grid()
        {
            this.ExtentsX = 10;
            this.ExtentsY = 10;
            this.Size = 6;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="extX"></param>
        /// <param name="extY"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="modules"></param>
        public Grid(int extX, int extY, int size, List<Module> modules)
        {
            this.ExtentsX = extX;
            this.ExtentsY = extY;
            this.Size = size;
            this.Modules = modules;

            Initialize();
        }

        public void Initialize()
        {
            this.Steps = 0;
            // Set up the grid and uncertainty matrices.
            for (int i = 0; i < this.ExtentsX; i++)
            {
                List<Cell> row = new List<Cell>();
                for (int j = 0; j < this.ExtentsY; j++)
                {
                    Cell cell = new Cell(this, i, j, this.Modules);
                    row.Add(cell);
                    Uncertain++;
                }
                this.Matrix.Add(row);
                row.Clear();
            }
            this.Contradiction = false;
        }

        public bool Propogate(int x, int y)
        {
            this.Steps += 1;
            if (this.Steps > this.MaxSteps) return false;

            Cell cell = this.Matrix[x][y];
            List<double[]> nCoords = GetNeighbours(cell);

            for (int i = 0; i < nCoords.Count; i++)
            {
                double[] nC = nCoords[i];

                // Skip the neighbour if there is none.
                if (double.IsNaN(nC[0]) && double.IsNaN(nC[1])) continue;
                int nX = (int)nC[0];
                int nY = (int)nC[1];

                Cell neighbour = this.Matrix[nX][nY];
                List<Edge> nEdges = GetBorder((i + 2) % 4);
                List<Edge> cEdges = GetBorder(i);

                // First, update based on neighbours. Keep only cell edges that are in the neighbouring cells.
                foreach (Edge e in cEdges)
                {
                    Edge eOpp = new Edge(e.Name, (e.Type * 2) % 3);
                    if (!nEdges.Contains(eOpp))
                    {
                        // Remove the possibilities that rely on this.
                        HashSet<Module> reliantSet = new HashSet<Module>();
                        foreach (Module m in this.Matrix[x][y].Modules)
                        {
                            if (m.Edges[i] == e)
                                reliantSet.Add(m);
                        }
                        this.Modules = reliantSet.ToList();
                    }
                }

                // Secondly, check if the neighbour needs to be propogated.
                foreach (Edge e in nEdges)
                {
                    Edge eOpp = new Edge(e.Name, (e.Type * 2) % 3);
                    if (!cEdges.Contains(eOpp))
                    {
                        Propogate(nX, nY); // Recursive?
                    }
                }
            }

            // If a cell only has one option left, mark it as certain.
            if (this.Modules.Count == 0)
            {
                this.Contradiction = true;
                throw new Exception("Unresolveable state reached.");
            }
            // Success
            if (this.Modules.Count == 1)
            {
                this.Uncertain -= 1;
            }

            return true;
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
                            this.Uncertain -= 1;
                        }
                    }
                    else
                    {
                        // TODO: Implement Z as third position.
                        this.Text[i][j] = String.Format("X:{0}, Y:{1}, P:{2}",
                            (i * this.Size).ToString(),
                            (j * this.Size).ToString(),
                            possibilities.ToString());
                    }
                }
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
        public List<double[]> GetNeighbours(Cell cell)
        {
            int x = cell.X;
            int y = cell.Y;

            List<double> nX = new List<double>();
            List<double> nY = new List<double>();

            if (y < cell.GridInstance.ExtentsY - 1)
            {
                nX.Add(x); nY.Add(y + 1);
            }
            else
            {
                nX.Add(double.NaN); nY.Add(double.NaN);
            }
            if (x < cell.GridInstance.ExtentsX - 1)
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
            for (int i = 0; i < nX.Count; i++)
            {
                double[] c = { nX[i], nY[i] };
                coords.Add(c);
            }

            return coords;
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
                Point3d pt = new Point3d(this.X * this.GridInstance.Size, this.Y * this.GridInstance.Size, 0);

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
