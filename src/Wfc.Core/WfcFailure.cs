namespace Wfc.Core;

/// <summary>
/// Describes a failure encountered while solving a WFC problem.
/// </summary>
public sealed class WfcFailure
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WfcFailure"/> class.
    /// </summary>
    /// <param name="reason">The failure reason.</param>
    /// <param name="message">A human-readable description.</param>
    /// <param name="x">The optional cell X coordinate related to the failure.</param>
    /// <param name="y">The optional cell Y coordinate related to the failure.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is null, empty, or whitespace.</exception>
    public WfcFailure(WfcFailureReason reason, string message, int? x = null, int? y = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Failure message must be non-empty.", nameof(message));
        }

        Reason = reason;
        Message = message;
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the failure reason.
    /// </summary>
    public WfcFailureReason Reason { get; }

    /// <summary>
    /// Gets a human-readable description of the failure.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the X coordinate of the cell related to this failure, when available.
    /// </summary>
    public int? X { get; }

    /// <summary>
    /// Gets the Y coordinate of the cell related to this failure, when available.
    /// </summary>
    public int? Y { get; }
}

