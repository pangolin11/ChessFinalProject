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
        private IDispatcherTimer _playerTimer; // Local timer for smooth UI countdown
        private bool _busy;
        public bool Busy
        {
            get => _busy;
            set
            {
                if (_busy != value)
                {
                    _busy = value;
                    OnPropertyChanged(nameof(Busy));
                }
            }
        }
        private string _loadingMessage;
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged(nameof(LoadingMessage));
                }
            }
        }
        private readonly IGameService _gameService;
        private TimeSpan WhiteTimeDisplay;
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
            WhiteTimeDisplay = TimeSpan.FromSeconds(10);
            _playerTimer = Application.Current.Dispatcher.CreateTimer();
            _playerTimer.Interval = TimeSpan.FromSeconds(1); // Update every second
            _playerTimer.Tick += OnPlayerTimerTick;
            

        }

        private async void OnPlayerTimerTick(object? sender, EventArgs e)
        {
            if (WhiteTimeDisplay.TotalSeconds > 0)
            {
                WhiteTimeDisplay = WhiteTimeDisplay.Subtract(TimeSpan.FromSeconds(1));
                // Notify UI if using data binding
                OnPropertyChanged(nameof(WhiteTimeDisplay));
            }
            else
            {
                _playerTimer.Stop();
                await Shell.Current.GoToAsync("MainPageView");

                // Handle timeout — e.g., end turn, declare winner, etc.
            }



        }
        public async Task InitializeBoardAsync(Dictionary<string, string> board, int batchSize = 64, int delayMs = 0)
        {
           
            Board.Clear();
            string[] files = { "A", "B", "C", "D", "E", "F", "G", "H" };

            var buffer = new List<ChessSquare>(batchSize);

            for (int row = 8; row >= 1; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                    string square = files[col] + row;
                        buffer.Add(new ChessSquare
                        {
                            Name = square,
                            IsWhite = (row + col) % 2 == 0,
                            Image = board.FirstOrDefault(x => x.Key == square).Value
                        });
                    if (buffer.Count >= batchSize)
                    {
                        // Add the batch on the UI thread
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            foreach (var s in buffer) Board.Add(s);
                        });

                        buffer.Clear();

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
            if (KingType == "blackking.png")
            {
                var reversed = Board.Reverse().ToList();
                Board.Clear();
                foreach (var s in reversed) Board.Add(s);
            }
        }
        [RelayCommand]
        private async Task SquareTapped(string square)
        {
            if (_currentLocalGameState.Status == "waiting")
                return;
            if (KingType == "whiteking.png" && _currentLocalGameState.Board.FirstOrDefault(x => x.Key == square).Value.Contains("black") || KingType == "blackking.png" && _currentLocalGameState.Board.FirstOrDefault(x => x.Key == square).Value.Contains("white"))
                if(selectedSquare == null)
                    return;
            if (KingType == "whiteking.png" && _currentLocalGameState.IsWhiteTurn == false || KingType == "blackking.png" && _currentLocalGameState.IsWhiteTurn == true)
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
            if (PieceToMove?.Image == null || PieceToMove.Image == "" || PieceToMove == null)
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
                    
                    await _gameService.SendToFirebase(selectedSquare, square, _currentLocalGameState.GameId);
                }
            }
            Board.FirstOrDefault(s => s.Name == selectedSquare)?.IsYellow = false;
            selectedSquare = null;
        }
        internal async Task StartGame()
        {
            string id = _authService.GetCurrentUserId();
            string gameId = await _gameService.FindGame(id);
            StartListening(gameId);
            _currentLocalGameState = await _gameService.GetGameState(gameId);

            if (_currentLocalGameState?.WhitePlayerId == _authService.GetCurrentUserId())
            {
                KingType = "whiteking.png";
            }
            else
            {
                KingType = "blackking.png";
            }
            await InitializeBoardAsync(_currentLocalGameState.Board);
            if(KingType == "whiteking.png")
                _playerTimer.Start();



        }
        public void StartListening(string gameId)
        {
            _gameStateSubscription?.Dispose();
            Console.WriteLine("entered start listening");
            _gameStateSubscription = _gameService
                .ListenForGameState(gameId)
                .Subscribe(
                    onNext: firebaseObject =>
                    {
                        if (firebaseObject == null || firebaseObject.Object == null)
                            return;
                        if (firebaseObject.Object.Status != "waiting")
                        {
                            _currentLocalGameState = firebaseObject.Object;

                        }
                        Console.WriteLine("entered on next");
                        /*                        Console.WriteLine($"Type of firebaseObject.Object: {firebaseObject.Object.GetType()}");
                                                var json = Newtonsoft.Json.JsonConvert.SerializeObject(firebaseObject.Object, Newtonsoft.Json.Formatting.Indented);
                                                Console.WriteLine($"Raw Firebase object received:\n{json}");*/
                        if (firebaseObject.Object.CanBeChanged)
                        {
                            _currentLocalGameState = firebaseObject.Object;
                            //_currentLocalGameState = firebaseObject.Object; // Update local state with latest from Firebase
                            string squareFrom = _currentLocalGameState.squareFrom;
                            string squareTo = _currentLocalGameState.squareTo;

                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Board.FirstOrDefault(s => s.Name == squareTo)?.Image = Board.FirstOrDefault(s => s.Name == squareFrom)?.Image;
                                Board.FirstOrDefault(s => s.Name == squareFrom)?.Image = "";
                            });
                            if (_playerTimer.IsRunning)
                                _playerTimer.Stop();
                            else
                            _playerTimer.Start();
                        }
                       
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
        public async Task EndGame()
        {

        }
    }
}
