using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Models
{
    public class GameState
    {
        public Dictionary<string, string> Board { get; set; }
        public string WhoseTurn { get; set; }
    }
}
