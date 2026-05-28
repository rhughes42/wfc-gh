namespace Wfc.Core;

/// <summary>
/// Represents a completed WFC solution as a grid of tile indices.
/// </summary>
public sealed class WfcSolution
{
    private readonly int[] _tileIndices;

    /// <summary>
    /// Initializes a new instance of the <see cref="WfcSolution"/> class.
    /// </summary>
    /// <param name="width">The grid width in cells.</param>
    /// <param name="height">The grid height in cells.</param>
    /// <param name="tileIndices">
    /// The chosen tile indices in row-major order (<c>index = y * width + x</c>).
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> or <paramref name="height"/> is not positive.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tileIndices"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tileIndices"/> length does not match <paramref name="width"/> * <paramref name="height"/>.</exception>
    public WfcSolution(int width, int height, int[] tileIndices)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");
        }

        _tileIndices = tileIndices ?? throw new ArgumentNullException(nameof(tileIndices));
        if (_tileIndices.Length != checked(width * height))
        {
            throw new ArgumentException("Tile index array must match grid size.", nameof(tileIndices));
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
    /// Gets the selected tile index at a specific coordinate.
    /// </summary>
    /// <param name="x">The X coordinate (0-based).</param>
    /// <param name="y">The Y coordinate (0-based).</param>
    /// <returns>The selected tile index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="x"/> or <paramref name="y"/> is outside the grid bounds.</exception>
    public int GetTileIndex(int x, int y)
    {
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x), x, "X must be within grid bounds.");
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be within grid bounds.");
        }

        return _tileIndices[(y * Width) + x];
    }

    /// <summary>
    /// Gets a copy of the solution tile indices in row-major order (<c>index = y * width + x</c>).
    /// </summary>
    /// <returns>A copy of the tile index array.</returns>
    public int[] GetTileIndices() => (int[])_tileIndices.Clone();
}

