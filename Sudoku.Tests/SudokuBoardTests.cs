using System.Reflection;
using Xunit;

namespace Sudoku.Tests;

public class SudokuBoardTests
{
    private static int[,] CreateSolvedBoard()
    {
        int[,] board =
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

        return board;
    }

    private static SudokuBoard CreateBoardWithState(int[,] board, bool[,] initialClues)
    {
        var gameBoard = new SudokuBoard();

        var boardField = typeof(SudokuBoard).GetField("board", BindingFlags.NonPublic | BindingFlags.Instance);
        var cluesField = typeof(SudokuBoard).GetField("initialClues", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(boardField);
        Assert.NotNull(cluesField);

        boardField.SetValue(gameBoard, board);
        cluesField.SetValue(gameBoard, initialClues);

        return gameBoard;
    }

    [Fact]
    public void Constructor_GeneratesBoardWithRandomClues()
    {
        var board = new SudokuBoard();

        var boardField = typeof(SudokuBoard).GetField("board", BindingFlags.NonPublic | BindingFlags.Instance);
        var cluesField = typeof(SudokuBoard).GetField("initialClues", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(boardField);
        Assert.NotNull(cluesField);

        var currentBoard = Assert.IsType<int[,]>(boardField.GetValue(board));
        var clues = Assert.IsType<bool[,]>(cluesField.GetValue(board));

        Assert.Equal(9, currentBoard.GetLength(0));
        Assert.Equal(9, currentBoard.GetLength(1));

        int clueCount = 0;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (currentBoard[r, c] != 0)
                {
                    clueCount++;
                }

                Assert.Equal(currentBoard[r, c] != 0, clues[r, c]);
            }
        }

        Assert.InRange(clueCount, 1, 80);
    }

    [Fact]
    public void TryMakeMove_AllowsValidMoveOnEmptyCell()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.TryMakeMove(0, 0, 5);

        Assert.True(result);
        Assert.Equal(5, GetBoardValue(gameBoard, 0, 0));
    }

    [Fact]
    public void TryMakeMove_RejectsMoveOnInitialClue()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 5;
        clues[0, 0] = true;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.TryMakeMove(0, 0, 7);

        Assert.False(result);
        Assert.Equal(5, GetBoardValue(gameBoard, 0, 0));
    }

    [Fact]
    public void TryMakeMove_AllowsDuplicateValueForLaterValidation()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 1;
        board[0, 1] = 2;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.TryMakeMove(0, 1, 1);

        Assert.True(result);
        Assert.Equal(1, GetBoardValue(gameBoard, 0, 1));
    }

    [Fact]
    public void TryClear_RemovesUserEntryButKeepsInitialClueLocked()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[2, 3] = 7;
        clues[2, 3] = false;

        var gameBoard = CreateBoardWithState(board, clues);

        Assert.True(gameBoard.TryClear(2, 3));
        Assert.Equal(0, GetBoardValue(gameBoard, 2, 3));

        board[1, 1] = 9;
        clues[1, 1] = true;
        var lockedBoard = CreateBoardWithState(board, clues);

        Assert.False(lockedBoard.TryClear(1, 1));
    }

    [Fact]
    public void GiveHint_ReportsCorrectValueWithoutChangingBoard()
    {
        int[,] board = new int[9, 9];
        bool[,] clues = new bool[9, 9];

        board[0, 0] = 5;
        board[0, 1] = 3;
        board[0, 2] = 4;
        board[0, 3] = 6;
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

        board[8, 8] = 0;
        clues[8, 8] = false;

        var gameBoard = CreateBoardWithState(board, clues);

        string message = gameBoard.GiveHint();

        Assert.Contains("Hint: Cell", message);
        Assert.Equal(0, GetBoardValue(gameBoard, 8, 8));
    }

    [Fact]
    public void GiveHint_UsesDifferentTargetOnRepeatCall_WhenOnlyOneIncorrectCellExists()
    {
        var solvedBoard = CreateSolvedBoard();
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        board[0, 0] = 8;
        clues[0, 0] = false;

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (r == 0 && c == 0)
                {
                    continue;
                }

                clues[r, c] = board[r, c] != 0;
            }
        }

        var gameBoard = CreateBoardWithState(board, clues);
        var masterSolutionField = typeof(SudokuBoard).GetField("masterSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(masterSolutionField);
        masterSolutionField.SetValue(gameBoard, solvedBoard);

        string firstHint = gameBoard.GiveHint();
        string secondHint = gameBoard.GiveHint();

        Assert.Contains("A1", firstHint);
        Assert.DoesNotContain("A1", secondHint);
        Assert.Contains("Hint: Cell", secondHint);
    }

    [Fact]
    public void CheckBoard_ReturnsSuccessWhenBoardIsFullyFilled()
    {
        int[,] solvedBoard = CreateSolvedBoard();
        bool[,] clues = new bool[9, 9];

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                clues[r, c] = solvedBoard[r, c] != 0;
            }
        }

        var gameBoard = CreateBoardWithState(solvedBoard, clues);

        var result = gameBoard.CheckBoard();

        Assert.Contains("successfully completed", result);
    }

    [Fact]
    public void HasRuleViolation_ReturnsTrue_WhenDuplicateExists()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 1;
        board[0, 1] = 1;

        var gameBoard = CreateBoardWithState(board, clues);

        Assert.True(gameBoard.HasRuleViolation());
        Assert.Contains((0, 0), gameBoard.GetInvalidCells());
    }

    [Fact]
    public void CheckBoard_ReturnsRowViolationMessage()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 1;
        board[0, 1] = 1;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.CheckBoard();

        Assert.Equal("Number 1 already exists in Row A.", result);
    }

    [Fact]
    public void CheckBoard_ReturnsSubgridViolationMessage()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 1;
        board[0, 1] = 2;
        board[0, 2] = 3;
        board[1, 0] = 4;
        board[1, 1] = 5;
        board[1, 2] = 6;
        board[2, 0] = 7;
        board[2, 1] = 8;
        board[2, 2] = 1;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.CheckBoard();

        Assert.Equal("Number 1 already exists in the same 3×3 subgrid.", result);
    }

    private static int GetBoardValue(SudokuBoard board, int row, int col)
    {
        var boardField = typeof(SudokuBoard).GetField("board", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(boardField);

        var currentBoard = Assert.IsType<int[,]>(boardField.GetValue(board));
        return currentBoard[row, col];
    }
}
