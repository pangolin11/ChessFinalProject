
using ChessFinalProject.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Service.DBService.Firebase
{
    public interface IGameService
    {
        Task<string> FindGame(string Id);
        Task<GameState?> GetGameState(string gameId);
        IObservable<FirebaseObject<object>> ListenForGameState(string gameId);
        Task SendToFirebase(GameState gameState);
    }
}
