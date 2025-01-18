# Cquence Card Game API

## Overview

Cquence is a level-based card game that provides APIs for managing game sessions. The API allows users to start games, retrieve game states, and submit sequences for evaluation. The game ensures data persistence through XML files and supports progression across multiple levels.

## Demo

https://github.com/user-attachments/assets/e730dedc-99e8-425a-91ad-e6aad3c3cd26

## Modules and Functionalities

### 1. GameController

The **GameController** serves as the API layer for all game-related operations. It exposes the following endpoints:

#### a. StartGame
- **Route:** `POST /api/Game/start`
- **Input:** `userId` (string)
- **Output:** Returns the initial game state, including the game ID, user ID, and the first level's sequence.
- **Functionality:** 
  - Initializes a new game session for the user.
  - Calls `IGameService.StartGameAsync` to create and store the initial game state.

#### b. GetGameState
- **Route:** `GET /api/Game/{gameId}/state`
- **Input:** `gameId` (string)
- **Output:** Returns the current state of the game.
- **Functionality:** 
  - Fetches the current state of the game using the `gameId`.
  - Returns a `404` response if the game is not found.

#### c. SubmitSequence
- **Route:** `POST /api/Game/{gameId}/submit`
- **Input:** `gameId` (string), `userSequence` (list of strings)
- **Output:** Returns the updated game state or error details for invalid submissions.
- **Functionality:** 
  - Evaluates the user-submitted sequence.
  - Updates the game state for correct submissions or records errors for incorrect ones.
  - Handles validation and exception scenarios gracefully.

### 2. GameService

The **GameService** implements the core business logic for game operations.

#### a. StartGameAsync
- **Input:** `userId` (string)
- **Output:** Returns a `GameState` object initialized for the user.
- **Functionality:** 
  - Generates a new game ID.
  - Initializes the game state with level 1 and a randomly generated sequence.
  - Persists the state using `IGameRepository.SaveGameStateAsync`.

#### b. GetGameStateAsync
- **Input:** `gameId` (string)
- **Output:** Returns the `GameState` object or throws an error if the game is not found.
- **Functionality:** 
  - Retrieves the game state using `IGameRepository.GetGameStateAsync`.
  - Validates the existence of the game state.

#### c. SubmitSequenceAsync
- **Input:** `gameId` (string), `userSequence` (list of strings)
- **Output:** Updated `GameState` object or error message.
- **Functionality:** 
  - Validates the user's sequence.
  - Updates the game state for correct submissions (progress to the next level).
  - Records incorrect submissions and logs details.
  - Uses a random sequence generator to create new levels.

### 3. GameRepository

The **GameRepository** manages data persistence for game states using XML files.

#### a. GetGameStateAsync
- **Input:** `gameId` (string)
- **Output:** `GameState` object or `null` if the game is not found.
- **Functionality:** 
  - Reads game state data from an XML file.
  - Logs warnings for missing files.

#### b. SaveGameStateAsync
- **Input:** `gameState` (GameState object)
- **Output:** None
- **Functionality:** 
  - Writes game state data to an XML file.
  - Ensures the directory structure is created if missing.

## Data Flow and Interactions

<img width="439" alt="Image" src="https://github.com/user-attachments/assets/ec2945b3-1f84-446a-b946-ef731caafed7" />

1. **Start Game:**
   - User invokes `POST /api/Game/start` with `userId`.
   - `GameController` calls `StartGameAsync`.
   - `GameService` initializes and saves the game state.
   - The response includes game details and the initial sequence.

2. **Retrieve Game State:**
   - User invokes `GET /api/Game/{gameId}/state`.
   - `GameController` calls `GetGameStateAsync`.
   - `GameService` fetches the state from `GameRepository`.
   - The response includes the current game state.

3. **Submit Sequence:**
   - User invokes `POST /api/Game/{gameId}/submit` with `userSequence`.
   - `GameController` calls `SubmitSequenceAsync`.
   - `GameService` evaluates the sequence, updates state, and saves it.
   - The response includes updated state or error details.

## Error Handling

1. **Game Not Found:**
   - Returns a `404` response for invalid `gameId` during state retrieval.

2. **Invalid Sequence:**
   - Logs the incorrect submission.
   - Includes detailed error messages with user and correct sequences.

## Logging

- Detailed logging for game state retrieval, submissions, and errors.
- Logs include:
  - `gameId`
  - `userId`
  - Sequences
  - Error messages

## Getting Started

### Prerequisites
- .NET Core
- Visual Studio Code
- GitHub repository access
