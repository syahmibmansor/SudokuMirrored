using Sudoku;
using System;

class Program
{
    static void Main(string[] args)
    {
        SudokuBoard gameBoard = new SudokuBoard();

        List<string> historyLog = new List<string> { "Game loaded with 30 random clues. Good luck!" };
        string activeMessage = historyLog[0];
        bool gameCompleted = false;

        while (true)
        {
            if (gameCompleted)
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }

                Console.WriteLine("You have successfully completed the Sudoku puzzle!");
                Console.WriteLine("Press any key to play again, or Q to quit...");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (char.ToLowerInvariant(key.KeyChar) == 'q')
                {
                    break;
                }

                gameBoard = new SudokuBoard();
                historyLog = new List<string> { "Game loaded with 30 random clues. Good luck!" };
                activeMessage = historyLog[0];
                gameCompleted = false;
                continue;
            }

            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }

            Console.WriteLine("================= History Log  ================");
            foreach (string log in historyLog)
            {
                Console.WriteLine($"  > {log}");
            }

            Console.WriteLine("=================== Board =====================");
            gameBoard.Print();

            Console.WriteLine("================ Current move =================");
            Console.WriteLine($">>{activeMessage}");

            Console.WriteLine("================== Commands ===================");
            Console.WriteLine("Insert: [Cell] [Number]  (e.g., A3 4)");
            Console.WriteLine("Clear:  [Cell] clear     (e.g., C5 clear)");
            Console.WriteLine("Hint:   hint");
            Console.WriteLine("Check:  check");
            Console.WriteLine("Quit:   exit");
            Console.WriteLine("===============================================");
            Console.Write("Enter command: ");

            string? input = Console.ReadLine()?.Trim();
            if (input?.ToLower() == "exit") break;
            if (string.IsNullOrEmpty(input)) continue;

            if (input.ToLower() == "hint")
            {
                if (gameBoard.HasRuleViolation())
                {
                    string hintMessage = "Hint: The current board has rule violations. Fix them before requesting a hint.";
                    historyLog.Add(hintMessage);
                    activeMessage = hintMessage;
                    continue;
                }

                string hintMessage2 = gameBoard.GiveHint();
                historyLog.Add(hintMessage2);
                activeMessage = hintMessage2;
                continue;
            }

            if (input.ToLower() == "check")
            {
                string msg = gameBoard.CheckBoard();
                historyLog.Add(msg);
                activeMessage = msg;
                continue;
            }

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
                        gameBoard.ClearHighlight();

                        if (parts[1].ToLower() == "clear")
                        {
                            bool cleared = gameBoard.TryClear(row, col);
                            if (cleared)
                            {
                                string msg = $"Move accepted. Cleared {cellCoord}.";
                                historyLog.Add(msg);
                                activeMessage = msg;
                            }
                            else
                            {
                                string msg = $"Invalid move. {cellCoord} is pre-filled.";
                                historyLog.Add(msg);
                                activeMessage = msg;
                            }
                        }
                        else if (int.TryParse(parts[1], out int num) && num >= 1 && num <= 9)
                        {
                            bool success = gameBoard.TryMakeMove(row, col, num);
                            if (success)
                            {
                                string msg = $"Move accepted. Placed {num} at {cellCoord}.";
                                historyLog.Add(msg);
                                activeMessage = msg;

                                if (gameBoard.IsBoardComplete() && !gameBoard.HasRuleViolation())
                                {
                                    string completed = "You have successfully completed the Sudoku puzzle!";
                                    historyLog.Add(completed);
                                    activeMessage = completed;
                                    gameCompleted = true;
                                    continue;
                                }
                            }
                            else
                            {
                                string msg = $"Invalid move. {cellCoord} is pre-filled.";
                                historyLog.Add(msg);
                                activeMessage = msg;
                            }
                        }
                        else
                        {
                            string msg = "Error: Invalid number. Use 1-9 or 'clear'.";
                            historyLog.Add(msg);
                            activeMessage = msg;
                        }
                        continue;
                    }
                }
            }

            string invalidMsg = "Error: Invalid format. Use 'A3 4' or 'C5 clear'.";
            historyLog.Add(invalidMsg);
            activeMessage = invalidMsg;
        }
    }
}