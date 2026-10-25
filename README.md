# Sudoku Console Game

This solution contains a simple console-based Sudoku game built with C# and .NET. The program generates a new puzzle at startup, displays the board in the terminal, and allows the player to place numbers, clear cells, request hints, and check progress.

## Assumptions

- The game starts with a valid Sudoku board that has been solved and then partially emptied.
- A valid, well-formed Sudoku puzzle has exactly one unique solution.
- The starting board contains pre-filled random numbers in random positions.
- The player is expected to fill the remaining empty cells while preserving Sudoku rules.
- The board uses a standard 9x9 layout with rows labeled A through I and columns numbered 1 through 9.
- The application is designed for a single user playing in a terminal/console session.

## How to run the game

### Prerequisites

- .NET SDK 10.0 or later
- A terminal or command prompt
- Access to the project files in this solution

### Run from the command line

1. Open a terminal in the solution root.
2. Restore/build the project:

   dotnet build

3. Start the game:

   dotnet run --project Sudoku/Sudoku.csproj

### Available commands

- Insert a value: `A3 4`
- Clear a cell: `C5 clear`
- Request a hint: `hint`
- Check progress: `check`
- Exit the game: `exit`

## Game behavior

- A random valid Sudoku solution is generated before play begins and stored as the master board.
- A fixed number of cells are removed from that solved board to create the puzzle.
- After removing clues, the puzzle is validated to ensure it still has one and only one valid solution.
- The program marks original clues distinctly from user-entered values.
- User-entered cells cannot overwrite pre-filled clue values.
- Invalid moves are rejected, and the game can provide a hint to fill the next empty or incorrect cell.
- Hint behavior: 
-- The game compares the current board to the master solution and logs the correct value for the selected cell without changing the board.
-- Hint will prioritize checking the user input first and show if reveal the correct value.
-- If no user input is incorrect or if there is no user input, then the correct value will be revealed for a random empty cell instead.
-- Rewards Accuracy: If a player is playing carefully and hasn't made any mistakes, they get rewarded with a helpful new hint to push them forward.
-- Saves Players from Dead Ends: If they've quietly drifted onto a wrong path without breaking immediate rules, the hint catches the root cause of their future deadlock immediately.

## Environment

This project runs in a .NET console environment and is intended for local execution from a terminal. It targets the .NET 10 framework and does not use a graphical user interface (GUI). It is suitable for running in:

- Windows development environments (including Visual Studio)
- Command-line environments with the .NET SDK installed
- Other supported .NET 10 operating systems when the SDK is available

## Testing

This solution uses xUnit for automated validation and regression coverage. The test project is located in `Sudoku.Tests` and validates both the core board logic and the user command flow.

For the categorized testing breakdown, see `TESTING.md`.

### Test execution

Run the suite from the repository root:

```bash
dotnet test
```