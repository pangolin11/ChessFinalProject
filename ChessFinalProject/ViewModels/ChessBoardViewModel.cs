using ChessFinalProject.Models;
using ChessFinalProject.Service.DBService.Firebase;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;

namespace ChessFinalProject.ViewModels
{
    public partial class ChessBoardViewModel : ViewModelBase
    {
        private FirebaseService _firebaseService;
        private GameState _currentLocalGameState; // Local copy of the full game state
        public ObservableCollection<ChessSquare> Board { get; } = new();

        [ObservableProperty]
        private string selectedSquare;



        public ChessBoardViewModel()
        {
           /*InitializeGameListener(_currentLocalGameState.GameId);       */

        }

        public async Task InitializeBoardAsync(int batchSize = 8, int delayMs = 16)
        {
            Board.Clear();

            string[] files = { "A", "B", "C", "D", "E", "F", "G", "H" };

            var buffer = new List<ChessSquare>(batchSize);

            for (int row = 8; row >= 1; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (files[col] + row == "E1")
                    {
                        buffer.Add(new ChessSquare
                        {
                            Name = files[col] + row,
                            IsWhite = (row + col) % 2 == 0,
                            Image = "whiteking.png"
                        });
  
                    }
                    else
                    {
                        buffer.Add(new ChessSquare
                        {
                            Name = files[col] + row,
                            IsWhite = (row + col) % 2 == 0,
                            Image = null
                        });
       

                    }
             

                    if (buffer.Count >= batchSize)
                    {
                        // Add the batch on the UI thread
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            foreach (var s in buffer) Board.Add(s);
                        });

                        buffer.Clear();

                        // give UI a tick to render
                        await Task.Delay(delayMs);
                    }
                }
            }

            // Add any remaining
            if (buffer.Count > 0)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var s in buffer) Board.Add(s);
                });
            }
        }
        [RelayCommand]
        private void SquareTapped(string square)
        {
            Board.FirstOrDefault(s => s.Name == square).Image = "whiteking.png";


        }

       /* private void InitializeGameListener(string gameId)
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
        }*/
    }
}
