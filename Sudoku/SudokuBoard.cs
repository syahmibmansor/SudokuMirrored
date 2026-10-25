using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    /// <summary>
    /// Represents the in-memory state of a Sudoku game board.
    /// </summary>
    public class SudokuBoard
    {
        private int[,] board = new int[9, 9];
        private readonly bool[,] initialClues = new bool[9, 9];
        private readonly Random rand = new Random();
        private int[,] masterSolution = new int[9, 9];
        private (int row, int col)? highlightedCell;
        private (int row, int col)? lastHintedCell;
        private readonly HashSet<(int row, int col)> hintedCells = [];
        private readonly List<(int row, int col, int value)> userEntries = [];

        /// <summary>
        /// Initializes a new random Sudoku board with a valid puzzle state.
        /// </summary>
        public SudokuBoard()
        {
            GenerateRandomPuzzle();
        }

        private void GenerateRandomPuzzle()
        {
            board = new int[9, 9];
            masterSolution = new int[9, 9];

            if (!GenerateSolvedBoard(board))
            {
                throw new InvalidOperationException("Unable to generate a valid Sudoku solution.");
            }

            masterSolution = (int[,])board.Clone();

            int cellsToRemove = 51; // Leave 30 pre-filled clues.
            var positions = new List<(int row, int col)>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    positions.Add((r, c));
                }
            }

            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                var temp = positions[i];
                positions[i] = positions[j];
                positions[j] = temp;
            }

            foreach (var (row, col) in positions)
            {
                if (cellsToRemove == 0)
                    break;

                int original = board[row, col];
                board[row, col] = 0;

                if (CountSolutions(board) != 1)
                {
                    board[row, col] = original;
                    continue;
                }

                cellsToRemove--;
            }

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    initialClues[r, c] = board[r, c] != 0;
                }
            }
        }

        /// <summary>
        /// Renders the current board to the console.
        /// </summary>
        public void Print()
        {
            var invalidCells = GetInvalidCells();

            Console.WriteLine("    1 2 3   4 5 6   7 8 9");
            Console.WriteLine("  -------------------------");

            for (int r = 0; r < 9; r++)
            {
                char rowLabel = (char)('A' + r);
                Console.Write($"{rowLabel} | ");

                for (int c = 0; c < 9; c++)
                {
                    bool isHighlighted = highlightedCell.HasValue && highlightedCell.Value.row == r && highlightedCell.Value.col == c;
                    bool isInvalid = invalidCells.Contains((r, c));

                    if (isHighlighted)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }

                    if (board[r, c] == 0)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        if (isInvalid)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (initialClues[r, c])
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }

                        Console.Write(board[r, c]);
                        Console.Write(' ');
                    }

                    Console.ResetColor();

                    if ((c + 1) % 3 == 0)
                    {
                        Console.Write("| ");
                    }
                }

                Console.WriteLine();
                if ((r + 1) % 3 == 0)
                {
                    Console.WriteLine("  -------------------------");
                }
            }
        }

        /// <summary>
        /// Clears the active highlighted hint cell.
        /// </summary>
        public void ClearHighlight()
        {
            highlightedCell = null;
        }

        /// <summary>
        /// Attempts to place a value in the grid when the position is editable.
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="col">The zero-based column index.</param>
        /// <param name="num">The number to place.</param>
        /// <returns>True when the move is accepted; otherwise, false.</returns>
        public bool TryMakeMove(int row, int col, int num)
        {
            if (row < 0 || row >= 9 || col < 0 || col >= 9)
                return false;

            if (initialClues[row, col])
                return false;

            if (num < 1 || num > 9)
                return false;

            board[row, col] = num;
            highlightedCell = null;
            lastHintedCell = null;
            hintedCells.Clear();
            userEntries.RemoveAll(entry => entry.row == row && entry.col == col);
            userEntries.Add((row, col, num));
            return true;
        }

        /// <summary>
        /// Clears a non-clue cell.
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="col">The zero-based column index.</param>
        /// <returns>True when the cell was cleared; otherwise, false.</returns>
        public bool TryClear(int row, int col)
        {
            if (initialClues[row, col]) return false;

            board[row, col] = 0;
            highlightedCell = null;
            lastHintedCell = null;
            hintedCells.Clear();
            userEntries.RemoveAll(entry => entry.row == row && entry.col == col);
            return true;
        }

        /// <summary>
        /// Determines whether every cell has been filled.
        /// </summary>
        /// <returns>True when the board is completely filled; otherwise, false.</returns>
        public bool IsBoardComplete()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the board violates Sudoku row, column, or subgrid rules.
        /// </summary>
        /// <returns>True when a rule violation exists; otherwise, false.</returns>
        public bool HasRuleViolation()
        {
            return GetFirstViolationMessage() != null;
        }

        /// <summary>
        /// Validates the current grid and returns the first violation message or a completion message.
        /// </summary>
        /// <returns>A rule violation message or success status.</returns>
        public string CheckBoard()
        {
            string? violation = GetFirstViolationMessage();
            if (violation != null)
            {
                return violation;
            }

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                        return "No rule violations detected.";
                }
            }

            return "You have successfully completed the Sudoku puzzle!";
        }

        private string? GetFirstViolationMessage()
        {
            for (int r = 0; r < 9; r++)
            {
                var seen = new HashSet<int>();
                for (int c = 0; c < 9; c++)
                {
                    int value = board[r, c];
                    if (value == 0) continue;
                    if (!seen.Add(value))
                        return $"Number {value} already exists in Row {GetRowLabel(r)}.";
                }
            }

            for (int c = 0; c < 9; c++)
            {
                var seen = new HashSet<int>();
                for (int r = 0; r < 9; r++)
                {
                    int value = board[r, c];
                    if (value == 0) continue;
                    if (!seen.Add(value))
                        return $"Number {value} already exists in Column {c + 1}.";
                }
            }

            for (int startRow = 0; startRow < 9; startRow += 3)
            {
                for (int startCol = 0; startCol < 9; startCol += 3)
                {
                    var seen = new HashSet<int>();
                    for (int r = startRow; r < startRow + 3; r++)
                    {
                        for (int c = startCol; c < startCol + 3; c++)
                        {
                            int value = board[r, c];
                            if (value == 0) continue;
                            if (!seen.Add(value))
                                return $"Number {value} already exists in the same 3×3 subgrid.";
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the set of coordinates that violate Sudoku rules.
        /// </summary>
        /// <returns>A set of invalid cell positions.</returns>
        public HashSet<(int row, int col)> GetInvalidCells()
        {
            var invalid = new HashSet<(int row, int col)>();

            for (int r = 0; r < 9; r++)
            {
                var seen = new Dictionary<int, List<(int row, int col)>>();
                for (int c = 0; c < 9; c++)
                {
                    int value = board[r, c];
                    if (value == 0) continue;
                    if (!seen.ContainsKey(value))
                        seen[value] = [];
                    seen[value].Add((r, c));
                }

                foreach (var positions in seen.Values)
                {
                    if (positions.Count > 1)
                    {
                        foreach (var position in positions)
                            invalid.Add(position);
                    }
                }
            }

            for (int c = 0; c < 9; c++)
            {
                var seen = new Dictionary<int, List<(int row, int col)>>();
                for (int r = 0; r < 9; r++)
                {
                    int value = board[r, c];
                    if (value == 0) continue;
                    if (!seen.ContainsKey(value))
                        seen[value] = [];
                    seen[value].Add((r, c));
                }

                foreach (var positions in seen.Values)
                {
                    if (positions.Count > 1)
                    {
                        foreach (var position in positions)
                            invalid.Add(position);
                    }
                }
            }

            for (int startRow = 0; startRow < 9; startRow += 3)
            {
                for (int startCol = 0; startCol < 9; startCol += 3)
                {
                    var seen = new Dictionary<int, List<(int row, int col)>>();
                    for (int r = startRow; r < startRow + 3; r++)
                    {
                        for (int c = startCol; c < startCol + 3; c++)
                        {
                            int value = board[r, c];
                            if (value == 0) continue;
                            if (!seen.ContainsKey(value))
                                seen[value] = [];
                            seen[value].Add((r, c));
                        }
                    }

                    foreach (var positions in seen.Values)
                    {
                        if (positions.Count > 1)
                        {
                            foreach (var position in positions)
                                invalid.Add(position);
                        }
                    }
                }
            }

            return invalid;
        }

        /// <summary>
        /// Provides a non-destructive hint based on incorrect user entries or empty cells.
        /// </summary>
        /// <returns>A descriptive hint message.</returns>
        public string GiveHint()
        {
            var wrongUserEntries = new List<(int row, int col)>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] != 0 && !initialClues[r, c] && board[r, c] != masterSolution[r, c])
                    {
                        wrongUserEntries.Add((r, c));
                    }
                }
            }

            var emptyCells = new List<(int row, int col)>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        emptyCells.Add((r, c));
                    }
                }
            }

            var unhintedWrongEntries = wrongUserEntries
                .Where(cell => !hintedCells.Contains(cell))
                .ToList();

            if (unhintedWrongEntries.Count > 0)
            {
                var target = unhintedWrongEntries[rand.Next(unhintedWrongEntries.Count)];
                hintedCells.Add(target);
                highlightedCell = target;
                lastHintedCell = target;
                int correctValue = masterSolution[target.row, target.col];
                char rowLabel = (char)('A' + target.row);
                return $"Hint: Cell {rowLabel}{target.col + 1} should be {correctValue}. The current value is incorrect.";
            }

            var unhintedEmptyCells = emptyCells
                .Where(cell => !hintedCells.Contains(cell))
                .ToList();

            if (unhintedEmptyCells.Count > 0)
            {
                var target = unhintedEmptyCells[rand.Next(unhintedEmptyCells.Count)];
                hintedCells.Add(target);
                highlightedCell = target;
                lastHintedCell = target;
                int correctValue = masterSolution[target.row, target.col];
                char rowLabel = (char)('A' + target.row);
                return $"Hint: Cell {rowLabel}{target.col + 1} should be {correctValue}.";
            }

            if (wrongUserEntries.Count > 0 || emptyCells.Count > 0)
            {
                hintedCells.Clear();
                return GiveHint();
            }

            highlightedCell = null;
            lastHintedCell = null;
            hintedCells.Clear();
            return "Hint: No empty or incorrect cells remain.";
        }

        private static string GetRowLabel(int row)
        {
            return ((char)('A' + row)).ToString();
        }

        private static bool IsValidPlacement(int[,] grid, int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (grid[row, i] == num || grid[i, col] == num)
                    return false;
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (grid[r + startRow, c + startCol] == num)
                        return false;
                }
            }
            return true;
        }

        private bool GenerateSolvedBoard(int[,] grid)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (grid[r, c] == 0)
                    {
                        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        Shuffle(numbers);

                        foreach (int num in numbers)
                        {
                            if (IsValidPlacement(grid, r, c, num))
                            {
                                grid[r, c] = num;
                                if (GenerateSolvedBoard(grid)) return true;
                                grid[r, c] = 0;
                            }
                        }
                        return false;
                    }
                }
            }

            return true;
        }

        private int CountSolutions(int[,] grid)
        {
            int count = 0;
            CountSolutions(grid, ref count);
            return count;
        }

        private void CountSolutions(int[,] grid, ref int count)
        {
            if (count > 1)
                return;

            int? row = null;
            int? col = null;

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (grid[r, c] == 0)
                    {
                        row = r;
                        col = c;
                        break;
                    }
                }

                if (row.HasValue)
                    break;
            }

            if (!row.HasValue || !col.HasValue)
            {
                count++;
                return;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsValidPlacement(grid, row.Value, col.Value, num))
                {
                    grid[row.Value, col.Value] = num;
                    CountSolutions(grid, ref count);
                    grid[row.Value, col.Value] = 0;
                }

                if (count > 1)
                    return;
            }
        }

        private void Shuffle(int[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rand.Next(n--);
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
