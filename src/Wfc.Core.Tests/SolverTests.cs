using Wfc.Core;
using Xunit;

namespace Wfc.Core.Tests;

public sealed class SolverTests
{
    [Fact]
    public void Solve_IsDeterministic_ForSameSeed()
    {
        var tiles = new[]
        {
            new Tile(
                id: "AllA",
                north: new Edge("A", EdgePolarity.Neutral),
                east: new Edge("A", EdgePolarity.Neutral),
                south: new Edge("A", EdgePolarity.Neutral),
                west: new Edge("A", EdgePolarity.Neutral),
                weight: 1.0),
            new Tile(
                id: "AllA_2",
                north: new Edge("A", EdgePolarity.Neutral),
                east: new Edge("A", EdgePolarity.Neutral),
                south: new Edge("A", EdgePolarity.Neutral),
                west: new Edge("A", EdgePolarity.Neutral),
                weight: 1.0),
        };

        var problem = new WfcProblem(width: 4, height: 3, tiles);
        var options = new WfcOptions(seed: 123, maxSteps: 500);

        var first = new WfcSolver(problem, options).Solve();
        var second = new WfcSolver(problem, options).Solve();

        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(first.Solution!.GetTileIndices(), second.Solution!.GetTileIndices());
    }

    [Fact]
    public void Solve_Fails_WithContradiction_WhenNoTileCanMatchItself()
    {
        var tiles = new[]
        {
            new Tile(
                id: "AlwaysPositive",
                north: new Edge("A", EdgePolarity.Positive),
                east: new Edge("A", EdgePolarity.Positive),
                south: new Edge("A", EdgePolarity.Positive),
                west: new Edge("A", EdgePolarity.Positive),
                weight: 1.0),
        };

        var problem = new WfcProblem(width: 2, height: 1, tiles);
        var options = new WfcOptions(seed: 0, maxSteps: 50);

        var result = new WfcSolver(problem, options).Solve();

        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal(WfcFailureReason.Contradiction, result.Failure!.Reason);
    }

    [Fact]
    public void Solve_Fails_WithMaxStepsExceeded_WhenMaxStepsTooLow()
    {
        var tiles = new[]
        {
            new Tile(
                id: "AllA_1",
                north: new Edge("A", EdgePolarity.Neutral),
                east: new Edge("A", EdgePolarity.Neutral),
                south: new Edge("A", EdgePolarity.Neutral),
                west: new Edge("A", EdgePolarity.Neutral),
                weight: 1.0),
            new Tile(
                id: "AllA_2",
                north: new Edge("A", EdgePolarity.Neutral),
                east: new Edge("A", EdgePolarity.Neutral),
                south: new Edge("A", EdgePolarity.Neutral),
                west: new Edge("A", EdgePolarity.Neutral),
                weight: 1.0),
        };

        var problem = new WfcProblem(width: 2, height: 2, tiles);
        var options = new WfcOptions(seed: 42, maxSteps: 1);

        var result = new WfcSolver(problem, options).Solve();

        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal(WfcFailureReason.MaxStepsExceeded, result.Failure!.Reason);
        Assert.Equal(1, result.Steps);
    }
}
