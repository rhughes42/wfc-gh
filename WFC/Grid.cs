using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC
{
    /// <summary>
    /// Represents a 2D grid of cells for Wave Function Collapse.
    /// </summary>
    public class Grid
    {
        private static readonly (int dx, int dy)[] DirectionOffsets =
        {
            (0, -1), // North
            (1, 0),  // East
            (0, 1),  // South
            (-1, 0), // West
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class with default dimensions.
        /// </summary>
        public Grid()
        {
            ExtentsX = 10;
            ExtentsY = 10;
            Size = 6;
            Modules = new List<Module>();

            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="extX">Grid width in cells.</param>
        /// <param name="extY">Grid height in cells.</param>
        /// <param name="size">Cell spacing (model units).</param>
        /// <param name="modules">The tileset modules available for the solver.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="extX"/>, <paramref name="extY"/>, or <paramref name="size"/> is not positive.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="modules"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="modules"/> is empty.</exception>
        public Grid(int extX, int extY, int size, List<Module> modules)
        {
            if (extX <= 0) throw new ArgumentOutOfRangeException(nameof(extX), extX, "Grid width must be positive.");
            if (extY <= 0) throw new ArgumentOutOfRangeException(nameof(extY), extY, "Grid height must be positive.");
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), size, "Cell size must be positive.");
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            if (modules.Count == 0) throw new ArgumentException("Tileset must contain at least one module.", nameof(modules));

            ExtentsX = extX;
            ExtentsY = extY;
            Size = size;
            Modules = modules;

            Reset();
            Initialize();
        }

        /// <summary>
        /// Gets or sets the grid width in cells.
        /// </summary>
        public int ExtentsX { get; set; }

        /// <summary>
        /// Gets or sets the grid height in cells.
        /// </summary>
        public int ExtentsY { get; set; }

        /// <summary>
        /// Gets or sets the cell spacing (model units).
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the tileset modules available to the solver.
        /// </summary>
        public List<Module> Modules { get; set; }

        /// <summary>
        /// Gets the cell matrix in <c>[x][y]</c> layout.
        /// </summary>
        public List<List<Cell>> Matrix { get; private set; }

        /// <summary>
        /// Gets the output mesh geometry in <c>[x][y]</c> layout (only set after a successful solve).
        /// </summary>
        public List<List<Mesh>> Geometry { get; private set; }

        /// <summary>
        /// Gets the debug text in <c>[x][y]</c> layout (optional).
        /// </summary>
        public List<List<string>> Text { get; private set; }

        /// <summary>
        /// Gets the number of collapse steps performed during the last solve attempt.
        /// </summary>
        public int Steps { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of collapse steps to attempt during a solve.
        /// </summary>
        public int MaxSteps { get; set; }

        /// <summary>
        /// Gets the number of cells that still have more than one possible module.
        /// </summary>
        public int Uncertain { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the solver encountered a contradiction.
        /// </summary>
        public bool Contradiction { get; private set; }

        /// <summary>
        /// Resets solver counters and flags (does not reinitialize the cell matrix).
        /// </summary>
        public void Reset()
        {
            Steps = 0;
            MaxSteps = 1000;
            Uncertain = 0;
            Contradiction = false;
        }

        /// <summary>
        /// (Re)initializes the cell matrix and resets output containers.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Modules"/> is null or empty.</exception>
        public void Initialize()
        {
            if (Modules == null || Modules.Count == 0)
            {
                throw new InvalidOperationException("Grid must have at least one module before initialization.");
            }

            var matrix = new List<List<Cell>>();
            for (var x = 0; x < ExtentsX; x++)
            {
                matrix.Add(new List<Cell>());
                for (var y = 0; y < ExtentsY; y++)
                {
                    matrix[x].Add(new Cell(this, x, y, Modules));
                }
            }

            Matrix = matrix;
            Uncertain = ExtentsX * ExtentsY;
            Contradiction = false;
            Steps = 0;

            Geometry = Create2DList<Mesh>(ExtentsX, ExtentsY);
            Text = Create2DList<string>(ExtentsX, ExtentsY);
        }

        /// <summary>
        /// Attempts to solve the grid using Wave Function Collapse.
        /// </summary>
        /// <param name="seed">The random seed to use.</param>
        /// <param name="maxSteps">The maximum number of collapse steps to attempt.</param>
        /// <param name="meshes">Output meshes for collapsed cells (flattened, in row-major order).</param>
        /// <param name="log">Diagnostic log messages.</param>
        /// <returns><see langword="true"/> when the grid was fully solved; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSteps"/> is not positive.</exception>
        public bool TrySolve(int seed, int maxSteps, out List<Mesh> meshes, out List<string> log)
        {
            if (maxSteps <= 0) throw new ArgumentOutOfRangeException(nameof(maxSteps), maxSteps, "Max steps must be positive.");

            MaxSteps = maxSteps;
            Initialize();

            log = new List<string>();
            meshes = new List<Mesh>();

            var random = new Random(seed);

            while (Uncertain > 0 && Steps < MaxSteps)
            {
                var next = FindLowestEntropyCell(random);
                if (next == null)
                {
                    break;
                }

                Steps++;
                log.Add(string.Format("Step {0}: collapsing cell {1},{2} (entropy={3})", Steps, next.X, next.Y, next.Modules.Count));

                Module chosen;
                if (!next.TryCollapse(random, out chosen))
                {
                    Contradiction = true;
                    log.Add(string.Format("Contradiction at cell {0},{1}: no modules remaining.", next.X, next.Y));
                    break;
                }

                // Collapse reduces this cell to 1.
                Uncertain--;

                if (!Propagate(next.X, next.Y, log))
                {
                    Contradiction = true;
                    log.Add(string.Format("Contradiction encountered during propagation from {0},{1}.", next.X, next.Y));
                    break;
                }
            }

            if (Uncertain == 0 && !Contradiction)
            {
                GetGeometry(out meshes);
                log.Add("Solved.");
                return true;
            }

            if (Steps >= MaxSteps)
            {
                log.Add(string.Format("Max steps ({0}) exceeded.", MaxSteps));
            }

            return false;
        }

        /// <summary>
        /// Propagates constraints outward from a collapsed or updated cell.
        /// </summary>
        /// <param name="x">The cell X coordinate.</param>
        /// <param name="y">The cell Y coordinate.</param>
        /// <param name="log">Optional diagnostic log.</param>
        /// <returns><see langword="true"/> when propagation completes without contradiction; otherwise <see langword="false"/>.</returns>
        public bool Propagate(int x, int y, List<string> log = null)
        {
            var queue = new Queue<Cell>();
            queue.Enqueue(Matrix[x][y]);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                for (var dir = 0; dir < 4; dir++)
                {
                    var (dx, dy) = DirectionOffsets[dir];
                    var nx = cell.X + dx;
                    var ny = cell.Y + dy;
                    if (nx < 0 || nx >= ExtentsX || ny < 0 || ny >= ExtentsY)
                    {
                        continue;
                    }

                    var neighbor = Matrix[nx][ny];
                    var beforeCount = neighbor.Modules.Count;

                    var changed = ReduceNeighbourOptions(cell, neighbor, dir);
                    if (changed == null)
                    {
                        return false;
                    }

                    if (changed.Value)
                    {
                        var afterCount = neighbor.Modules.Count;
                        if (beforeCount > 1 && afterCount == 1)
                        {
                            Uncertain--;
                        }

                        if (log != null)
                        {
                            log.Add(string.Format("Reduced cell {0},{1} -> {2} possibilities.", neighbor.X, neighbor.Y, afterCount));
                        }

                        queue.Enqueue(neighbor);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Backwards-compatible misspelled alias for <see cref="Propagate(int,int,List{string})"/>.
        /// </summary>
        /// <param name="x">The cell X coordinate.</param>
        /// <param name="y">The cell Y coordinate.</param>
        /// <returns><see langword="true"/> when propagation completes without contradiction; otherwise <see langword="false"/>.</returns>
        [Obsolete("Use Propagate instead.")]
        public bool Propogate(int x, int y)
        {
            return Propagate(x, y, null);
        }

        /// <summary>
        /// Computes and caches output geometry for the current grid state.
        /// </summary>
        /// <param name="meshes">Flattened list of output meshes in row-major order.</param>
        public void GetGeometry(out List<Mesh> meshes)
        {
            meshes = new List<Mesh>();

            for (var x = 0; x < ExtentsX; x++)
            {
                for (var y = 0; y < ExtentsY; y++)
                {
                    var cell = Matrix[x][y];
                    if (!cell.IsCollapsed)
                    {
                        Text[x][y] = string.Format("X:{0}, Y:{1}, P:{2}", x * Size, y * Size, cell.Modules.Count);
                        continue;
                    }

                    var module = cell.Modules[0];
                    var geo = module.Geometry.DuplicateMesh();
                    var pt = new Point3d(x * Size, y * Size, 0);
                    var vec = new Vector3d(pt - module.Origin);
                    var xform = Transform.Translation(vec);
                    geo.Transform(xform);

                    Geometry[x][y] = geo;
                    meshes.Add(geo);
                }
            }
        }

        private Cell FindLowestEntropyCell(Random random)
        {
            Cell best = null;
            var bestEntropy = int.MaxValue;

            for (var x = 0; x < ExtentsX; x++)
            {
                for (var y = 0; y < ExtentsY; y++)
                {
                    var cell = Matrix[x][y];
                    var entropy = cell.Modules.Count;
                    if (entropy <= 1) continue;

                    if (entropy < bestEntropy)
                    {
                        best = cell;
                        bestEntropy = entropy;
                        continue;
                    }

                    if (entropy == bestEntropy && best != null && random.Next(0, 2) == 0)
                    {
                        best = cell;
                    }
                }
            }

            return best;
        }

        private bool? ReduceNeighbourOptions(Cell cell, Cell neighbor, int directionToNeighbor)
        {
            if (cell.Modules.Count == 0 || neighbor.Modules.Count == 0)
            {
                return null;
            }

            var oppositeDirection = (directionToNeighbor + 2) % 4;

            var remove = new List<Module>();
            for (var n = 0; n < neighbor.Modules.Count; n++)
            {
                var neighborModule = neighbor.Modules[n];

                var compatible = false;
                for (var c = 0; c < cell.Modules.Count; c++)
                {
                    var cellModule = cell.Modules[c];
                    var a = cellModule.Edges[directionToNeighbor];
                    var b = neighborModule.Edges[oppositeDirection];
                    if (a.Matches(b))
                    {
                        compatible = true;
                        break;
                    }
                }

                if (!compatible)
                {
                    remove.Add(neighborModule);
                }
            }

            if (remove.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < remove.Count; i++)
            {
                neighbor.Modules.Remove(remove[i]);
            }

            if (neighbor.Modules.Count == 0)
            {
                return null;
            }

            return true;
        }

        private static List<List<T>> Create2DList<T>(int xCount, int yCount)
        {
            var list = new List<List<T>>();
            for (var x = 0; x < xCount; x++)
            {
                list.Add(new List<T>());
                for (var y = 0; y < yCount; y++)
                {
                    list[x].Add(default(T));
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Represents a single cell in the WFC grid and its remaining module possibilities.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="grid">The owning grid.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="modules">The initial module possibilities.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="grid"/> or <paramref name="modules"/> is null.</exception>
        public Cell(Grid grid, int x, int y, List<Module> modules)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            if (modules == null) throw new ArgumentNullException(nameof(modules));

            GridInstance = grid;
            X = x;
            Y = y;
            Modules = new List<Module>(modules);
        }

        /// <summary>
        /// Gets the owning grid.
        /// </summary>
        public Grid GridInstance { get; private set; }

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the remaining possible modules for this cell.
        /// </summary>
        public List<Module> Modules { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the cell has been collapsed to a single module.
        /// </summary>
        public bool IsCollapsed
        {
            get { return Modules != null && Modules.Count == 1; }
        }

        /// <summary>
        /// Collapses the cell to a single randomly selected module.
        /// </summary>
        /// <param name="random">The random instance to use.</param>
        /// <param name="chosen">The chosen module.</param>
        /// <returns><see langword="true"/> when a module could be chosen; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is null.</exception>
        public bool TryCollapse(Random random, out Module chosen)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));

            chosen = null;

            if (Modules == null || Modules.Count == 0)
            {
                return false;
            }

            if (Modules.Count == 1)
            {
                chosen = Modules[0];
                return true;
            }

            var idx = random.Next(0, Modules.Count);
            chosen = Modules[idx];

            Modules.Clear();
            Modules.Add(chosen);
            return true;
        }
    }
}

