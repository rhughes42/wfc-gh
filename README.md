# WFC (Grasshopper) + Wfc.Core

Wave Function Collapse (WFC) tools intended for Grasshopper/Rhino, plus a portable core solver library.

This repo currently contains:

- `WFC/`: a Grasshopper plugin codebase (RhinoCommon/Grasshopper SDK required to build).
- `src/Wfc.Core/`: a buildable, tested, fully XML-documented WFC core library (no Rhino dependencies).
- `src/Wfc.Core.Tests/`: xUnit tests for the core solver.

See `docs/ARCHITECTURE.md` for solver details and `docs/ROADMAP.md` for planned work.

## Build and Test (Core)

Run:

`dotnet test Wfc.Core.slnx`

## Grasshopper Plugin (WFC/)

The Grasshopper project references RhinoCommon/Grasshopper assemblies from a Rhino installation (Windows).

Typical workflow:

1. Install Rhino + Grasshopper.
2. Open `WFC.sln` in Visual Studio.
3. Fix/confirm reference paths to `RhinoCommon.dll`, `Grasshopper.dll`, `GH_IO.dll` in `WFC/WFC.csproj`.
4. Build the project to produce a `.gha`.

## Grasshopper Usage (High Level)

1. **Create Edges**: define connectors (name + type).
2. **Create Module**: combine mesh geometry with 4 edges (N, E, S, W).
3. **Augment Modules**: optionally generate rotated/reflected variants.
4. **Create Grid**: define grid size and spacing.
5. **Compute**: run the solver (seed + max steps), returning meshes and a log.
