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
    public partial class ChessBoardViewModel : ViewModelBase, IDisposable
    {
        #region Variables
        private readonly IAuthService _authService;
        private readonly IGameService _gameService;
        private IDisposable _gameStateSubscription;
        private GameState _currentLocalGameState;
        private string KingType;
        public ObservableCollection<ChessSquare> Board { get; } = new();

        private string selectedSquare;
        #endregion
        public ChessBoardViewModel(IAuthService authService, IGameService gameService)
        {
            /*InitializeGameListener(_currentLocalGameState.GameId);       */
            _authService = authService;
            _gameService = gameService;
           
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
            if (KingType == "whiteking.png" && square.Contains("black") || KingType == "blackking.png" && square.Contains("white"))
                return;
            if (selectedSquare == null)
            {
                if (Board.FirstOrDefault(s => s.Name == square)?.Image == null || Board.FirstOrDefault(s => s.Name == square)?.Image == "")
                    return;

                selectedSquare = square;
                Board.FirstOrDefault(s => s.Name == square)?.IsYellow = true;
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
                var copy = new Dictionary<string, string>(_currentLocalGameState.Board)
                {
                    [square] = PieceToMove.Image,
                    [selectedSquare] = ""
                }; // shallow copy
                if (!ChessHelper.StillInCheck(copy,KingType))
                {
                    _currentLocalGameState.Board = copy;
                    _currentLocalGameState.squareFrom = selectedSquare;
                    _currentLocalGameState.squareTo = square;
                    _gameService.SendToFirebase(_currentLocalGameState);
                    Board.FirstOrDefault(s => s.Name == square)?.Image = Board.FirstOrDefault(s => s.Name == selectedSquare)?.Image;
                    Board.FirstOrDefault(s => s.Name == selectedSquare)?.Image = "";
                    Board.FirstOrDefault(s => s.Name == selectedSquare)?.IsYellow = false;
                }
            }
            Board.FirstOrDefault(s => s.Name == selectedSquare)?.IsYellow = false;
            selectedSquare = null;
        }
        internal async Task StartGame()
        {
            string id = _authService.GetCurrentUserId();
            string gameId = await _gameService.FindGame(id);
            _currentLocalGameState = await _gameService.GetGameState(gameId);
            if (_currentLocalGameState?.WhitePlayerId == _authService.GetCurrentUserId())
            {
                KingType = "whiteking.png";
            }
            else
            {
                KingType = "blackking.png";
            }
            StartListening(gameId);
        }
        public void StartListening(string gameId)
        {
            // for onNext to work i need to get object from the listener and i dont know why
            _gameStateSubscription?.Dispose();
            Console.WriteLine("entered start listening");
            _gameStateSubscription = _gameService
                .ListenForGameState(gameId)
                .Subscribe(
                    onNext: firebaseObject =>
                    {
                        Console.WriteLine("entered on next");
                        //_currentLocalGameState = firebaseObject.Object; // Update local state with latest from Firebase
                        string squareFrom = _currentLocalGameState.squareFrom;
                        string squareTo = _currentLocalGameState.squareTo;
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Board.FirstOrDefault(s => s.Name == squareTo)?.Image = Board.FirstOrDefault(s => s.Name == squareFrom)?.Image;
                            Board.FirstOrDefault(s => s.Name == squareFrom)?.Image = "";
                        });
                    },
                    onError: ex =>
                    {
                        Console.WriteLine("entered error");
                        // Handle errors (e.g. lost connection, permission denied)
                        Console.WriteLine($"Firebase error: {ex.Message}");
                    },
                    onCompleted: () =>
                    {
                        Console.WriteLine("entered completed");
                        // Stream ended (rarely happens with Firebase listeners)
                        Console.WriteLine("Firebase stream completed.");
                    }
                );
        }
        public void Dispose()
        {
            StopListening();
            GC.SuppressFinalize(this); 
        }
        public void StopListening()
        {
            _gameStateSubscription?.Dispose();
            _gameStateSubscription = null;
        }
    }
}
