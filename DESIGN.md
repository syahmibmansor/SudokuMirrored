# Design Overview

This solution implements a command-line Sudoku game in C# using a small, testable architecture with a single core board model and a thin console entry point.

## Solution structure

- `Sudoku/SudokuBoard.cs`
  - Contains the core game-state logic, puzzle generation, validation, hinting, and board rendering.
  - Maintains the master solution, current board state, and immutable clue tracking.

- `Sudoku/Program.cs`
  - Hosts the console loop and command processing.
  - Reads user input, renders the board and history, and orchestrates game flow.

- `Sudoku/CommandParser.cs`
  - Parses raw commands such as `A3 4`, `C5 clear`, `hint`, `check`, and `exit`.
  - Normalizes case and whitespace so the CLI is resilient to user input variation.

- `Sudoku.Tests/*`
  - xUnit tests cover board logic, parser behavior, validation messages, hinting, and lifecycle flows.

## Design intent

The design keeps responsibilities separated:

- `SudokuBoard` is responsible for the game rules and data model.
- `CommandParser` is responsible for parsing input.
- `Program` is responsible for user interaction and flow orchestration.

This keeps the game logic independent from the console shell and makes the behavior easier to test.

## Object-oriented design and SOLID alignment

### Single responsibility
Each major component owns one concern:
- board logic stays inside `SudokuBoard`
- command parsing stays in `CommandParser`
- terminal flow stays in `Program`

### Encapsulation
Board internals such as `initialClues`, `masterSolution`, and `highlightedCell` are hidden behind methods and public behavior.

### Small, focused behavior
Methods are designed to do one thing, such as validating a move, generating a puzzle, checking rules, or providing a hint. This makes it easier to reason about edge cases and maintain the project.

### Testability
The game loop and board model were intentionally structured so they can be exercised via xUnit without requiring a live terminal session. This supports robust automated testing and keeps complex command-flow rules under validation.

## Testing strategy

The project uses xUnit as the unit test framework.

Coverage areas include:
- startup clue count and board validity
- command parsing and malformed input handling
- row/column/subgrid validation
- clue protection and overwrite safety
- hint priority and near-complete board behavior
- game completion and exit flow

This gives good confidence in both business rules and the interactive console experience.

## Quality and analysis

The repo is configured to integrate with SonarQube/SonarCloud in Azure Pipelines, with test coverage collected and passed to the analysis pipeline.

This helps keep the solution aligned with quality gates and supports detection of:
- dead or untested code paths
- maintainability issues
- complex logic hotspots
- regressions in future changes

## Documentation consistency

The project uses XML documentation comments on public members where they provide value, especially on the board model and key game behaviors. This improves readability and keeps the codebase easy to maintain without over-commenting every internal detail.
