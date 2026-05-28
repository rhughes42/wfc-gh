using System.Collections.Generic;
using System.Linq;

namespace Wfc.Core;

/// <summary>
/// A simple, deterministic Wave Function Collapse solver for 2D grids using named edge connectors.
/// </summary>
public sealed class WfcSolver
{
    private readonly WfcProblem _problem;
    private readonly WfcOptions _options;
    private readonly Random _random;
    private readonly List<string> _log = new();
    private readonly HashSet<int>[] _possibleByCell;

    /// <summary>
    /// Initializes a new instance of the <see cref="WfcSolver"/> class.
    /// </summary>
    /// <param name="problem">The WFC problem definition.</param>
    /// <param name="options">The solver options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="problem"/> or <paramref name="options"/> is null.</exception>
    public WfcSolver(WfcProblem problem, WfcOptions options)
    {
        _problem = problem ?? throw new ArgumentNullException(nameof(problem));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _random = new Random(_options.Seed);

        _possibleByCell = new HashSet<int>[checked(_problem.Width * _problem.Height)];
        for (var i = 0; i < _possibleByCell.Length; i++)
        {
            _possibleByCell[i] = CreateFullSet(_problem.Tiles.Count);
        }
    }

    /// <summary>
    /// Solves the problem using constraint propagation and lowest-entropy selection.
    /// </summary>
    /// <returns>A <see cref="WfcSolveResult"/> containing either a solution or failure details.</returns>
    public WfcSolveResult Solve()
    {
        var steps = 0;

        var initialPropagationOk = PropagateAllCells();
        if (!initialPropagationOk)
        {
            var failureCell = FindContradictionCellIndex();
            var (x, y) = failureCell >= 0 ? ToCoord(failureCell) : (x: (int?)null, y: (int?)null);
            var failure = new WfcFailure(
                WfcFailureReason.Contradiction,
                failureCell >= 0
                    ? $"Contradiction encountered during initial propagation at cell ({x},{y})."
                    : "Contradiction encountered during initial propagation.",
                x,
                y);
            return new WfcSolveResult(solution: null, failure, steps, _log);
        }

        while (true)
        {
            var (cellIndex, possibleCount) = FindLowestEntropyCell();
            if (cellIndex < 0)
            {
                var solution = BuildSolution();
                _log.Add("Solved.");
                return new WfcSolveResult(solution, failure: null, steps, _log);
            }

            if (steps >= _options.MaxSteps)
            {
                var failure = new WfcFailure(WfcFailureReason.MaxStepsExceeded, $"Max steps ({_options.MaxSteps}) exceeded.");
                return new WfcSolveResult(solution: null, failure, steps, _log);
            }

            steps++;
            _log.Add($"Step {steps}: collapsing cell {ToCoordString(cellIndex)} (entropy={possibleCount}).");

            var collapseOk = CollapseCell(cellIndex);
            if (!collapseOk)
            {
                var (x, y) = ToCoord(cellIndex);
                var failure = new WfcFailure(WfcFailureReason.Contradiction, "Contradiction encountered while collapsing a cell.", x, y);
                return new WfcSolveResult(solution: null, failure, steps, _log);
            }

            var propagateOk = PropagateFrom(cellIndex);
            if (!propagateOk)
            {
                var failureCell = FindContradictionCellIndex();
                var (x, y) = failureCell >= 0 ? ToCoord(failureCell) : (x: (int?)null, y: (int?)null);
                var failure = new WfcFailure(
                    WfcFailureReason.Contradiction,
                    failureCell >= 0
                        ? $"Contradiction encountered during propagation at cell ({x},{y})."
                        : "Contradiction encountered during propagation.",
                    x,
                    y);
                return new WfcSolveResult(solution: null, failure, steps, _log);
            }
        }
    }

    private static HashSet<int> CreateFullSet(int count)
    {
        var set = new HashSet<int>();
        for (var i = 0; i < count; i++)
        {
            set.Add(i);
        }
        return set;
    }

    private (int cellIndex, int possibleCount) FindLowestEntropyCell()
    {
        var bestCellIndex = -1;
        var bestCount = int.MaxValue;

        for (var i = 0; i < _possibleByCell.Length; i++)
        {
            var count = _possibleByCell[i].Count;
            if (count <= 1)
            {
                continue;
            }

            if (count < bestCount)
            {
                bestCount = count;
                bestCellIndex = i;
                continue;
            }

            if (count == bestCount && bestCellIndex >= 0 && _random.Next(0, 2) == 0)
            {
                bestCellIndex = i;
            }
        }

        return (bestCellIndex, bestCellIndex >= 0 ? bestCount : 0);
    }

    private bool CollapseCell(int cellIndex)
    {
        var possible = _possibleByCell[cellIndex];
        if (possible.Count == 0)
        {
            return false;
        }

        if (possible.Count == 1)
        {
            return true;
        }

        var chosen = ChooseWeightedRandom(possible);
        possible.Clear();
        possible.Add(chosen);
        return true;
    }

