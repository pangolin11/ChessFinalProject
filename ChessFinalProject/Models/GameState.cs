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
        public Dictionary<string, string> Board { get; set; } = new Dictionary<string, string>(); // e.g., "e2" => "white_pawn.svg" (white pawn)
        public List<ChessMove> Moves { get; set; } = new List<ChessMove>();
        public string Status { get; set; } // e.g., "playing", "checkmate", "stalemate"
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string King { get; set; }
        public GameState() { }
    }
}
