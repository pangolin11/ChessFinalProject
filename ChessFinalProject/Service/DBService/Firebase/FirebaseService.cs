// Services/FirebaseService.cs
using ChessFinalProject.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using System.Collections.ObjectModel; // If you want to use ObservableCollection
using System.Reactive.Linq; // Required for Observable functionality
   

namespace ChessFinalProject.Service.DBService.Firebase;

public class FirebaseService
{
    private readonly FirebaseClient firebaseClient;
    private const string FirebaseDatabaseUrl = "https://chessfinalproject-66573-default-rtdb.firebaseio.com"; // Replace with your DB URL
    // Your Database URL: https://chessfinalproject-66573-default-rtdb.firebaseio.com

    public FirebaseService()
    {
        // Initialize Firebase Client
        firebaseClient = new FirebaseClient(FirebaseDatabaseUrl);
    }

    // --- Writing Data (Making a Move) ---
    public async Task SendMove(string gameId, ChessMove move)
    {
        try
        {
            // Pushes a new ChessMove object to a list of moves under a specific gameId
            // This will generate a unique key for each move.
            await firebaseClient
                .Child("games")      // Top-level "games" node
                .Child(gameId)       // Specific game instance (e.g., "game_123")
                .Child("moves")      // A list of moves for this game
                .PostAsync(move);    // Post the move object

            Console.WriteLine($"Move sent successfully for Game ID: {gameId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending move: {ex.Message}");
        }
    }

    // --- Reading Data (Listening for Opponent's Moves) ---
    public IObservable<FirebaseEvent<ChessMove>> ListenForMoves(string gameId)
    {
        // Listen for all changes (added, changed, removed) to the "moves" child
        return firebaseClient
            .Child("games")
            .Child(gameId)
            .Child("moves")
            .AsObservable<ChessMove>(); // This gives you an observable stream of Firebase events
    }

    // --- Listening for entire GameState (Alternative/Complementary) ---
    public IObservable<FirebaseObject<GameState>> ListenForGameState(string gameId)
    {
        // Listen for changes to the entire GameState object
        return firebaseClient
            .Child("games")
            .Child(gameId)
            .AsObservable<GameState>(); // You might store GameState directly at the gameId level
    }

    // --- Initializing a new Game State (example) ---
    public async Task InitializeGame(GameState initialState)
    {
        try
        {
            await firebaseClient
                .Child("games")
                .Child(initialState.GameId)
                .PutAsync(initialState); // Use PutAsync to set the initial state or overwrite
            Console.WriteLine($"Game {initialState.GameId} initialized.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing game: {ex.Message}");
        }
    }
    //not good, use later
    /*  public async Task<GameState?> FindOrCreateGame(string currentPlayerId)
      {
          Console.WriteLine($"Player {currentPlayerId} is looking for or creating a game...");

          // 1. Try to find an open game (status "waiting")
          // We limit to first 10 to avoid excessive data transfer if many games exist.
          var openGames = await firebaseClient
              .Child("games")
              .OrderBy("Status") // Ordering might help in finding "waiting" games faster
              .LimitToFirst(10) // Limit the search
              .OnceAsync<GameState>();

          foreach (var gameSnapshot in openGames)
          {
              var game = gameSnapshot.Object;
              // Check if the game is actually waiting and has space for a second player
              if (game.Status == "waiting" && string.IsNullOrEmpty(game.BlackPlayerId))
              {
                  // Attempt to join this game using a transaction to prevent race conditions
                  var joinedGame = await TryJoinGame(game.GameId, currentPlayerId);
                  if (joinedGame != null)
                  {
                      Console.WriteLine($"Joined existing game: {joinedGame.GameId}");
                      return joinedGame;
                  }
              }
              // Also handle if the player is already white and waiting in this game
              else if (game.Status == "waiting" && game.WhitePlayerId == currentPlayerId)
              {
                  Console.WriteLine($"Rejoined own waiting game: {game.GameId}");
                  return game;
              }
          }

          // 2. If no open game found (or couldn't join one), create a new game
          Console.WriteLine("No open games found or could be joined. Creating a new game...");
          return await CreateNewGame(currentPlayerId);
      }*/

    // --- Helper Method: Try to Join an Existing Game using a Transaction ---
    // --- Helper Method: Try to Join an Existing Game using a Transaction ---
    // Services/FirebaseService.cs (Revisiting TryJoinGame)
    // ... (other parts of FirebaseService) ...

