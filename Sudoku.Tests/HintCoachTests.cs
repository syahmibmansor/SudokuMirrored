using System.Reflection;
using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public class HintCoachTests
{
    private static int[,] CreateSolvedBoard()
    {
        return new int[,]
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
    }

    private static SudokuBoard CreateBoardWithState(int[,] board, bool[,] clues)
    {
        var gameBoard = new SudokuBoard();

        var boardField = typeof(SudokuBoard).GetField("board", BindingFlags.NonPublic | BindingFlags.Instance);
        var cluesField = typeof(SudokuBoard).GetField("initialClues", BindingFlags.NonPublic | BindingFlags.Instance);
        var masterField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(boardField);
        Assert.NotNull(cluesField);
        Assert.NotNull(masterField);

        boardField.SetValue(gameBoard, board);
        cluesField.SetValue(gameBoard, clues);
        masterField.SetValue(gameBoard, CreateSolvedBoard());

        return gameBoard;
    }

    [Fact]
    public void GiveHint_PrioritizesMultipleUserErrorsBeforeEmptyCells()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        board[0, 0] = 8;
        board[0, 1] = 7;
        board[0, 2] = 0;
        clues[0, 0] = false;
        clues[0, 1] = false;
        clues[0, 2] = false;

        var gameBoard = CreateBoardWithState(board, clues);
        var first = gameBoard.GiveHint();
        var second = gameBoard.GiveHint();

        Assert.Contains("Hint: Cell", first);
        Assert.Contains("Hint: Cell", second);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void GiveHint_IgnoresCorrectEntries()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        board[0, 0] = 5;
        board[0, 1] = 3;
        board[0, 2] = 4;
        board[0, 3] = 1;
        board[0, 4] = 7;
        board[0, 5] = 8;
        board[0, 6] = 9;
        board[0, 7] = 1;
        board[0, 8] = 2;

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = board[r, c] != 0;
            }
        }

        board[0, 3] = 9;
        clues[0, 3] = false;

        var gameBoard = CreateBoardWithState(board, clues);
        var hint = gameBoard.GiveHint();

        Assert.DoesNotContain("A1", hint);
        Assert.DoesNotContain("A2", hint);
        Assert.Contains("Hint: Cell", hint);
    }

    [Fact]
    public void GiveHint_ReturnsHintForEmptyBoardWithoutCrashing()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = false;
            }
        }

        var gameBoard = CreateBoardWithState(board, clues);
        var hint = gameBoard.GiveHint();

        Assert.Contains("Hint: Cell", hint);
        Assert.DoesNotContain("No empty or incorrect cells remain.", hint);
    }

    [Fact]
    public void GiveHint_PrioritizesIncorrectUserEntryBeforeEmptyCell()
    {
        var solvedBoard = CreateSolvedBoard();
        var board = (int[,])solvedBoard.Clone();
        var clues = new bool[9, 9];

        board[0, 0] = 8;
        board[0, 1] = 0;
        clues[0, 0] = false;
        clues[0, 1] = false;

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (r == 0 && c < 2)
                {
                    continue;
                }

                if (board[r, c] != 0)
                {
                    clues[r, c] = true;
                }
            }
        }

        var gameBoard = CreateBoardWithState(board, clues);
        var masterSolutionField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterSolutionField);
        masterSolutionField.SetValue(gameBoard, solvedBoard);

        var hint = gameBoard.GiveHint();

        Assert.Contains("A1", hint);
        Assert.Contains("The current value is incorrect.", hint);
    }

    [Fact]
    public void GiveHint_OnNearlyFullBoard_TargetsFinalRemainingCell()
    {
        var solvedBoard = CreateSolvedBoard();
        var board = (int[,])solvedBoard.Clone();
        var clues = new bool[9, 9];

        board[8, 8] = 0;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = board[r, c] != 0;
            }
        }
        clues[8, 8] = false;

        var gameBoard = CreateBoardWithState(board, clues);
        var masterSolutionField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterSolutionField);
        masterSolutionField.SetValue(gameBoard, solvedBoard);

        var hint = gameBoard.GiveHint();

        Assert.Contains("I9", hint);
        Assert.Contains("should be", hint);
    }
}
