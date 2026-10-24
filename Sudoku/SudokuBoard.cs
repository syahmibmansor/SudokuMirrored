using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class SudokuBoard
    {
        private int[,] board = new int[9, 9];
        private readonly bool[,] initialClues = new bool[9, 9];
        private readonly Random rand = new Random();

        public SudokuBoard()
        {
            GenerateRandomPuzzle();
        }

        private void GenerateRandomPuzzle()
        {
            board = new int[9, 9];
            SolveSudoku(board);

            int cellsToRemove = 51; // Leave 30 pre-filled clues
            while (cellsToRemove > 0)
            {
                int r = rand.Next(0, 9);
                int c = rand.Next(0, 9);

                if (board[r, c] != 0)
                {
                    board[r, c] = 0;
                    cellsToRemove--;
                }
            }

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] != 0)
                    {
                        initialClues[r, c] = true;
                    }
                    else
                    {
                        initialClues[r, c] = false;
                    }
                }
            }
        }

        public void Print()
        {
            Console.WriteLine("    1 2 3   4 5 6   7 8 9 ");
            Console.WriteLine("  -------------------------");
            for (int r = 0; r < 9; r++)
            {
                char rowLabel = (char)('A' + r);
                Console.Write($"{rowLabel} | ");
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(". ");
                    }
                    else if (initialClues[r, c])
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"{board[r, c]} ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{board[r, c]} ");
                    }

                    Console.ResetColor();

                    if ((c + 1) % 3 == 0) Console.Write("| ");
                }
                Console.WriteLine();
                if ((r + 1) % 3 == 0) Console.WriteLine("  -------------------------");
            }
        }

        public bool TryMakeMove(int row, int col, int num)
        {
            if (initialClues[row, col]) return false;

            if (IsValidPlacement(board, row, col, num))
            {
                board[row, col] = num;
                return true;
            }
            return false;
        }

        public bool TryClear(int row, int col)
        {
            if (initialClues[row, col]) return false;

            board[row, col] = 0;
            return true;
        }

        public string CheckBoard()
        {
            bool complete = true;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0) complete = false;
                }
            }

            if (complete)
                return "Check: Congratulations! The board is completely filled and valid!";
            else
                return "Check: No rule violations found so far. Keep going!";
        }

        public string GiveHint()
        {
            int emptyRow = -1;
            int emptyCol = -1;

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        emptyRow = r;
                        emptyCol = c;
                        break;
                    }
                }

                if (emptyRow >= 0)
                {
                    break;
                }
            }

            if (emptyRow < 0 || emptyCol < 0)
            {
                return "Hint: No empty cells left to solve!";
            }

            int[,] solvedCopy = (int[,])board.Clone();
            if (!SolveSudoku(solvedCopy))
            {
                return "Hint: Unable to find a valid solution for the current board.";
            }

            int value = solvedCopy[emptyRow, emptyCol];
            board[emptyRow, emptyCol] = value;

            char rowLabel = (char)('A' + emptyRow);
            return $"Hint: Filled {rowLabel}{emptyCol + 1} with {value}";
        }

        private bool IsValidPlacement(int[,] grid, int row, int col, int num)
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

        private bool SolveSudoku(int[,] grid)
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
                                if (SolveSudoku(grid)) return true;
                                grid[r, c] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
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
