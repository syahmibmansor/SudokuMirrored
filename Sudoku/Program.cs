using System;

namespace Sudoku;

internal class Program
{
    /// <summary>
    /// Runs the interactive Sudoku game loop using the supplied console or test-friendly streams.
    /// </summary>
    /// <param name="inputReader">Optional input source for tests; otherwise Console.In is used.</param>
    /// <param name="outputWriter">Optional output writer for tests; otherwise Console.Out is used.</param>
    /// <param name="boardFactory">Optional board factory hook for controlled test scenarios.</param>
    /// <param name="keyReader">Optional key reader hook for replay or quit testing.</param>
    internal static void RunGame(
        TextReader? inputReader = null,
        TextWriter? outputWriter = null,
        Func<SudokuBoard>? boardFactory = null,
        Func<ConsoleKeyInfo>? keyReader = null)
    {
        TextReader input = inputReader ?? Console.In;
        TextWriter output = outputWriter ?? Console.Out;
        Func<SudokuBoard> gameBoardFactory = boardFactory ?? (() => new SudokuBoard());
        Func<ConsoleKeyInfo> readKey = keyReader ?? (() => Console.ReadKey(true));

        SudokuBoard gameBoard = gameBoardFactory();

        List<string> historyLog = ["Game loaded with 30 random clues. Good luck!"];
        string activeMessage = historyLog[0];
        bool gameCompleted = false;

        while (true)
        {
            if (gameCompleted)
            {
                if (outputWriter == null && !Console.IsOutputRedirected)
                {
                    Console.Clear();
                }

                output.WriteLine("You have successfully completed the Sudoku puzzle!");
                output.WriteLine("Press any key to play again, or Q to quit...");
                ConsoleKeyInfo key = readKey();
                if (char.ToLowerInvariant(key.KeyChar) == 'q')
                {
                    break;
                }

                gameBoard = gameBoardFactory();
                historyLog = ["Game loaded with 30 random clues. Good luck!"];
                activeMessage = historyLog[0];
                gameCompleted = false;
                continue;
            }

            if (outputWriter == null && !Console.IsOutputRedirected)
            {
                Console.Clear();
            }

            output.WriteLine("================= History Log  ================");
            foreach (string log in historyLog)
            {
                output.WriteLine($"  > {log}");
            }

            output.WriteLine("=================== Board =====================");
            gameBoard.Print();

            output.WriteLine("================ Current move =================");
            output.WriteLine($">>{activeMessage}");

            output.WriteLine("================== Commands ===================");
            output.WriteLine("Insert: [Cell] [Number]  (e.g., A3 4)");
            output.WriteLine("Clear:  [Cell] clear     (e.g., C5 clear)");
            output.WriteLine("Hint:   hint");
            output.WriteLine("Check:  check");
            output.WriteLine("Quit:   exit");
            output.WriteLine("===============================================");
            output.Write("Enter command: ");

            string? userInput = input.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput)) continue;

            ParsedCommand command = CommandParser.Parse(userInput);
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

    /// <summary>
    /// Application entry point for the command-line Sudoku game.
    /// </summary>
    /// <param name="args">Command-line arguments, currently unused.</param>
    private static void Main(string[] args)
    {
        RunGame();
    }
}