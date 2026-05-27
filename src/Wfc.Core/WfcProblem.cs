namespace Wfc.Core;

/// <summary>
/// Defines an input problem instance for Wave Function Collapse.
/// </summary>
public sealed class WfcProblem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WfcProblem"/> class.
    /// </summary>
    /// <param name="width">The grid width in cells.</param>
    /// <param name="height">The grid height in cells.</param>
    /// <param name="tiles">The available tileset.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> or <paramref name="height"/> is not positive.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tiles"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tiles"/> is empty.</exception>
    public WfcProblem(int width, int height, IReadOnlyList<Tile> tiles)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");
        }

        Tiles = tiles ?? throw new ArgumentNullException(nameof(tiles));
        if (Tiles.Count == 0)
        {
            throw new ArgumentException("Tileset must contain at least one tile.", nameof(tiles));
        }

        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the grid width in cells.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the grid height in cells.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the available tileset.
    /// </summary>
    public IReadOnlyList<Tile> Tiles { get; }
}

