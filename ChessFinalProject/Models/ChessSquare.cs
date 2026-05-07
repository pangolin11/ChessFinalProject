using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ChessFinalProject.Models
{
    public partial class ChessSquare : ObservableObject
    {
        [ObservableProperty]
        private string _Name;   // "A8"

        [ObservableProperty]
        private string _image; // piece image

        [ObservableProperty]
        private bool _IsWhite;
    }

}