    private int ChooseWeightedRandom(HashSet<int> possible)
    {
        var ordered = possible.OrderBy(i => i).ToArray();
        double totalWeight = 0;
        foreach (var index in ordered)
        {
            totalWeight += _problem.Tiles[index].Weight;
        }

        var roll = _random.NextDouble() * totalWeight;
        foreach (var index in ordered)
        {
            roll -= _problem.Tiles[index].Weight;
            if (roll <= 0)
            {
                return index;
            }
        }

        return ordered[0];
    }

    private bool PropagateFrom(int startCellIndex)
    {
        var queue = new Queue<int>();
        queue.Enqueue(startCellIndex);

        while (queue.Count > 0)
        {
            var cellIndex = queue.Dequeue();
            var (x, y) = ToCoord(cellIndex);

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = dir.ToOffset();
                var nx = x + dx;
                var ny = y + dy;
                if (nx < 0 || nx >= _problem.Width || ny < 0 || ny >= _problem.Height)
                {
                    continue;
                }

                var neighborIndex = ToIndex(nx, ny);
                var changed = ReduceNeighborOptions(cellIndex, neighborIndex, dir);
                if (changed is null)
                {
                    return false;
                }

                if (changed.Value)
                {
                    queue.Enqueue(neighborIndex);
                }
            }
        }

        return true;
    }

    private bool PropagateAllCells()
    {
        var queue = new Queue<int>();
        for (var i = 0; i < _possibleByCell.Length; i++)
        {
            queue.Enqueue(i);
        }

        while (queue.Count > 0)
        {
            var cellIndex = queue.Dequeue();
            var (x, y) = ToCoord(cellIndex);

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                var (dx, dy) = dir.ToOffset();
                var nx = x + dx;
                var ny = y + dy;
                if (nx < 0 || nx >= _problem.Width || ny < 0 || ny >= _problem.Height)
                {
                    continue;
                }

                var neighborIndex = ToIndex(nx, ny);
                var changed = ReduceNeighborOptions(cellIndex, neighborIndex, dir);
                if (changed is null)
                {
                    return false;
                }

                if (changed.Value)
                {
                    queue.Enqueue(neighborIndex);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Reduces the neighbor's possible set based on the current cell's possibilities.
    /// </summary>
    /// <param name="cellIndex">The current cell.</param>
    /// <param name="neighborIndex">The neighbor cell index.</param>
    /// <param name="directionToNeighbor">The direction from the current cell to the neighbor.</param>
    /// <returns>
    /// <see langword="true"/> when neighbor possibilities changed, <see langword="false"/> when unchanged,
    /// or <see langword="null"/> when the neighbor becomes contradictory (empty set).
    /// </returns>
    private bool? ReduceNeighborOptions(int cellIndex, int neighborIndex, Direction directionToNeighbor)
    {
        var cellPossible = _possibleByCell[cellIndex];
        var neighborPossible = _possibleByCell[neighborIndex];

        if (cellPossible.Count == 0 || neighborPossible.Count == 0)
        {
            return null;
        }

        var reduced = false;
        var toRemove = new List<int>();

        foreach (var neighborTileIndex in neighborPossible)
        {
            var compatible = false;
            foreach (var cellTileIndex in cellPossible)
            {
                if (AreCompatible(cellTileIndex, neighborTileIndex, directionToNeighbor))
                {
                    compatible = true;
                    break;
                }
            }

            if (!compatible)
            {
                toRemove.Add(neighborTileIndex);
            }
        }

        if (toRemove.Count == 0)
        {
            return false;
        }

        foreach (var idx in toRemove)
        {
            neighborPossible.Remove(idx);
            reduced = true;
        }

        if (neighborPossible.Count == 0)
        {
            return null;
        }

        if (reduced)
        {
            _log.Add($"Reduced cell {ToCoordString(neighborIndex)} -> {neighborPossible.Count} possibilities.");
        }

        return reduced;
    }

    private bool AreCompatible(int cellTileIndex, int neighborTileIndex, Direction directionToNeighbor)
    {
        var a = _problem.Tiles[cellTileIndex];
        var b = _problem.Tiles[neighborTileIndex];

        var aEdge = a.GetEdge(directionToNeighbor);
        var bEdge = b.GetEdge(directionToNeighbor.Opposite());
        return aEdge.Matches(bEdge);
    }

    private WfcSolution BuildSolution()
    {
        var tileIndices = new int[_possibleByCell.Length];
        for (var i = 0; i < _possibleByCell.Length; i++)
        {
            var possible = _possibleByCell[i];
            if (possible.Count != 1)
            {
                throw new InvalidOperationException("Cannot build solution unless all cells are collapsed.");
            }

            tileIndices[i] = possible.Single();
        }

        return new WfcSolution(_problem.Width, _problem.Height, tileIndices);
    }

    private int FindContradictionCellIndex()
    {
        for (var i = 0; i < _possibleByCell.Length; i++)
        {
            if (_possibleByCell[i].Count == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private int ToIndex(int x, int y) => (y * _problem.Width) + x;

    private (int x, int y) ToCoord(int cellIndex)
    {
        var x = cellIndex % _problem.Width;
        var y = cellIndex / _problem.Width;
        return (x, y);
    }

    private string ToCoordString(int cellIndex)
    {
        var (x, y) = ToCoord(cellIndex);
        return $"({x},{y})";
    }
}
