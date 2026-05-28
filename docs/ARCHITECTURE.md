# Architecture

## Goals

- Keep the WFC solver logic portable and testable outside of Rhino/Grasshopper.
- Keep Grasshopper-specific code focused on UI, data marshaling, and geometry placement.
- Make solver runs deterministic given a seed.

## Repository Structure

- `src/Wfc.Core/`
  - Portable Wave Function Collapse solver and core types.
  - No Rhino/Grasshopper dependencies.
  - Public API is XML-documented for IntelliSense and generated docs.
- `src/Wfc.Core.Tests/`
  - xUnit tests validating determinism and failure modes.
- `WFC/`
  - Grasshopper plugin layer (RhinoCommon/Grasshopper SDK).
  - Responsible for creating modules from meshes and running the grid solve.

## Wfc.Core Model

### Tiles and Edges

`Wfc.Core.Tile` represents a single tile/module with four `Wfc.Core.Edge` connectors (N, E, S, W) and an optional weight.

An `Edge` is a `(Name, Polarity)` pair:

- `Name`: connector identifier (e.g., `"Road"`, `"Wall"`, `"A"`).
- `Polarity`: `Neutral`, `Positive`, or `Negative`.

Matching rules:

- Names must match (`Ordinal` comparison).
- `Neutral` only matches `Neutral`.
- `Positive` matches `Negative` and vice versa.

### Solver

`Wfc.Core.WfcSolver` solves `Wfc.Core.WfcProblem` using:

1. **Initial propagation** across all cells.
2. Repeating:
   - Select cell with **lowest entropy** (fewest remaining possibilities, > 1).
   - Collapse that cell to one tile using weighted random selection.
   - Propagate constraints outward until stable.

The solver returns a `WfcSolveResult` that contains either:

- `Solution` (`WfcSolution`), or
- `Failure` (`WfcFailure`) with a `WfcFailureReason`.

## Grasshopper Layer

The Grasshopper layer currently keeps a Rhino/mesh-centric `WFC.Module` representation and uses a WFC-style propagation approach to solve a `WFC.Grid`.

The long-term plan is to use `Wfc.Core` as the source of truth for solving, and treat the Grasshopper layer as an adapter:

- Convert Grasshopper `Module` + edges → `Wfc.Core.Tile`.
- Solve using `Wfc.Core.WfcSolver`.
- Convert solution tile indices back into placed `Rhino.Geometry.Mesh` instances.

