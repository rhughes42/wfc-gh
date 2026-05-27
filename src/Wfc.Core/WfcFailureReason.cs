namespace Wfc.Core;

/// <summary>
/// Indicates why a solve attempt failed.
/// </summary>
public enum WfcFailureReason
{
    /// <summary>
    /// A contradiction was encountered (a cell ended up with zero possible tiles).
    /// </summary>
    Contradiction = 0,

    /// <summary>
    /// The solver exceeded <see cref="WfcOptions.MaxSteps"/> without fully collapsing the grid.
    /// </summary>
    MaxStepsExceeded = 1,
}

