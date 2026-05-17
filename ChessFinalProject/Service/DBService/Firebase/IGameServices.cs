
using ChessFinalProject.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Service.DBService.Firebase
{
    public interface IGameService
    {
        Task EndGame(GameState currentLocalGameState, string gameId, string kingType);
        Task<string> FindGame(string Id);
        Task<GameState?> GetGameState(string gameId);
        IObservable<FirebaseObject<GameState>> ListenForGameState(string gameId);
        Task SendToFirebase(string squareFrom, string squareTo, string gameid);
    }
}
