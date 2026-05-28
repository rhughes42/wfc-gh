using System.Diagnostics;

namespace Wfc.Core;

/// <summary>
/// Represents a single tile/module in a tileset, including its edge connectors and optional weight.
/// </summary>
[DebuggerDisplay("{Id,nq}")]
public sealed class Tile
{
    private readonly Edge[] _edges;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tile"/> class.
    /// </summary>
    /// <param name="id">A stable tile identifier used for logging/diagnostics.</param>
    /// <param name="north">The edge connector on the north side.</param>
    /// <param name="east">The edge connector on the east side.</param>
    /// <param name="south">The edge connector on the south side.</param>
    /// <param name="west">The edge connector on the west side.</param>
    /// <param name="weight">
    /// A relative weight used when randomly selecting among possible tiles during collapse.
    /// Higher values make the tile more likely to be selected.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="weight"/> is not positive.</exception>
    public Tile(string id, Edge north, Edge east, Edge south, Edge west, double weight = 1.0)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Tile id must be non-empty.", nameof(id));
        }

        if (weight <= 0 || double.IsNaN(weight) || double.IsInfinity(weight))
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Tile weight must be a positive finite number.");
        }

        Id = id;
        Weight = weight;
        _edges = new[] { north, east, south, west };
    }

    /// <summary>
    /// Gets the tile identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the tile weight used during random selection.
    /// </summary>
    public double Weight { get; }

    /// <summary>
    /// Gets a copy of the tile edges in <c>N, E, S, W</c> order.
    /// </summary>
    /// <returns>An array of edges in <c>N, E, S, W</c> order.</returns>
    public Edge[] GetEdges() => (Edge[])_edges.Clone();

    /// <summary>
    /// Gets the edge connector for a given direction.
    /// </summary>
    /// <param name="direction">The direction to query.</param>
    /// <returns>The edge connector on that side of the tile.</returns>
    public Edge GetEdge(Direction direction) => _edges[(int)direction];
}

