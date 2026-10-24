using System;

namespace Sudoku
{
    public enum CommandKind
    {
        Invalid,
        Move,
        Clear,
        Hint,
        Check,
        Exit
    }

    public sealed class ParsedCommand
    {
        public CommandKind Kind { get; }
        public int Row { get; }
        public int Column { get; }
        public int Value { get; }
        public string ErrorMessage { get; }

        private ParsedCommand(CommandKind kind, int row = 0, int column = 0, int value = 0, string? errorMessage = null)
        {
            Kind = kind;
            Row = row;
            Column = column;
            Value = value;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public static ParsedCommand InvalidResult(string message) => new ParsedCommand(CommandKind.Invalid, errorMessage: message);
        public static ParsedCommand MoveResult(int row, int column, int value) => new ParsedCommand(CommandKind.Move, row, column, value);
        public static ParsedCommand ClearResult(int row, int column) => new ParsedCommand(CommandKind.Clear, row, column);
        public static ParsedCommand HintResult() => new ParsedCommand(CommandKind.Hint);
        public static ParsedCommand CheckResult() => new ParsedCommand(CommandKind.Check);
        public static ParsedCommand ExitResult() => new ParsedCommand(CommandKind.Exit);
    }

    public static class CommandParser
    {
        public static ParsedCommand Parse(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return ParsedCommand.InvalidResult("Error: Invalid format. Use 'A3 4' or 'C5 clear'.");
            }

            string trimmed = input.Trim();
            if (trimmed.Equals("hint", StringComparison.OrdinalIgnoreCase))
            {
                return ParsedCommand.HintResult();
            }

            if (trimmed.Equals("check", StringComparison.OrdinalIgnoreCase))
            {
                return ParsedCommand.CheckResult();
            }

            if (trimmed.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return ParsedCommand.ExitResult();
            }

            string[] parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                return ParsedCommand.InvalidResult("Error: Invalid format. Use 'A3 4' or 'C5 clear'.");
            }

            string cell = parts[0];
            if (cell.Length != 2 || !char.IsLetter(cell[0]) || !char.IsDigit(cell[1]))
            {
                return ParsedCommand.InvalidResult("Error: Invalid format. Use 'A3 4' or 'C5 clear'.");
            }

            int row = char.ToUpperInvariant(cell[0]) - 'A';
            int column = cell[1] - '0';

            if (row < 0 || row > 8 || column < 1 || column > 9)
            {
                return ParsedCommand.InvalidResult("Error: Invalid format. Use 'A3 4' or 'C5 clear'.");
            }

            if (parts[1].Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                return ParsedCommand.ClearResult(row, column - 1);
            }

            if (!int.TryParse(parts[1], out int value) || value < 1 || value > 9)
            {
                return ParsedCommand.InvalidResult("Error: Invalid number. Use 1-9 or 'clear'.");
            }

            return ParsedCommand.MoveResult(row, column - 1, value);
        }
    }
}
