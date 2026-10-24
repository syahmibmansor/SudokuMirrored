# Sudoku App - Development Backlog & Tickets

This document outlines the incremental roadmap and ticket tracking for evolving the C# Sudoku console application from an MVP to a production-grade release.

---

## Epic 1: Core Specification & Compliance (MVP Solidification)

### CORE-01: Standardize Command Parsing & Input Validation
* **Category:** `Refactor` / `Bugfix`
* **Description:** Align command parsing with the exact specification in `Requirements.md`. Ensure robust handling of user inputs such as `A3 4`, `C5 clear`, `hint`, `check`, and `quit`.
* **Acceptance Criteria:**
  * Commands are case-insensitive or cleanly handled.
  * Invalid cell formats (e.g., `Z9 4` or `A10 2`) display clean error messages without crashing.
  * Pre-filled cells are protected from modification with the exact message: `Invalid move. [Cell] is pre-filled.`[cite: 2]

### CORE-02: Comprehensive Board Validation & Rule Enforcement
* **Category:** `Feature` / `Validation`
* **Description:** Ensure the `check` command accurately diagnoses rule violations and pinpoints whether a duplicate exists in a row, column, or 3x3 subgrid.
* **Acceptance Criteria:**
  * Detects duplicate numbers in rows, columns, and 3x3 subgrids separately[cite: 2].
  * Outputs descriptive messages matching the spec (e.g., `Number 3 already exists in Row A.`, `Number 5 already exists in Column 1.`, `Number 8 already exists in the same 3×3 subgrid.`)[cite: 2].
  * Unit tests thoroughly verify all violation scenarios.

---

## Epic 2: Advanced Gameplay Features

### GAME-01: Implement Undo & Redo Stacks
* **Category:** `Feature`
* **Description:** Add support for state history using `System.Collections.Generic` (`Stack<T>`) so players can step backward and forward through their moves.
* **Acceptance Criteria:**
  * Add `undo` and `redo` command handlers to the CLI loop.
  * Ensure pre-filled cells and immutable board states cannot be undone past the initial puzzle setup.
  * Unit tests verify state rollback correctness.

### GAME-02: Native Pencil Marks (Candidate System)
* **Category:** `Feature`
* **Description:** Allow users to toggle candidate/pencil-mark numbers inside empty cells without committing to a primary move.
* **Acceptance Criteria:**
  * Support syntax for toggling pencil marks (e.g., `A3+4` to add candidate 4).
  * Render candidate marks cleanly in the console view without disrupting the primary grid layout.

---

## Epic 3: Interactive UI & Persistence

### UI-01: Interactive Arrow-Key Navigation (`System.Console`)
* **Category:** `Enhancement`
* **Description:** Upgrade the CLI from pure text-command entry to an interactive grid view where users can navigate cells using arrow keys.
* **Acceptance Criteria:**
  * Use raw `System.Console` key reads (`Console.KeyAvailable`, `ConsoleKey.LeftArrow`, etc.) to highlight the active cell.
  * Allow direct number entry or clearing while navigating via arrow keys.
  * Maintain fallback command text input for non-interactive automation or testing.

### PERS-01: Native Save, Load, & Statistics (`System.Text.Json`)
* **Category:** `Feature`
* **Description:** Enable users to persist their active game session to disk and track game history using `System.Text.Json`.
* **Acceptance Criteria:**
  * Add `save [filename]` and `load [filename]` commands.
  * Automatically track and persist player statistics (games played, wins, best times) to a local JSON file.
  * Handle IO exceptions gracefully if files are missing or corrupted.

---

## Epic 4: Robust Puzzle Generation & Production Engineering

### PROD-01: Custom Backtracking / DLX Generator
* **Category:** `Core Logic`
* **Description:** Replace hardcoded or static boards with a dynamic puzzle generator that guarantees a unique solution and exactly 30 pre-filled numbers.
* **Acceptance Criteria:**
  * Generator implements a robust backtracking algorithm (or Dancing Links / DLX) to construct valid, solvable Sudoku grids.
  * Guarantees that the initial puzzle state presents precisely 30 pre-filled cells denoted by `_`[cite: 2].

### PROD-02: Production CI/CD Pipeline & Repository Hardening
* **Category:** `DevOps`
* **Description:** Finalize the project structure with professional engineering artifacts, automated testing pipelines, and code quality enforcers.
* **Acceptance Criteria:**
  * Add GitHub Actions (`.github/workflows/ci.yml`) or Azure DevOps (`azure-pipelines.yml`) pipeline to restore, build, and run unit tests automatically on every push/PR.
  * Include a root `Directory.Build.props` and `.editorconfig` for consistent C# styling.
  * Update `README.md` with comprehensive setup instructions, environment prerequisites, and gameplay guidelines[cite: 2].