namespace Wfc.Core;

/// <summary>
/// Controls the behavior of the Wave Function Collapse solver.
/// </summary>
public sealed class WfcOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WfcOptions"/> class.
    /// </summary>
    /// <param name="seed">The random seed used by the solver.</param>
    /// <param name="maxSteps">The maximum number of collapse steps to attempt.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxSteps"/> is not positive.</exception>
    public WfcOptions(int seed = 0, int maxSteps = 10_000)
    {
        if (maxSteps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSteps), maxSteps, "Max steps must be positive.");
        }

        Seed = seed;
        MaxSteps = maxSteps;
    }

    /// <summary>
    /// Gets the random seed used by the solver.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the maximum number of collapse steps to attempt.
    /// </summary>
    public int MaxSteps { get; }
}

