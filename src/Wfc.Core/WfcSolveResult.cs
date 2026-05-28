namespace Wfc.Core;

/// <summary>
/// Represents the outcome of attempting to solve a WFC problem.
/// </summary>
public sealed class WfcSolveResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WfcSolveResult"/> class.
    /// </summary>
    /// <param name="solution">The resulting solution when successful.</param>
    /// <param name="failure">The failure when unsuccessful.</param>
    /// <param name="steps">The number of collapse steps performed.</param>
    /// <param name="log">A diagnostic log produced during solving.</param>
    /// <exception cref="ArgumentException">Thrown when both <paramref name="solution"/> and <paramref name="failure"/> are provided, or when neither are provided.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="steps"/> is negative.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="log"/> is null.</exception>
    public WfcSolveResult(WfcSolution? solution, WfcFailure? failure, int steps, IReadOnlyList<string> log)
    {
        if ((solution is null) == (failure is null))
        {
            throw new ArgumentException("Result must contain either a solution or a failure, but not both.");
        }

        if (steps < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(steps), steps, "Steps cannot be negative.");
        }

        Solution = solution;
        Failure = failure;
        Steps = steps;
        Log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>
    /// Gets a value indicating whether the solve succeeded.
    /// </summary>
    public bool Succeeded => Solution is not null;

    /// <summary>
    /// Gets the computed solution when <see cref="Succeeded"/> is <see langword="true"/>.
    /// </summary>
    public WfcSolution? Solution { get; }

    /// <summary>
    /// Gets the failure details when <see cref="Succeeded"/> is <see langword="false"/>.
    /// </summary>
    public WfcFailure? Failure { get; }

    /// <summary>
    /// Gets the number of collapse steps performed.
    /// </summary>
    public int Steps { get; }

    /// <summary>
    /// Gets the diagnostic log produced during solving.
    /// </summary>
    public IReadOnlyList<string> Log { get; }
}

