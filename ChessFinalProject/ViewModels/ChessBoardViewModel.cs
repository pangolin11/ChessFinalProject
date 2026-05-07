using ChessFinalProject.Models;
using ChessFinalProject.Service.DBService.Firebase;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace ChessFinalProject.ViewModels
{
    public class ChessBoardViewModel : ViewModelBase
    {
        private Dictionary<string, string> pieceImages = new Dictionary<string, string>(64);
        private FirebaseService _firebaseService;
        private GameState _currentLocalGameState; // Local copy of the full game state

        public Dictionary<string, string> PieceImages
        {
            get { return pieceImages; }
            set
            {
                if (pieceImages != value)
                {
                    pieceImages = value;
                    OnPropertyChanged();
                }
            }
        }
        public ChessBoardViewModel()
        {
/*           InitializeGameListener(_currentLocalGameState.GameId);
*/
        }

        private void InitializeGameListener(string gameId)
        {
            _firebaseService.ListenForGameState(gameId)
                .Subscribe(firebaseObject =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (firebaseObject != null && firebaseObject.Object != null)
                        {
                            _currentLocalGameState = firebaseObject.Object; // Keep a local copy of the full state
                            pieceImages = _currentLocalGameState.BoardPieces; // Update the piece images based on the current game state
                        }
                        else
                        {
                            Console.WriteLine("GameState object was null in the update.");
                        }
                    });
                },
                error =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Console.WriteLine($"Error listening for game state: {error.Message}");
                        // Handle error, e.g., show an alert
                    });
                },
                () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Console.WriteLine("Game state listener completed.");
                    });
                });
        }
    }
}
