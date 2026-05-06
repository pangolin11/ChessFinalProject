using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.ViewModels
{
    public class ChessBoardViewModel : ViewModelBase
    {
        private Dictionary<string, string> squares = new Dictionary<string, string>(64);
        public Dictionary<string, string> Squares
        {
            get { return squares; }
            set
            {
                if (squares != value)
                {
                    squares = value;
                    OnPropertyChanged();
                }
            }
        }
        public ChessBoardViewModel()
        {
            InitializeSquares();

        }

        private void InitializeSquares()
        {
            for(char file = 'a'; file <= 'h'; file++ ) 
            {
                for(int rank = 1; rank <= 8; rank++)
                {
                    string squareName = $"{file}{rank}";
                    squares[squareName] = ""; // Initialize with empty string or default piece
                }
            }
        }
    }
}
