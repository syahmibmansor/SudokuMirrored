using Sudoku;
using Xunit;

namespace Sudoku.Tests;

public class CommandParserTests
{
    [Fact]
    public void Parse_ValidCoordinateParsing()
    {
        var command = CommandParser.Parse("A3 4");

        Assert.Equal(CommandKind.Move, command.Kind);
        Assert.Equal(0, command.Row);
        Assert.Equal(2, command.Column);
        Assert.Equal(4, command.Value);
    }

    [Fact]
    public void Parse_HandlesCaseInsensitiveInput()
    {
        var lowercaseMove = CommandParser.Parse("a3 4");
        var lowercaseClear = CommandParser.Parse("c5 clear");

        Assert.Equal(CommandKind.Move, lowercaseMove.Kind);
        Assert.Equal(CommandKind.Clear, lowercaseClear.Kind);
        Assert.Equal(0, lowercaseMove.Row);
        Assert.Equal(2, lowercaseMove.Column);
        Assert.Equal(4, lowercaseMove.Value);
        Assert.Equal(2, lowercaseClear.Row);
        Assert.Equal(4, lowercaseClear.Column);
    }

    [Fact]
    public void Parse_RejectsOutOfBoundsRow()
    {
        var command = CommandParser.Parse("Z9 4");

        Assert.Equal(CommandKind.Invalid, command.Kind);
        Assert.Equal("Error: Invalid format. Use 'A3 4' or 'C5 clear'.", command.ErrorMessage);
    }

    [Fact]
    public void Parse_RejectsOutOfBoundsColumn()
    {
        var zeroColumn = CommandParser.Parse("A0 4");
        var tooLargeColumn = CommandParser.Parse("A10 4");

        Assert.Equal(CommandKind.Invalid, zeroColumn.Kind);
        Assert.Equal(CommandKind.Invalid, tooLargeColumn.Kind);
        Assert.Equal("Error: Invalid format. Use 'A3 4' or 'C5 clear'.", zeroColumn.ErrorMessage);
    }

    [Fact]
    public void Parse_RejectsOutOfRangeValues()
    {
        var lowValue = CommandParser.Parse("A3 0");
        var highValue = CommandParser.Parse("A3 12");

        Assert.Equal(CommandKind.Invalid, lowValue.Kind);
        Assert.Equal(CommandKind.Invalid, highValue.Kind);
        Assert.Equal("Error: Invalid number. Use 1-9 or 'clear'.", lowValue.ErrorMessage);
    }

    [Fact]
    public void Parse_HandlesMalformedCommandSyntax()
    {
        var missingValue = CommandParser.Parse("A3");
        var trailingWhitespace = CommandParser.Parse(" check ");
        var garbage = CommandParser.Parse("random text");

        Assert.Equal(CommandKind.Invalid, missingValue.Kind);
        Assert.Equal(CommandKind.Check, trailingWhitespace.Kind);
        Assert.Equal(CommandKind.Invalid, garbage.Kind);
        Assert.Equal("Error: Invalid format. Use 'A3 4' or 'C5 clear'.", missingValue.ErrorMessage);
    }

    [Fact]
    public void Parse_HandlesWhitespaceAndPadding()
    {
        var command = CommandParser.Parse("   A3    4   ");

        Assert.Equal(CommandKind.Move, command.Kind);
        Assert.Equal(0, command.Row);
        Assert.Equal(2, command.Column);
        Assert.Equal(4, command.Value);
    }

    [Fact]
    public void Parse_RejectsEmptyInputWithoutThrowing()
    {
        var command = CommandParser.Parse("   ");

        Assert.Equal(CommandKind.Invalid, command.Kind);
        Assert.Equal("Error: Invalid format. Use 'A3 4' or 'C5 clear'.", command.ErrorMessage);
    }

    [Fact]
    public void Parse_HandlesExitCommand()
    {
        var command = CommandParser.Parse("exit");

        Assert.Equal(CommandKind.Exit, command.Kind);
    }
}
