# Sudoku App - Development Roadmap & Backlog

This document outlines the phased delivery plan for the C# Sudoku console application, structured around your MVP implementation milestones and future post-MVP enhancements.

---

## MVP

### CORE-01: Fulfil functional requirements
* **Category:** `Feature` / `Unit Tests`
* **Status:** Priority 1
* **Description:** Deliver the complete set of core features required to make the Sudoku application fully playable, robust, and visually polished according to the specification.
* **Acceptance Criteria / Implementation Breakdown:**
  1. **Setup:** Basic board with hardcoded static layout, user inputs (`[Cell] [Number]`, `[Cell] clear`, `hint`, `check`, `quit`), and command logs[cite: 2, 3].
  2. **Validation:** Comprehensive rule checking for duplicates in rows, columns, and 3x3 subgrids, plus blocking invalid moves (pre-filled cells, out-of-range values)[cite: 2, 3].
  3. **Logs & Feedback:** Status logging, history tracking, and clean messaging for errors and accepted moves[cite: 2, 3].
  4. **Display & UI:** Proper terminal borders, clean spacing, font colors, and highlighted cell rendering[cite: 1, 3].
  5. **Puzzle Generation Enhancement:** Transition from static boards to dynamic generation ensuring exactly 30 pre-filled cells with randomized numbers and positions[cite: 2, 3].
  6. **Enhanced Hint Function:** Backed by a pre-calculated master solution grid to catch hidden drift errors before revealing new moves[cite: 3].
  7. **Smooth Gameplay Experience:** Graceful handling of game completion (`You have successfully completed...`) with prompts to start a new game[cite: 2, 3].

### PROD-01: Production-ready with appropriate documentation
* **Category:** `DevOps` / `Engineering Standards`
* **Status:** Priority 1
* **Description:** Establish production readiness through comprehensive documentation, consistent C# project properties, and a CI/CD pipeline template simulating staging and production deployments.
* **Acceptance Criteria:**
  * Comprehensive `README.md` with environment prerequisites, build instructions, design assumptions, and execution guidelines[cite: 2, 3].
  * Base configuration files (e.g., `Directory.Build.props`) for uniform C# compiler settings[cite: 3].
  * Foundational CI/CD YAML configuration (`azure-pipelines.yml` or GitHub Actions) demonstrating automated build, test, and release flows for staging and production targets[cite: 3].

---

## Post-MVP
*Objective: Demonstrate technical scalability, advanced UX design thinking, and professional engineering capabilities beyond baseline requirements.*

### GAME-01: Implement Undo & Redo Stacks
* **Category:** `Feature` (Post-MVP)
* **Description:** Add support for state history using `System.Collections.Generic` (`Stack<T>`) so players can step backward and forward through their moves[cite: 3].

### GAME-02: Game Modes: 3 levels of Difficulty
* **Category:** `Feature` (Post-MVP)
* **Description:** Easy, normal and hard mode with 30 pre-filled cells but distributed differently.

### GAME-03: Native Pencil Marks (Candidate System)
* **Category:** `Feature` (Post-MVP)
* **Description:** Allow users to toggle candidate/pencil-mark numbers inside empty cells without committing to a primary move[cite: 3].

### UI-01: Interactive Arrow-Key Navigation (`System.Console`)
* **Category:** `Enhancement` (Post-MVP)
* **Description:** Upgrade the CLI from pure text-command entry to an interactive grid view where users can navigate cells using arrow keys[cite: 3].

### PERS-01: Native Save, Load, & Statistics (`System.Text.Json`)
* **Category:** `Feature` (Post-MVP)
* **Description:** Enable users to persist their active game session to disk and track game history using `System.Text.Json`[cite: 3].