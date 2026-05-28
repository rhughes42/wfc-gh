namespace Wfc.Core;

/// <summary>
/// Represents the four cardinal directions used by the 2D Wave Function Collapse grid.
/// </summary>
public enum Direction
{
    /// <summary>
    /// The direction pointing toward decreasing <c>Y</c> (row - 1).
    /// </summary>
    North = 0,

    /// <summary>
    /// The direction pointing toward increasing <c>X</c> (column + 1).
    /// </summary>
    East = 1,

    /// <summary>
    /// The direction pointing toward increasing <c>Y</c> (row + 1).
    /// </summary>
    South = 2,

    /// <summary>
    /// The direction pointing toward decreasing <c>X</c> (column - 1).
    /// </summary>
    West = 3,
}

/// <summary>
/// Direction helpers.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Gets the opposite direction.
    /// </summary>
    /// <param name="direction">The direction to invert.</param>
    /// <returns>The opposite direction.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="direction"/> is not a valid <see cref="Direction"/> value.</exception>
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction."),
        };
    }

    /// <summary>
    /// Gets the <c>(dx, dy)</c> offset for moving one cell in the given direction.
    /// </summary>
    /// <param name="direction">The direction of travel.</param>
    /// <returns>A tuple containing <c>dx</c> and <c>dy</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="direction"/> is not a valid <see cref="Direction"/> value.</exception>
    public static (int dx, int dy) ToOffset(this Direction direction)
    {
        return direction switch
        {
            Direction.North => (0, -1),
            Direction.East => (1, 0),
            Direction.South => (0, 1),
            Direction.West => (-1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction."),
        };
    }
}

