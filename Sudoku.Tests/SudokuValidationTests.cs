using System.Reflection;
using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public class SudokuValidationTests
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
    public void CheckBoard_ReturnsColumnViolationFormat()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];
        board[0, 0] = 1;
        board[1, 0] = 1;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.CheckBoard();

        Assert.Equal("Number 1 already exists in Column 1.", result);
    }

    [Fact]
    public void CheckBoard_ReturnsRowViolationFormat()
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
    public void CheckBoard_ReturnsSubgridViolationFormat()
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

    [Fact]
    public void CheckBoard_ReturnsNoViolationsMessage()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

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

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.CheckBoard();

        Assert.Equal("No rule violations detected.", result);
    }

    [Fact]
    public void CheckBoard_ReturnsSubgridViolationMessage_ForBoundaryCells()
    {
        var board = new int[9, 9];
        var clues = new bool[9, 9];

        board[0, 2] = 1;
        board[1, 0] = 2;
        board[1, 1] = 3;
        board[2, 0] = 1;

        var gameBoard = CreateBoardWithState(board, clues);

        var result = gameBoard.CheckBoard();

        Assert.Equal("Number 1 already exists in the same 3×3 subgrid.", result);
    }
}
