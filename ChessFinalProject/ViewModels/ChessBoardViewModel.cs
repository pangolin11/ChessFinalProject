using ChessFinalProject.Helper;
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

        private string selectedSquare;



        public ChessBoardViewModel()
        {
            /*InitializeGameListener(_currentLocalGameState.GameId);       */
            _currentLocalGameState = new GameState
            {
                BlackPlayerId = null, // Initially null, waiting for second player
                Status = "waiting",
                LastUpdated = DateTime.UtcNow,
                Board = new()
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
        }

        public async Task InitializePieces()
        {
            Board.FirstOrDefault(s => s.Name == "A1")?.Image = "whiterook.png";
            Board.FirstOrDefault(s => s.Name == "B1")?.Image = "whiteknight.png";
            Board.FirstOrDefault(s => s.Name == "C1")?.Image = "whitebishop.png";
            Board.FirstOrDefault(s => s.Name == "D1")?.Image = "whitequeen.png";
            Board.FirstOrDefault(s => s.Name == "E1")?.Image = "whiteking.png";
            Board.FirstOrDefault(s => s.Name == "F1")?.Image = "whitebishop.png";
            Board.FirstOrDefault(s => s.Name == "G1")?.Image = "whiteknight.png";
            Board.FirstOrDefault(s => s.Name == "H1")?.Image = "whiterook.png";
            Board.FirstOrDefault(s => s.Name == "A2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "B2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "C2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "D2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "E2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "F2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "G2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "H2")?.Image = "whitepawn.png";
            Board.FirstOrDefault(s => s.Name == "A8")?.Image = "blackrook.png";
            Board.FirstOrDefault(s => s.Name == "B8")?.Image = "blackknight.png";
            Board.FirstOrDefault(s => s.Name == "C8")?.Image = "blackbishop.png";
            Board.FirstOrDefault(s => s.Name == "D8")?.Image = "blackqueen.png";
            Board.FirstOrDefault(s => s.Name == "E8")?.Image = "blackking.png";
            Board.FirstOrDefault(s => s.Name == "F8")?.Image = "blackbishop.png";
            Board.FirstOrDefault(s => s.Name == "G8")?.Image = "blackknight.png";
            Board.FirstOrDefault(s => s.Name == "H8")?.Image = "blackrook.png";
            Board.FirstOrDefault(s => s.Name == "A7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "B7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "C7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "D7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "E7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "F7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "G7")?.Image = "blackpawn.png";
            Board.FirstOrDefault(s => s.Name == "H7")?.Image = "blackpawn.png";

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
            if (selectedSquare == null)
            {
                if (Board.FirstOrDefault(s => s.Name == square).Image == null || Board.FirstOrDefault(s => s.Name == square).Image == "")
                    return;

                selectedSquare = square;
                Board.FirstOrDefault(s => s.Name == square).IsYellow = true;
                return;
            }
            var PieceToMove = Board.FirstOrDefault(s => s.Name == selectedSquare);
            if (PieceToMove.Image == null || PieceToMove.Image == "" || PieceToMove == null)
            {
                selectedSquare = null;
                return;
            }
            var PieceType = PieceToMove.GetPieceType();
            if (ChessHelper.IsMoveLegal(PieceType, selectedSquare, square, _currentLocalGameState.Board))
            { 
                _currentLocalGameState.Board[square] = PieceToMove.Image;
                _currentLocalGameState.Board[selectedSquare] = "";
                Board.FirstOrDefault(s => s.Name == square).Image = PieceToMove.Image;
                Board.FirstOrDefault(s => s.Name == selectedSquare).Image = null;
                Board.FirstOrDefault(s => s.Name == selectedSquare).IsYellow = false;
                selectedSquare = null;

                return;
            }
            Board.FirstOrDefault(s => s.Name == selectedSquare).IsYellow = false;
            selectedSquare = null;




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
