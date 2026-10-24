# Sudoku Console Game

This solution contains a simple console-based Sudoku game built with C# and .NET. The program generates a new puzzle at startup, displays the board in the terminal, and allows the player to place numbers, clear cells, request hints, and check progress.

## Assumptions

- The game starts with a valid Sudoku board that has been solved and then partially emptied.
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

- A random valid Sudoku solution is generated before play begins.
- A fixed number of cells are removed from that solved board to create the puzzle.
- The program marks original clues distinctly from user-entered values.
- User-entered cells cannot overwrite pre-filled clue values.
- Invalid moves are rejected, and the game can provide a hint to fill the next empty cell.

## Environment

This project runs in a .NET console environment and is intended for local execution from a terminal. It targets the .NET 10 framework and does not use a graphical user interface (GUI). It is suitable for running in:

- Windows development environments (including Visual Studio)
- Command-line environments with the .NET SDK installed
- Other supported .NET 10 operating systems when the SDK is available

## Notes

The project is intentionally lightweight and self-contained, making it a good example of a console-based game implementation in C#.
