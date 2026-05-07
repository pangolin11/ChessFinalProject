using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Models
{
    public class ChessMove
    {
            public string PlayerId { get; set; }
            public string FromSquare { get; set; } // e.g., "e2"
            public string ToSquare { get; set; }   // e.g., "e4"
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;

            public ChessMove() { } // Required for Firebase deserialization
    }
}
