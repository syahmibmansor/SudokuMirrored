# Unit Testing Summary

This solution uses xUnit to validate gameplay, input parsing, validation rules, hints, and user flow. The tests are grouped by responsibility so they are easier to understand, extend, and maintain.

## 1. Board and Puzzle Mechanics

These tests cover the core Sudoku board model and puzzle generation behavior.

### SudokuBoardTests
- Generates a valid Sudoku board with a complete solution
- Creates a puzzle with the required number of clues removed
- Ensures the board contains empty cells represented correctly
- Validates correct placement into empty cells
- Rejects assignment to a pre-filled clue cell
- Clears a user-filled cell back to empty
- Does not crash when clearing an already empty cell
- Detects the board as complete when all cells are filled correctly
- Preserves clue values and prevents overwriting them
- Verifies the initial puzzle contains exactly 30 clues
- Confirms the generated board is valid and has no rule violations at startup
- Rejects out-of-range values outside 1-9 and preserves the latest valid user entry
- Returns false for an incomplete board when any cell remains empty
- Identifies invalid cells across duplicate row, column, and subgrid boundaries
- Returns the final no-more-hints message when the board is already fully correct
- Renders board output correctly for highlighted and invalid cell states

## 2. Command Prompt and Input Parsing

These tests cover command parsing and console input safety.

### CommandParserTests
- Parses valid commands such as `A3 4`
- Parses commands case-insensitively, such as `a3 4` and `c5 clear`
- Maps row/column coordinates to the correct board indices
- Rejects invalid row values such as `Z9 4` or `J1 5`
- Rejects invalid column values such as `A0 4` or `A10 4`
- Rejects values outside the 1-9 range
- Handles malformed input cleanly without crashing
- Rejects incomplete commands like `A3` or random garbage input
- Accepts clear commands such as `C5 clear`
- Accepts other valid commands such as `hint`, `check`, and `exit`
- Handles extra whitespace and padded input
- Rejects empty input without crashing
- Gracefully accepts exit commands
- Normalizes padded and lowercase command variants for resilient CLI input parsing
- Safely ignores blank lines or enter-only input without breaking the game loop

### Input Validation Scenarios
- Valid Coordinate Parsing: `A3 4` -> Row 0, Column 2, Value 4
- Case-Insensitive Input: lowercase row/command parsing behaves the same as uppercase input
- Out-of-Bounds Row Handling: invalid row letters fail gracefully
- Out-of-Bounds Column Handling: invalid column numbers fail gracefully
- Value Range Enforcement: values below 1 and above 9 are rejected
- Malformed Command Syntax: missing values and garbage commands are handled safely
- Whitespace & Formatting Resilience: extra spaces and padded input are normalized
- Empty Input / Enter Key Press: blank input is handled safely and re-prompts cleanly
- Quit Command Handling: `exit` gracefully terminates the loop

## 3. Rule Validation and Message Precision

These tests validate the exact wording and logic of Sudoku rule checking.

### SudokuValidationTests
- Detects a duplicate in a row and returns the expected message
- Detects a duplicate in a column and returns the exact column violation message
- Detects a duplicate in a 3x3 subgrid and returns the expected subgrid message
- Returns `No rule violations detected.` when the board is valid
- Verifies the exact format required by the specification
- Tests a subgrid boundary case spanning the 3x3 block edges

### Exact Validation Message Expectations
- `Number [X] already exists in Column 1.`
- `Number [X] already exists in Row A.`
- `Number [X] already exists in the same 3×3 subgrid.`
- `No rule violations detected.`

## 4. Hint Coach and Smart Guidance

These tests verify the hint system behavior and decision order.

### HintCoachTests
- Prioritizes incorrect user entries before revealing empty cells
- If multiple incorrect values exist, resolves them in a deterministic order
- Ignores correct user entries and does not keep reusing them
- Reveals a valid hint for a random empty cell when the board has no incorrect player input
- Prevents repeated hints on the same cell in a stale loop
- Moves on after known wrong entries have already been checked
- Does not crash when requesting a hint on a fresh board with only initial clues
- Ensures a hint does not mutate the board state
- Handles nearly full boards and final remaining cells without looping
- Explicitly verifies wrong-user-entry priority over empty-cell hints

### Smart Hint Scenarios
- One incorrect user action -> hint reveals the correct value
- Multiple user mistakes -> hint resolves the first root error before empty cells
- Empty board or untouched board -> hint reveals a valid empty-cell target
- Repeat hint requests -> system advances instead of getting stuck on the same action
- Nearly full board -> final empty cell is hinted correctly without looping

## 5. Game Flow and Completion Lifecycle

These tests cover the overall gameplay loop and end-state behavior.

### GameFlowTests
- Clear on an already empty cell does nothing and does not throw
- Clear on a user-filled cell resets the cell to empty
- Initial clue cells cannot be cleared
- Invalid move message is returned when attempting to clear a pre-filled clue
- Final correct move triggers completion automatically
- Completion message is shown when the puzzle is solved
- The board is marked complete at the correct moment
- Restart / replay flow is handled after winning
- A user can overwrite a value they previously entered themselves
- Clearing a user-filled cell works; clearing an initial clue cell remains blocked
- `Print()` renders the board consistently and includes expected headings and values
- `ClearHighlight()` clears the active hint highlight without throwing exceptions
- Game loop exits cleanly on `exit` without unhandled errors
- Completion flow handles win state and quit-after-victory behavior
- Command-processing branches cover `hint`, `check`, `clear`, `move`, invalid input, and exit logic in one end-to-end game loop scenario
- Main entrypoint execution path is tested for graceful startup and exit handling

### Completion Flow Expectations
- `You have successfully completed the Sudoku puzzle!`
- `Press any key to play again...` after a successful win

## 6. Regression and Quality Coverage

The suite is designed to catch both logic regressions and user experience issues, including:
- invalid input handling
- clue protection
- wrong-value prioritization in hints
- duplicate log or stale state issues in the console flow
- completion detection and replay flow
- exact validation output strings required by the specification
- 3x3 subgrid boundary cases
- startup board integrity and exact clue count validation
- whitespace and CLI resilience
- safe handling of empty input and exit commands
- rendering and highlight behavior in console output
- no-hint final state when the board is already solved
- command branching coverage for interactive game flow
- graceful startup/shutdown path for the console entrypoint

## Running the Tests

From the solution root, run:

```bash
dotnet test
```

This command executes the full xUnit suite and validates the current implementation against the scenarios listed above.
