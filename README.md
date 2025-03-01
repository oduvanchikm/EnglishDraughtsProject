# Main Project Structure
The project consists of two parts:

1) EnglishDraughtsProject – the main application with game logic and UI.

2) EnglishDraughtsProjectTests – tests to verify the code's functionality.

## EnglishDraughtsProject

### Models

The Models folder contains the core classes that define the structure of the game, including the board, cells, and game pieces.

- Board (Game Field)

The Board class represents the 8×8 game board. It consists of a two-dimensional array of Cell objects, each representing a position on the board.

The board is initialized with alternating black and white squares.
The first three rows contain Black Checkers, while the last three contain White Checkers.
The middle two rows start empty.

- Cell (Individual Square)

Each square on the board is represented by the Cell class. It contains:

X, Y Coordinates – The position of the square on the board.

Value – The type of piece on the square (Empty, WhiteChecker, BlackChecker, WhiteKing, BlackKing).

- CellValueEnum (Checker Types)

A simple enumeration defining possible values a Cell can have:

Empty – No piece.
WhiteChecker – A white piece.
BlackChecker – A black piece.
WhiteKing – A white King.
BlackKing – A black King.

- OpenAiResponse (AI Communication)

Handles responses from an AI service (possibly OpenAI) if integrated for move suggestions.

Choices[] – An array of possible AI-generated responses.
Message.Content – Contains the actual AI-generated text.

### Services (Game Logic Management)

The Services folder contains classes responsible for handling the application's core logic. These services manage AI-based move suggestions and gameplay mechanics.

- AiService (AI Integration for Move Suggestions)

The AiService class communicates with an AI service to provide move recommendations based on the current board state.

1) Serializes the board state into a structured format.
2) Sends a request to an AI model for the best possible move.
3) Receives and processes the AI response, returning a suggested move.

- GameLogicService (Game Mechanics Controller)

The GameLogicService class manages the rules and mechanics of the game.

1) Tracks the board state and the current player's turn.
2) Validates moves to ensure they follow the rules of English Draughts.
3) Handles captures (jumping over opponent pieces).
4) Promotes checkers to kings when they reach the opponent’s last row.
5) Uses AiService to provide hints for the best moves.

### Views (User Interface Representation)

The Views folder is responsible for managing the user interface (UI) of the application. It contains .axaml files and corresponding logic to render the checkers board and interactive components.

- BoardView (Game Board UI)

The BoardView class represents the main board interface where the game is played.

- HintDialog (AI Move Suggestion Dialog)

The HintDialog window displays hints from the AI, guiding players to make optimal moves.

## EnglishDraughtsProjectTests

### UnitTest1 (Testing AiService)

This class tests the interaction with the AI service, which suggests the best moves in the game.

Test1 - Verifies a successful AI response.

Test2 - Checks the handling of a server error.

### UnitTest2 (Testing GameLogicService)

This class tests the correctness of moves and compliance with the game rules.

Test1 - A regular checker move forward.

Test2 - Jumping over an opponent's checker.

Test3 - Attempting to move to an occupied square.