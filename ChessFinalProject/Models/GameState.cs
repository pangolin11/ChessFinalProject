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
        public GameState() { }
    }
}
