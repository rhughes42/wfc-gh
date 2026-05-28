using Wfc.Core;
using Xunit;

namespace Wfc.Core.Tests;

public sealed class EdgeTests
{
    [Fact]
    public void Matches_ReturnsTrue_ForNeutralToNeutralSameName()
    {
        var a = new Edge("A", EdgePolarity.Neutral);
        var b = new Edge("A", EdgePolarity.Neutral);

        Assert.True(a.Matches(b));
    }

    [Fact]
    public void Matches_ReturnsFalse_ForNeutralToNonNeutral()
    {
        var a = new Edge("A", EdgePolarity.Neutral);
        var b = new Edge("A", EdgePolarity.Positive);

        Assert.False(a.Matches(b));
        Assert.False(b.Matches(a));
    }

    [Fact]
    public void Matches_ReturnsTrue_ForPositiveToNegativeSameName()
    {
        var a = new Edge("A", EdgePolarity.Positive);
        var b = new Edge("A", EdgePolarity.Negative);

        Assert.True(a.Matches(b));
        Assert.True(b.Matches(a));
    }

    [Fact]
    public void Matches_ReturnsFalse_ForSamePolarity()
    {
        var a = new Edge("A", EdgePolarity.Positive);
        var b = new Edge("A", EdgePolarity.Positive);

        Assert.False(a.Matches(b));
    }
}

