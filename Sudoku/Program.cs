using Sudoku;
using System;

class Program
{
    static void Main(string[] args)
    {
        SudokuBoard gameBoard = new SudokuBoard();

        // Using a list that can freely expand upwards/downwards
        List<string> historyLog = new List<string> { "Game loaded with 30 random clues. Good luck!" };

        while (true)
        {
            Console.Clear();

            // 1. Display History Log FIRST (so it sits at the top and expands downward)
            Console.WriteLine("--- History Log ---");
            foreach (string log in historyLog)
            {
                Console.WriteLine($"  > {log}");
            }
            Console.WriteLine("-------------------\n");

            // 2. Display the Sudoku Board BELOW the history log
            gameBoard.Print();

            // 3. Display menu instructions & prompt
            Console.WriteLine("\nCommands:");
            Console.WriteLine("  Insert: [Cell] [Number]  (e.g., A3 4)");
            Console.WriteLine("  Clear:  [Cell] clear     (e.g., C5 clear)");
            Console.WriteLine("  Hint:   hint");
            Console.WriteLine("  Check:  check");
            Console.WriteLine("  Quit:   exit");
            Console.Write("\nEnter command: ");

            string? input = Console.ReadLine()?.Trim();
            if (input?.ToLower() == "exit") break;
            if (string.IsNullOrEmpty(input)) continue;

            // Handle special commands
            if (input.ToLower() == "hint")
            {
                string msg = gameBoard.GiveHint();
                historyLog.Add(msg);
                continue;
            }

            if (input.ToLower() == "check")
            {
                string msg = gameBoard.CheckBoard();
                historyLog.Add(msg);
                continue;
            }

            // Parse inputs like "A3 4" or "C5 clear"
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2 && parts[0].Length == 2)
            {
                string cellCoord = parts[0].ToUpper();
                char rowChar = cellCoord[0];
                char colChar = cellCoord[1];

                if (char.IsLetter(rowChar) && char.IsDigit(colChar))
                {
                    int row = rowChar - 'A';
                    int col = (colChar - '0') - 1;

                    if (row >= 0 && row < 9 && col >= 0 && col < 9)
                    {
                        if (parts[1].ToLower() == "clear")
                        {
                            bool cleared = gameBoard.TryClear(row, col);
                            if (cleared)
                                historyLog.Add($"Cleared cell {cellCoord}.");
                            else
                                historyLog.Add($"Error: Cannot clear initial clue at {cellCoord}!");
                        }
                        else if (int.TryParse(parts[1], out int num) && num >= 1 && num <= 9)
                        {
                            bool success = gameBoard.TryMakeMove(row, col, num);
                            if (success)
                                historyLog.Add($"Placed {num} at {cellCoord}.");
                            else
                                historyLog.Add($"Error: Invalid move or locked cell at {cellCoord}!");
                        }
                        else
                        {
                            historyLog.Add("Error: Invalid number. Use 1-9 or 'clear'.");
                        }
                        continue;
                    }
                }
            }

            historyLog.Add("Error: Invalid format. Use 'A3 4' or 'C5 clear'.");
        }
    }
}