    // --- Helper Method: Try to Join an Existing Game using a Transaction ---
    public async Task<GameState?> JoinGame(string gameId, string blackPlayerId)
    {
        try
        {
            // 1. Read the current game state
            var game = await firebaseClient
                .Child("games")
                .Child(gameId)
                .OnceSingleAsync<GameState>();

            // 2. Perform checks based on the read state
            if (game == null)
            {
                Console.WriteLine($"Attempted to join game {gameId}, but it was not found.");
                throw new Exception("Game not found.");
            }

            // Ensure the GameId property is set from the Firebase key for consistency
            game.GameId = gameId;

            if (game.Status != "waiting")
            {
                Console.WriteLine($"Attempted to join game {gameId}, but it already started or is finished.");
                throw new Exception("Game already started or finished.");
            }

            if (!string.IsNullOrEmpty(game.BlackPlayerId))
            {
                Console.WriteLine($"Attempted to join game {gameId}, but it already has two players.");
                throw new Exception("Game already has two players.");
            }

            // 3. Update game state locally
            game.BlackPlayerId = blackPlayerId;
            game.Status = "playing";
            game.LastUpdated = DateTime.UtcNow;

            // 4. Write the updated game state back to Firebase
            // WARNING: This is the point of the potential race condition.
            // If another player simultaneously passes the checks and writes before this PutAsync completes,
            // one of the updates will overwrite the other without explicit error or notification to the overwritten client.
            await firebaseClient
                .Child("games")
                .Child(gameId)
                .PutAsync(game);

            Console.WriteLine($"Player {blackPlayerId} successfully joined game {gameId}.");
            return game;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error joining game {gameId}: {ex.Message}");
            // Depending on your error handling strategy, you might return null or re-throw
            throw;
        }
    }

    // ... (rest of FirebaseService) ...



    // --- Helper Method: Create a New Game ---
    public async Task<string> CreateGame(string whitePlayerId)
    {
        // Generate a short, unique game ID (e.g., first 8 characters of a GUID)
        var gameId = Guid.NewGuid().ToString("N")[..8];

        var newGame = new GameState
        {
            GameId = gameId,
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = null, // Initially null, waiting for second player
            CurrentTurnPlayerId = whitePlayerId, // White typically starts
            Status = "waiting",
            LastUpdated = DateTime.UtcNow,
            BoardPieces = new()
{
    { "A8", "black_rook.png" },
    { "B8", "black_knight.png" },
    { "C8", "black_bishop.png" },
    { "D8", "black_queen.png" },
    { "E8", "black_king.png" },
    { "F8", "black_bishop.png" },
    { "G8", "black_knight.png" },
    { "H8", "black_rook.png" },

    { "A7", "black_pawn.png" },
    { "B7", "black_pawn.png" },
    { "C7", "black_pawn.png" },
    { "D7", "black_pawn.png" },
    { "E7", "black_pawn.png" },
    { "F7", "black_pawn.png" },
    { "G7", "black_pawn.png" },
    { "H7", "black_pawn.png" },

    { "A6", "" },
    { "B6", "" },
    { "C6", "" },
    { "D6", "" },
    { "E6", "" },
    { "F6", "" },
    { "G6", "" },
    { "H6", "" },

    { "A5", "" },
    { "B5", "" },
    { "C5", "" },
    { "D5", "" },
    { "E5", "" },
    { "F5", "" },
    { "G5", "" },
    { "H5", "" },

    { "A4", "" },
    { "B4", "" },
    { "C4", "" },
    { "D4", "" },
    { "E4", "" },
    { "F4", "" },
    { "G4", "" },
    { "H4", "" },

    { "A3", "" },
    { "B3", "" },
    { "C3", "" },
    { "D3", "" },
    { "E3", "" },
    { "F3", "" },
    { "G3", "" },
    { "H3", "" },

    { "A2", "white_pawn.png" },
    { "B2", "white_pawn.png" },
    { "C2", "white_pawn.png" },
    { "D2", "white_pawn.png" },
    { "E2", "white_pawn.png" },
    { "F2", "white_pawn.png" },
    { "G2", "white_pawn.png" },
    { "H2", "white_pawn.png" },

    { "A1", "white_rook.png" },
    { "B1", "white_knight.png" },
    { "C1", "white_bishop.png" },
    { "D1", "white_queen.png" },
    { "E1", "white_king.png" },
    { "F1", "white_bishop.png" },
    { "G1", "white_knight.png" },
    { "H1", "white_rook.png" }
}
        };

        try
        {
            // Use PutAsync to create the game at a specific path
            await firebaseClient
                .Child("games")
                .Child(gameId)
                .PutAsync(newGame);

            Console.WriteLine($"Game created with ID: {gameId} by {whitePlayerId}");
            return gameId; // Return this code for Player 1 to share
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating game: {ex.Message}");
            throw; // Re-throw to indicate creation failed
        }
    }

    // --- General Update Method (useful for updating turn, FEN, status after moves) ---
    public async Task UpdateGameState(GameState state)
    {
        try
        {
            await firebaseClient
                .Child("games")
                .Child(state.GameId)
                .PutAsync(state); // Overwrites the entire game state at this path
            Console.WriteLine($"Game {state.GameId} state updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating game state {state.GameId}: {ex.Message}");
            throw; // Re-throw to indicate update failed
        }
    }
}
