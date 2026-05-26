using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Models
{
    public class GameState
    {
        public string GameId { get; set; }
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public string CurrentTurnPlayerId { get; set; }
        public Dictionary<string, string> Board { get; set; } = new Dictionary<string, string>(); // e.g., "e2" => "white_pawn.png" (white pawn)
        public string Status { get; set; } 
        public string squareFrom { get; set; }
        public string squareTo { get; set; }
        public bool IsWhiteTurn { get; set; }
        public bool CanBeChanged { get; set; }
        public string WinningPlayerId { get; set; }
        public string LosingPlayerId { get; set; }
        public string WhiteTime { get; set; }
        public string BlackTime { get; set; }
        public Dictionary<string, bool> CastlingPiecesMoved { get; set; } = new Dictionary<string, bool>(); 


        public GameState() { }
    }
}
