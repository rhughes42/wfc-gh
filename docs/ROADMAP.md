# Roadmap / Further Development

This repository started as a minimal Grasshopper proof-of-concept and is being refactored into a maintainable core + adapter architecture.

## Near-Term

- Integrate `Wfc.Core` into `WFC/` so Grasshopper uses the same solver and behavior as the tested core library.
- Move `WFC/` project to a modern .NET target aligned with Rhino/Grasshopper versions (Rhino 7 .NET Framework 4.8 / Rhino 8 .NET).
- Add Grasshopper-facing UX improvements:
  - Clearer component names/descriptions
  - Better runtime validation and error messaging
  - Optional progress reporting / early stop
- Add examples (sample tilesets + GH definitions) and screenshots/gifs.

## Solver Features

- Support for **fixed constraints** (pin specific cells to specific tiles).
- Better contradiction handling:
  - backtracking / retry strategies
  - restarts with different seeds
- Weighted / learned frequency support from input examples.
- Performance improvements (bitsets, precomputed compatibility tables).
- Optional 3D grids (requires a generalized direction set and adjacency model).

## Build / Release

- Add CI to run `dotnet test Wfc.Core.slnx`.
- Provide a release pipeline for shipping a `.gha` artifact.
- Optionally publish `Wfc.Core` as a NuGet package for reuse.

