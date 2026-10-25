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
}
