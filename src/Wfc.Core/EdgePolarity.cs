namespace Wfc.Core;

/// <summary>
/// Indicates whether an edge connector is neutral or has an orientation (polarity).
/// </summary>
public enum EdgePolarity
{
    /// <summary>
    /// A neutral connector. Neutral connectors only match other neutral connectors of the same name.
    /// </summary>
    Neutral = 0,

    /// <summary>
    /// A positive connector. Positive connectors match negative connectors of the same name.
    /// </summary>
    Positive = 1,

    /// <summary>
    /// A negative connector. Negative connectors match positive connectors of the same name.
    /// </summary>
    Negative = 2,
}

