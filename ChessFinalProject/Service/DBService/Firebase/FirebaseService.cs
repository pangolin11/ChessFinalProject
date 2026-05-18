
using ChessFinalProject.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Java.Nio.Channels;


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
    public IObservable<FirebaseObject<GameState>> ListenForGameState(string gameId)
    {
        return firebaseClient
            .Child("games")
            .Child(gameId)
            .AsObservable<GameState>();
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
                    .Child(gameId)
                    .PutAsync(game);
                game.Status = "Playing";
                await firebaseClient
                    .Child("games")
                    .Child(gameId)
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
            IsWhiteTurn = true,
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
            var gameState = await firebaseClient
                .Child("games")
                .Child(game.Key)
                .Child(game.Key)
                .OnceSingleAsync<GameState>();
            if (gameState.WhitePlayerId != null && gameState.WhitePlayerId == playerId)
            {
                return await JoinGame(gameState.GameId, playerId);
            }
            else if(gameState.BlackPlayerId == null)
            {
                return await JoinGame(gameState.GameId, playerId);
            }
            else if(gameState.BlackPlayerId == playerId)
            {
                return await JoinGame(gameState.GameId, playerId);
            }
        }
        return await CreateGame(playerId);
    }
    public async Task<GameState?> GetGameState(string gameId)
    {
            var gameState = await firebaseClient
                .Child("games")
                .Child(gameId)
                .Child(gameId)
                .OnceSingleAsync<GameState>();
            return gameState; 
    }
    public async Task SendToFirebase(string squareFrom, string squareTo, string gameid)
    {

            var gameState = await GetGameState(gameid);
        if (gameState.IsWhiteTurn)
            gameState.IsWhiteTurn = false;

        else
            gameState.IsWhiteTurn = true;
        gameState?.Board[squareTo] = gameState.Board[squareFrom];
        gameState?.Board[squareFrom] = "";

        gameState?.squareFrom = squareFrom;
            gameState?.squareTo = squareTo;
        gameState.CanBeChanged = true;
            await firebaseClient
                .Child("games")
                .Child(gameState?.GameId)
                .Child(gameState?.GameId)
                .PutAsync(gameState);
        gameState?.squareFrom = null;
        gameState?.squareTo = null;
        gameState.CanBeChanged = false;
        await firebaseClient
               .Child("games")
               .Child(gameState?.GameId)
               .Child(gameState?.GameId)
               .PutAsync(gameState);

    }
    public async Task EndGame(GameState currentLocalGameState, string gameId, string kingType)
    {
        var newGameState = new GameState
        {
            GameId = currentLocalGameState.GameId,
            WhitePlayerId = currentLocalGameState.WhitePlayerId,
            BlackPlayerId = currentLocalGameState.BlackPlayerId,
            CurrentTurnPlayerId = currentLocalGameState.CurrentTurnPlayerId,
            Status = currentLocalGameState.Status, // We will update this below
            squareFrom = currentLocalGameState.squareFrom,
            squareTo = currentLocalGameState.squareTo,
            IsWhiteTurn = currentLocalGameState.IsWhiteTurn,
            UnImportant = currentLocalGameState.UnImportant,
            CanBeChanged = currentLocalGameState.CanBeChanged
        };
        if (kingType == "whiteking.png")
        {
            newGameState.WinningPlayerId = currentLocalGameState.BlackPlayerId;
            newGameState.Status = "victory";
            newGameState.LosingPlayerId = currentLocalGameState.WhitePlayerId;
            currentLocalGameState.Status = "black wins";
        }
        else
        {
            newGameState.WinningPlayerId = currentLocalGameState.WhitePlayerId;
            newGameState.Status = "victory";
            newGameState.LosingPlayerId = currentLocalGameState.BlackPlayerId;

            currentLocalGameState.Status = "white wins";
        }
        //save game to firebase
        await firebaseClient
            .Child("games")
            .Child(gameId)
            .Child(gameId)
            .PutAsync(currentLocalGameState);
        currentLocalGameState.UnImportant = !currentLocalGameState.UnImportant;
        await firebaseClient
            .Child("games")
            .Child(gameId)
            .Child(gameId)
            .PutAsync(currentLocalGameState);
        await firebaseClient
            .Child("games")
            .Child(gameId)
            .Child(gameId)
            .DeleteAsync();
        //save game as victory for victorious player
        await firebaseClient
             .Child("users")
             .Child(newGameState.WinningPlayerId)
             .Child(newGameState.GameId)
             .PutAsync(newGameState);
        newGameState.Status = "defeat";
        //save game as loss for defeated player
        await firebaseClient
            .Child("users")
             .Child(newGameState.LosingPlayerId)
             .Child(newGameState.GameId)
             .PutAsync(newGameState);
       
    }

    public async Task SendTime(GameState currentLocalGameState, string time, string kingType)
    {
            if (kingType == "whiteking.png")
            {
                var asd = new Dictionary<string, string>();
                asd.Add("WhiteTime", time);
                currentLocalGameState.WhiteTime = time;
                await firebaseClient
                   .Child("games")
                   .Child(currentLocalGameState.GameId)
                   .Child(currentLocalGameState.GameId)
                   .PatchAsync(asd);
        }
            else
            {
                currentLocalGameState.BlackTime = time;
            }
            
            currentLocalGameState.UnImportant = !currentLocalGameState.UnImportant;
         if (kingType == "whiteking.png")
        {
            currentLocalGameState.WhiteTime = time;
        }
        else
        {
            currentLocalGameState.BlackTime = time;
        }
    }
}
