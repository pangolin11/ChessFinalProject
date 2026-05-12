
using ChessFinalProject.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
   

namespace ChessFinalProject.Service.DBService.Firebase;

public class FirebaseService : IGameService
{
    private readonly FirebaseClient firebaseClient;
    private const string FirebaseDatabaseUrl = "https://chessfinalproject-66573-default-rtdb.firebaseio.com"; // Replace with your DB URL
    // Your Database URL: https://chessfinalproject-66573-default-rtdb.firebaseio.com
    public FirebaseService()
    {
        // Initialize Firebase Client
        firebaseClient = new FirebaseClient(FirebaseDatabaseUrl);
    }
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
    public IObservable<FirebaseEvent<ChessMove>> ListenForMoves(string gameId)
    {
       
        return firebaseClient
            .Child("games")
            .Child(gameId)
            .Child("moves")
            .AsObservable<ChessMove>(); 
    }
    public IObservable<FirebaseObject<object>> ListenForGameState(string gameId)
    {
       
        return firebaseClient
            .Child("games")
            .Child(gameId)
            .AsObservable<object>(); 
    }
    public async Task InitializeGame(GameState initialState)
    {
        try
        {
            await firebaseClient
                .Child("games")
                .Child(initialState.GameId)
                .PutAsync(initialState); 
            Console.WriteLine($"Game {initialState.GameId} initialized.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing game: {ex.Message}");
        }
    }   
    public async Task<string> JoinGame(string gameId, string blackPlayerId)
    {
        try
        {
            var game = await firebaseClient
                .Child("games")
                .Child(gameId)
                .OnceSingleAsync<GameState>();
            if (blackPlayerId == game.WhitePlayerId|| blackPlayerId == game.BlackPlayerId)
            {
                return game.GameId; 
            }
            else
            {
                game.BlackPlayerId = blackPlayerId;
                game.Status = "playing";
                await firebaseClient
                    .Child("games")
                    .Child(gameId)
                    .PutAsync(game);

                Console.WriteLine($"Player {blackPlayerId} successfully joined game {gameId}.");
                return game.GameId;
            }
          
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error joining game {gameId}: {ex.Message}");
            throw;
        }
    }
    public async Task<string> CreateGame(string whitePlayerId)
    {
        // Generate a short, unique game ID (e.g., first 8 characters of a GUID)
        var gameId = Guid.NewGuid().ToString("N")[..8];

        var newGame = new GameState
        {
            GameId = gameId,
            WhitePlayerId = whitePlayerId,
            BlackPlayerId = null,
            CurrentTurnPlayerId = whitePlayerId, 
            Status = "waiting",
            Board = new()
{
    { "A8", "blackrook.png" },
    { "B8", "blackknight.png" },
    { "C8", "blackbishop.png" },
    { "D8", "blackqueen.png" },
    { "E8", "blackking.png" },
    { "F8", "blackbishop.png" },
    { "G8", "blackknight.png" },
    { "H8", "blackrook.png" },

    { "A7", "blackpawn.png" },
    { "B7", "blackpawn.png" },
    { "C7", "blackpawn.png" },
    { "D7", "blackpawn.png" },
    { "E7", "blackpawn.png" },
    { "F7", "blackpawn.png" },
    { "G7", "blackpawn.png" },
    { "H7", "blackpawn.png" },

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

    { "A2", "whitepawn.png" },
    { "B2", "whitepawn.png" },
    { "C2", "whitepawn.png" },
    { "D2", "whitepawn.png" },
    { "E2", "whitepawn.png" },
    { "F2", "whitepawn.png" },
    { "G2", "whitepawn.png" },
    { "H2", "whitepawn.png" },

    { "A1", "whiterook.png" },
    { "B1", "whiteknight.png" },
    { "C1", "whitebishop.png" },
    { "D1", "whitequeen.png" },
    { "E1", "whiteking.png" },
    { "F1", "whitebishop.png" },
    { "G1", "whiteknight.png" },
    { "H1", "whiterook.png" }
}
        };

        try
        {
           
            await firebaseClient
                .Child("games")
                .Child(gameId)
                .PutAsync(newGame);

            Console.WriteLine($"Game created with ID: {gameId} by {whitePlayerId}");
            return gameId; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating game: {ex.Message}");
            throw; 
        }
    }
    public async Task UpdateGameState(GameState state)
    {
        try
        {
            await firebaseClient
                .Child("games")
                .Child(state.GameId)
                .PutAsync(state); 
            Console.WriteLine($"Game {state.GameId} state updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating game state {state.GameId}: {ex.Message}");
            throw; 
        }
    }
    public async Task<string> FindGame(string playerId)
    {
        foreach (var game in await firebaseClient.Child("games").OnceAsync<GameState>())
        {
            if (game.Object.WhitePlayerId != null && game.Object.WhitePlayerId == playerId)
            {
                return await JoinGame(game.Object.GameId, playerId);
            }
            else if(game.Object.BlackPlayerId == null)
            {
                return await JoinGame(game.Object.GameId, playerId);
            }
            else if(game.Object.BlackPlayerId == playerId)
            {
                return await JoinGame(game.Object.GameId, playerId);
            }
        }
        return await CreateGame(playerId);
    }
    public async Task<GameState?> GetGameState(string gameId)
    {
        try
        {
            var gameState = await firebaseClient
                .Child("games")
                .Child(gameId)
                .OnceSingleAsync<GameState>();
            Console.WriteLine($"Game state retrieved for Game ID: {gameId}");
            return gameState; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving game state for Game ID {gameId}: {ex.Message}");
            throw; 
        }
    }
    public async Task SendToFirebase(GameState gameState)
    {
        try
        {
            await firebaseClient
                .Child("games")
                .Child(gameState.GameId)
                .PutAsync(gameState); 
            Console.WriteLine($"Game state sent to Firebase for Game ID: {gameState.GameId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending game state to Firebase for Game ID {gameState.GameId}: {ex.Message}");
            throw; 
        }
    }

}
