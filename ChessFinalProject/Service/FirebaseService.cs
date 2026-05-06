
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChessFinalProject.Models;

namespace ChessFinalProject.Service
{
    public class FirebaseService
    {
        private readonly FirebaseClient _client;
        private IDisposable _subscription;

        public FirebaseService()
        {
            _client = new FirebaseClient("https://chessfinalproject-66573-default-rtdb.firebaseio.com");
        }

        // Call this to start a new game — share gameId with your opponent
        public string CreateGameId() => Guid.NewGuid().ToString("N")[..8];
        public async Task SaveMoveAsync(string gameId, Dictionary<string, string> board, string whoseTurn)
        {
            await _client
                .Child("games")
                .Child(gameId)
                .PutAsync(new Models.GameState
                {
                    Board = board,
                    WhoseTurn = whoseTurn   // "white" or "black"
                });
        }
        public void ListenToGame(string gameId, Action<GameState> onChanged)
        {
            _subscription = _client
                .Child("games")
                .Child(gameId)
                .AsObservable<GameState>()
                .Subscribe(item =>
                {
                    if (item.Object != null)
                        onChanged(item.Object);  // fires on both phones whenever anything changes
                });
        }
    }
}
