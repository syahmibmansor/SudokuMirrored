using System;

namespace Sudoku
{
    /// <summary>
    /// Represents the user command type parsed from console input.
    /// </summary>
    public enum CommandKind
    {
        /// <summary>
        /// The command was invalid or malformed.
        /// </summary>
        Invalid,
        /// <summary>
        /// A board move command such as A3 4.
        /// </summary>
        Move,
        /// <summary>
        /// A clear command such as C5 clear.
        /// </summary>
        Clear,
        /// <summary>
        /// A hint request.
        /// </summary>
        Hint,
        /// <summary>
        /// A rule-check request.
        /// </summary>
        Check,
        /// <summary>
        /// A quit command.
        /// </summary>
        Exit
    }

    /// <summary>
    /// A parsed representation of a user command.
    /// </summary>
    public sealed class ParsedCommand
    {
        /// <summary>
        /// Gets the command kind.
        /// </summary>
        public CommandKind Kind { get; }
        /// <summary>
        /// Gets the target row index, if applicable.
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// Gets the target column index, if applicable.
        /// </summary>
        public int Column { get; }
        /// <summary>
        /// Gets the value to place in a move command.
        /// </summary>
        public int Value { get; }
        /// <summary>
        /// Gets the validation message for invalid input.
        /// </summary>
        public string ErrorMessage { get; }

        private ParsedCommand(CommandKind kind, int row = 0, int column = 0, int value = 0, string? errorMessage = null)
        {
            Kind = kind;
            Row = row;
            Column = column;
            Value = value;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// Creates an invalid command result.
        /// </summary>
        /// <param name="message">The error message describing the invalid input.</param>
        /// <returns>A command result representing invalid input.</returns>
        public static ParsedCommand InvalidResult(string message) => new ParsedCommand(CommandKind.Invalid, errorMessage: message);
        /// <summary>
        /// Creates a move command result.
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="column">The zero-based column index.</param>
        /// <param name="value">The numeric value to place.</param>
        /// <returns>A parsed move command.</returns>
        public static ParsedCommand MoveResult(int row, int column, int value) => new ParsedCommand(CommandKind.Move, row, column, value);
        /// <summary>
        /// Creates a clear command result.
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="column">The zero-based column index.</param>
        /// <returns>A parsed clear command.</returns>
        public static ParsedCommand ClearResult(int row, int column) => new ParsedCommand(CommandKind.Clear, row, column);
        /// <summary>
        /// Creates a hint command result.
        /// </summary>
        /// <returns>A parsed hint command.</returns>
        public static ParsedCommand HintResult() => new ParsedCommand(CommandKind.Hint);
        /// <summary>
        /// Creates a check command result.
        /// </summary>
        /// <returns>A parsed check command.</returns>
        public static ParsedCommand CheckResult() => new ParsedCommand(CommandKind.Check);
        /// <summary>
        /// Creates an exit command result.
        /// </summary>
        /// <returns>A parsed exit command.</returns>
        public static ParsedCommand ExitResult() => new ParsedCommand(CommandKind.Exit);
    }

    /// <summary>
    /// Parses and validates console input to Sudoku commands.
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// Parses the supplied input string into a command.
        /// </summary>
        /// <param name="input">The raw user input string.</param>
        /// <returns>A parsed command object.</returns>
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
