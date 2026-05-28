using System.Diagnostics;

namespace Wfc.Core;

/// <summary>
/// Describes a connector on a tile edge used to determine adjacency compatibility.
/// </summary>
[DebuggerDisplay("{Name,nq}:{Polarity}")]
public readonly struct Edge : IEquatable<Edge>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Edge"/> struct.
    /// </summary>
    /// <param name="name">A connector name (for example <c>A</c>, <c>Road</c>, <c>Wall</c>).</param>
    /// <param name="polarity">
    /// The connector polarity. A positive connector matches a negative connector of the same name, and vice-versa.
    /// Neutral connectors only match other neutral connectors of the same name.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
    public Edge(string name, EdgePolarity polarity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Edge name must be non-empty.", nameof(name));
        }

        Name = name;
        Polarity = polarity;
    }

    /// <summary>
    /// Gets the connector name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the connector polarity.
    /// </summary>
    public EdgePolarity Polarity { get; }

    /// <summary>
    /// Determines whether this connector can connect to <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The opposing connector (i.e. the connector on the neighbor tile edge that faces this edge).</param>
    /// <returns><see langword="true"/> when the connectors are compatible; otherwise <see langword="false"/>.</returns>
    public bool Matches(Edge other)
    {
        if (!string.Equals(Name, other.Name, StringComparison.Ordinal))
        {
            return false;
        }

        if (Polarity == EdgePolarity.Neutral || other.Polarity == EdgePolarity.Neutral)
        {
            return Polarity == EdgePolarity.Neutral && other.Polarity == EdgePolarity.Neutral;
        }

        return (Polarity == EdgePolarity.Positive && other.Polarity == EdgePolarity.Negative)
            || (Polarity == EdgePolarity.Negative && other.Polarity == EdgePolarity.Positive);
    }

    /// <summary>
    /// Creates a new edge with the same <see cref="Name"/> but inverted <see cref="Polarity"/>.
    /// </summary>
    /// <returns>The inverted edge.</returns>
    public Edge Invert()
    {
        return Polarity switch
        {
            EdgePolarity.Neutral => this,
            EdgePolarity.Positive => new Edge(Name, EdgePolarity.Negative),
            EdgePolarity.Negative => new Edge(Name, EdgePolarity.Positive),
            _ => this,
        };
    }

    /// <inheritdoc />
    public override string ToString() => $"{Name}:{Polarity}";

    /// <inheritdoc />
    public bool Equals(Edge other)
        => string.Equals(Name, other.Name, StringComparison.Ordinal) && Polarity == other.Polarity;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Edge other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(Name);
            hash = (hash * 31) + (int)Polarity;
            return hash;
        }
    }

    /// <summary>
    /// Tests two edges for equality.
    /// </summary>
    /// <param name="left">The left edge.</param>
    /// <param name="right">The right edge.</param>
    /// <returns><see langword="true"/> when equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Edge left, Edge right) => left.Equals(right);

    /// <summary>
    /// Tests two edges for inequality.
    /// </summary>
    /// <param name="left">The left edge.</param>
    /// <param name="right">The right edge.</param>
    /// <returns><see langword="true"/> when not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Edge left, Edge right) => !left.Equals(right);
}
