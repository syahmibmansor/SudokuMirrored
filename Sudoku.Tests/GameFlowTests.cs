using System;
using System.IO;
using System.Reflection;
using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public class GameFlowTests
{
    private static SudokuBoard CreateBoardWithState(int[,] board, bool[,] clues)
    {
        var gameBoard = new SudokuBoard();

        var boardField = typeof(SudokuBoard).GetField("board", BindingFlags.NonPublic | BindingFlags.Instance);
        var cluesField = typeof(SudokuBoard).GetField("initialClues", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(boardField);
        Assert.NotNull(cluesField);

        boardField.SetValue(gameBoard, board);
        cluesField.SetValue(gameBoard, clues);

        return gameBoard;
    }

    [Fact]
    public void TryClear_EmptyCell_DoesNotThrowAndDoesNotChangeState()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        var gameBoard = CreateBoardWithState(board, clues);

        var exception = Record.Exception(() => gameBoard.TryClear(0, 0));

        Assert.Null(exception);
        Assert.Equal(0, board[0, 0]);
    }

    [Fact]
    public void TryClear_PreFilledCell_IsRejected()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 5;
        clues[0, 0] = true;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.TryClear(0, 0);

        Assert.False(result);
        Assert.Equal(5, board[0, 0]);
    }

    [Fact]
    public void TryMakeMove_AllowsUserToOverwriteTheirOwnValue()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        var gameBoard = CreateBoardWithState(board, clues);

        Assert.True(gameBoard.TryMakeMove(0, 0, 4));
        Assert.True(gameBoard.TryMakeMove(0, 0, 7));
        Assert.Equal(7, board[0, 0]);
    }

    [Fact]
    public void BoardCompletion_IsDetectedWhenSolved()
    {
        int[,] solvedBoard =
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };
        var clues = new bool[9, 9];

        var gameBoard = CreateBoardWithState(solvedBoard, clues);

        Assert.True(gameBoard.IsBoardComplete());
        Assert.Equal("You have successfully completed the Sudoku puzzle!", gameBoard.CheckBoard());
    }

    [Fact]
    public void Print_RendersBoardHeaderAndContents()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        board[0, 0] = 5;
        board[0, 1] = 3;
        board[0, 2] = 4;
        board[8, 8] = 9;
        clues[0, 0] = true;
        clues[0, 1] = true;
        clues[0, 2] = true;
        clues[8, 8] = true;

        var gameBoard = CreateBoardWithState(board, clues);
        var originalOut = Console.Out;

        using var writer = new StringWriter();
        try
        {
            Console.SetOut(writer);
            gameBoard.Print();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = writer.ToString();
        Assert.Contains("1 2 3", output);
        Assert.Contains("A |", output);
        Assert.Contains("5", output);
        Assert.Contains("9", output);
    }

    [Fact]
    public void ClearHighlight_RemovesCurrentHighlightedCell()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        var gameBoard = CreateBoardWithState(board, clues);
        var highlightedField = typeof(SudokuBoard).GetField("highlightedCell", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(highlightedField);
        highlightedField.SetValue(gameBoard, (0, 0));

        gameBoard.ClearHighlight();

        Assert.Null(highlightedField.GetValue(gameBoard));
    }

    [Fact]
    public void RunGame_ExitsCleanly_WhenExitCommandIsEntered()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var input = new StringReader("exit" + Environment.NewLine);
        var output = new StringWriter();
        var programType = typeof(SudokuBoard).Assembly.GetType("Sudoku.Program", throwOnError: true);
        var runGameMethod = programType!.GetMethod("RunGame", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.NotNull(programType);
        Assert.NotNull(runGameMethod);

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            var exception = Record.Exception(() => runGameMethod.Invoke(null, new object?[] { input, output, null, null }));

            Assert.Null(exception);
            Assert.Contains("History Log", output.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void RunGame_CompletesBoardAndExits_WhenUserQuitsAfterVictory()
    {
        var solvedBoard = new int[9, 9]
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        var board = new int[9, 9];
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                board[r, c] = solvedBoard[r, c];
            }
        }

        board[0, 0] = 0;
        var clues = new bool[9, 9];
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = board[r, c] != 0;
            }
        }
        clues[0, 0] = false;

        var gameBoard = CreateBoardWithState(board, clues);
        var masterField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterField);
        masterField.SetValue(gameBoard, solvedBoard);

        var programType = typeof(SudokuBoard).Assembly.GetType("Sudoku.Program", throwOnError: true);
        var runGameMethod = programType!.GetMethod("RunGame", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(runGameMethod);

        var originalIn = Console.In;
        var originalOut = Console.Out;
        var output = new StringWriter();
        var input = new StringReader("A1 5" + Environment.NewLine);

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            var exception = Record.Exception(() => runGameMethod!.Invoke(null, new object?[]
            {
                input,
                output,
                new Func<SudokuBoard>(() => gameBoard),
                new Func<ConsoleKeyInfo>(() => new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false))
            }));

            Assert.Null(exception);
            Assert.Contains("You have successfully completed the Sudoku puzzle!", output.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void RunGame_CoversHintCheckClearMoveAndErrorBranches()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 5;
        clues[0, 0] = true;

        var gameBoard = CreateBoardWithState(board, clues);
        var solvedBoard = new int[9, 9]
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        var masterField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterField);
        masterField.SetValue(gameBoard, solvedBoard);

        var programType = typeof(SudokuBoard).Assembly.GetType("Sudoku.Program", throwOnError: true);
        var runGameMethod = programType!.GetMethod("RunGame", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(runGameMethod);

        var input = new StringReader("check\nhint\nB1 clear\nA1 clear\nA1 5\nB1 5\nbad-command\nexit\n");
        var output = new StringWriter();

        var exception = Record.Exception(() => runGameMethod.Invoke(null, new object?[]
        {
            input,
            output,
            new Func<SudokuBoard>(() => gameBoard),
            new Func<ConsoleKeyInfo>(() => new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false))
        }));

        Assert.Null(exception);
        Assert.Contains("No rule violations detected.", output.ToString());
        Assert.Contains("Hint: Cell", output.ToString());
        Assert.Contains("Move accepted. Cleared B1.", output.ToString());
        Assert.Contains("Invalid move. A1 is pre-filled.", output.ToString());
        Assert.Contains("Move accepted. Placed 5 at B1.", output.ToString());
        Assert.Contains("Error: Invalid format.", output.ToString());
    }

    [Fact]
    public void RunGame_TriggersCompletion_WhenBoardIsSolvedByMove()
    {
        var solvedBoard = new int[9, 9]
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        var board = new int[9, 9];
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                board[r, c] = solvedBoard[r, c];
            }
        }
        board[8, 8] = 0;

        var clues = new bool[9, 9];
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = false;
            }
        }

        var gameBoard = CreateBoardWithState(board, clues);
        var masterField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterField);
        masterField.SetValue(gameBoard, solvedBoard);

        var programType = typeof(SudokuBoard).Assembly.GetType("Sudoku.Program", throwOnError: true);
        var runGameMethod = programType!.GetMethod("RunGame", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(runGameMethod);

        var input = new StringReader("I9 9\n");
        var output = new StringWriter();
        var exception = Record.Exception(() => runGameMethod.Invoke(null, new object?[]
        {
            input,
            output,
            new Func<SudokuBoard>(() => gameBoard),
            new Func<ConsoleKeyInfo>(() => new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false))
        }));

        Assert.Null(exception);
        Assert.Contains("You have successfully completed the Sudoku puzzle!", output.ToString());
    }

    [Fact]
    public void Main_InvokesGameLoop_AndExitsOnExitCommand()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var consoleInput = new StringReader("exit" + Environment.NewLine);
        var consoleOutput = new StringWriter();
        var programType = typeof(SudokuBoard).Assembly.GetType("Sudoku.Program", throwOnError: true);
        var mainMethod = programType!.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.NotNull(mainMethod);

        try
        {
            Console.SetIn(consoleInput);
            Console.SetOut(consoleOutput);

            var exception = Record.Exception(() => mainMethod.Invoke(null, new object[] { Array.Empty<string>() }));

            Assert.Null(exception);
            Assert.Contains("History Log", consoleOutput.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
