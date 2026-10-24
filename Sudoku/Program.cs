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

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            ParsedCommand command = CommandParser.Parse(input);
            if (command.Kind == CommandKind.Exit) break;

            if (command.Kind == CommandKind.Hint)
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

            if (command.Kind == CommandKind.Check)
            {
                string msg = gameBoard.CheckBoard();
                historyLog.Add(msg);
                activeMessage = msg;
                continue;
            }

            if (command.Kind == CommandKind.Clear)
            {
                string cellCoord = $"{(char)('A' + command.Row)}{command.Column + 1}";
                gameBoard.ClearHighlight();
                bool cleared = gameBoard.TryClear(command.Row, command.Column);
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
                continue;
            }

            if (command.Kind == CommandKind.Move)
            {
                string cellCoord = $"{(char)('A' + command.Row)}{command.Column + 1}";
                gameBoard.ClearHighlight();
                bool success = gameBoard.TryMakeMove(command.Row, command.Column, command.Value);
                if (success)
                {
                    string msg = $"Move accepted. Placed {command.Value} at {cellCoord}.";
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
                continue;
            }

            historyLog.Add(command.ErrorMessage);
            activeMessage = command.ErrorMessage;
        }
    }
}