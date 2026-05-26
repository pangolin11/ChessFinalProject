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
        private bool IsEnding;
        private IDispatcherTimer _playerTimer; // Local timer for smooth UI countdown
        [ObservableProperty]
        public partial string PlayerTime { get; set; }
        private string _friendlyTime;
        public string FriendlyTime
        {
            get => _friendlyTime;
            set
            {
                if (_friendlyTime != value)
                {
                    _friendlyTime = value;
                    OnPropertyChanged(nameof(FriendlyTime));
                }
            }
        }
        private string _enemyTime;
        public string EnemyTime
        {
            get => _enemyTime;
            set
            {
                if (_enemyTime != value)
                {
                    _enemyTime = value;
                    OnPropertyChanged(nameof(EnemyTime));
                }
            }
        }
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
        private TimeSpan Infinite;
        private IDispatcherTimer _infinityTimer;
        private IDisposable _gameStateSubscription;
        private GameState _currentLocalGameState;
        private string KingType;
        private string HypotheticalWhiteTime = "";
        private string HypotheticalBlackTime = "";

        public ObservableCollection<ChessSquare> Board { get; } = new();

        private string selectedSquare;
        #endregion
        public ChessBoardViewModel(IAuthService authService, IGameService gameService)
        {
            /*InitializeGameListener(_currentLocalGameState.GameId);       */
            _authService = authService;
            _gameService = gameService;
            WhiteTimeDisplay = TimeSpan.FromSeconds(2000);
            _playerTimer = Application.Current.Dispatcher.CreateTimer();
            _playerTimer.Interval = TimeSpan.FromSeconds(1); // Update every second
            _playerTimer.Tick += OnPlayerTimerTick;
            Infinite = TimeSpan.FromHours(9999);
            _infinityTimer = Application.Current.Dispatcher.CreateTimer();
            _infinityTimer.Interval = TimeSpan.FromSeconds(3);
            _infinityTimer.Tick += async (s, e) =>
            {
                if (!_playerTimer.IsRunning)
                {
                    if (HypotheticalBlackTime == _currentLocalGameState?.BlackTime && HypotheticalWhiteTime == _currentLocalGameState.WhiteTime)
                    {
                        if(KingType == "whiteking.png")
                            KingType = "blackking.png";
                        else
                            KingType = "whiteking.png";
                        await Shell.Current.Navigation.PopToRootAsync();
                    }
                }
                else
                {
                    HypotheticalBlackTime = _currentLocalGameState.BlackTime;
                    HypotheticalWhiteTime = _currentLocalGameState.WhiteTime;
                }
            };
        }

        private async void OnPlayerTimerTick(object? sender, EventArgs e)
        {
            if (WhiteTimeDisplay.TotalSeconds > 0 )
            {
                if (_currentLocalGameState.Status != "waiting")
                {
                    WhiteTimeDisplay = WhiteTimeDisplay.Subtract(TimeSpan.FromSeconds(1));
                    await _gameService.SendTime(_currentLocalGameState, WhiteTimeDisplay.ToString(), KingType);
                }
            }
            else
            {
                _playerTimer.Stop();
                IsEnding = true;
                await Shell.Current.Navigation.PopToRootAsync();
              
            }



        }
        public async Task InitializeBoardAsync( int batchSize = 64, int delayMs = 0)
        {
            Dictionary<string, string> board = new()
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
};
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
                        Image = _currentLocalGameState.Board.FirstOrDefault(x => x.Key == square).Value
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
            if (!_currentLocalGameState.Board.FirstOrDefault(x => x.Key == square).Value.Contains(KingType.Substring(0,2)))
                if (selectedSquare == null)
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
            if (PieceToMove == null || string.IsNullOrEmpty(PieceToMove.Image))
            {
                selectedSquare = null;
                return;
            }
            var PieceType = PieceToMove.GetPieceType();
            if (ChessHelper.IsMoveLegal(PieceType, selectedSquare, square, _currentLocalGameState.Board, _currentLocalGameState.CastlingPiecesMoved))
            {
                if(PieceType.Contains("king") && Math.Abs(selectedSquare[0] - square[0]) == 2)
                {
                    string Side = square[0] > selectedSquare[0] ? "right" : "left";
                    await _gameService.SendToFirebase(selectedSquare, square, _currentLocalGameState.GameId, Side);

                }
                var copy = new Dictionary<string, string>(_currentLocalGameState.Board)
                {
                    [square] = PieceToMove.Image,
                    [selectedSquare] = ""
                }; // shallow copy
                if (!ChessHelper.StillInCheck(copy, KingType))
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
            await InitializeBoardAsync();
            if (KingType == "whiteking.png")
                _playerTimer.Start();
            EnemyTime = WhiteTimeDisplay.ToString();
            FriendlyTime = WhiteTimeDisplay.ToString();


        }
        public void StartListening(string gameId)
        {
            _gameStateSubscription?.Dispose();
            Console.WriteLine("entered start listening");
            _gameStateSubscription = _gameService
                .ListenForGameState(gameId)
                .Subscribe(
                    onNext: async firebaseObject =>
                    {
                        if (firebaseObject == null || firebaseObject.Object == null)
                            return;
                        if (firebaseObject.Object.Status != "waiting")
                        {
                            _currentLocalGameState = firebaseObject.Object;

                        }
                        if (firebaseObject.Object.Status.Contains("wins"))
                        {
                            _currentLocalGameState = null;
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await Shell.Current.Navigation.PopToRootAsync();
                            });
                        }
                        _currentLocalGameState = firebaseObject.Object;
                        if (KingType == "whiteking.png")
                        {
                            if(_currentLocalGameState.WhiteTime != null)
                            FriendlyTime = _currentLocalGameState.WhiteTime;
                            if (_currentLocalGameState.BlackTime != null)
                                EnemyTime = _currentLocalGameState.BlackTime;
                        }
                        else
                        {
                            if (_currentLocalGameState.BlackTime != null)
                                FriendlyTime = _currentLocalGameState.BlackTime;
                            if (_currentLocalGameState.WhiteTime != null)
                                EnemyTime = _currentLocalGameState.WhiteTime;
                        }
                        if (firebaseObject.Object.CanBeChanged)
                        {
                            _currentLocalGameState = firebaseObject.Object;
                            string squareFrom = _currentLocalGameState.squareFrom;
                            string squareTo = _currentLocalGameState.squareTo;
                            if (firebaseObject.Object.Status.Contains("castl"))
                            {
                                if(firebaseObject.Object.Status.Contains("E1"))
                            }
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Board.FirstOrDefault(s => s.Name == squareTo)?.Image = _currentLocalGameState.Board[squareTo];
                                Board.FirstOrDefault(s => s.Name == squareFrom)?.Image = "";
                            });
                            if (ChessHelper.IsCheckmate(_currentLocalGameState.Board, KingType))
                            {
                                _playerTimer.Stop();
                                IsEnding = true;
                                await MainThread.InvokeOnMainThreadAsync(async () => 
                                {
                                    await Shell.Current.Navigation.PopToRootAsync();
                                });
                            }
                            if (_currentLocalGameState.IsWhiteTurn && KingType == "blackking.png" || !_currentLocalGameState.IsWhiteTurn && KingType == "whiteking.png")
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
            if (_currentLocalGameState == null)
                return;
            _playerTimer.Stop();
            _playerTimer.Tick -= OnPlayerTimerTick;
            IsEnding = false;
            _gameStateSubscription?.Dispose();
            await _gameService.EndGame(_currentLocalGameState, _currentLocalGameState.GameId, KingType);
            Console.WriteLine("EndGame");
            
        }
    }
}