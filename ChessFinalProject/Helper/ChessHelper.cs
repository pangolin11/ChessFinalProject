
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Helper
{
    public static class ChessHelper
    {
        private static readonly List<string> colArr = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

        internal static bool IsMoveLegal(string pieceType, string selectedSquare, string square, Dictionary<string, string> Board)
        {
            var PieceOnSquare = Board.GetValueOrDefault(square);
            if (selectedSquare == square || PieceOnSquare != null && PieceOnSquare != "" && pieceType.Substring(0,2) == PieceOnSquare.Substring(0,2))
                return false;
            int rowFrom = int.Parse(selectedSquare.Substring(1, 1));
            string colFrom = selectedSquare.Substring(0, 1);
            int rowTo = int.Parse(square.Substring(1, 1));
            string colTo = square.Substring(0, 1);
            bool Legal = CheckLegality(pieceType, rowFrom, colFrom, rowTo, colTo, Board);
            return Legal;
        }

        private static bool CheckLegality(string pieceType, int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> Board)
        {
            switch (pieceType)
            {
                case "whiterook":
                    if (IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if (rowFrom != rowTo && colFrom != colTo)
                        return false;
                    return true;
                case "blackrook":
                    if (IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if (rowFrom != rowTo && colFrom != colTo)
                        return false;
                    return true;
                case "whitebishop":
                    if (rowFrom == rowTo || colFrom == colTo)
                        return false;
                    if (IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if (IsMoveDiagonal(rowFrom, colFrom, rowTo, colTo))
                        return true;
                    return false;
                case "blackbishop":
                    if (rowFrom == rowTo || colFrom == colTo)
                        return false;
                    if (IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if (IsMoveDiagonal(rowFrom, colFrom, rowTo, colTo))
                        return true;
                    return false;
                case "whitequeen":
                    if (rowFrom != rowTo && colFrom != colTo)
                    {
                        if (IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                            return false;
                        if (IsMoveDiagonal(rowFrom, colFrom, rowTo, colTo))
                            return true;
                        return false;
                    }
                    else
                    {
                        if (IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                            return false;
                        return true;
                    }
                case "blackqueen":
                    if (rowFrom != rowTo && colFrom != colTo)
                    {
                        if (IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                            return false;
                        if (IsMoveDiagonal(rowFrom, colFrom, rowTo, colTo))
                            return true;
                        return false;
                    }
                    else
                    {
                        if (IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                            return false;
                        return true;
                    }
                case "whiteknight":
                    if ((Math.Abs(rowFrom - rowTo) == 2 && Math.Abs(colFrom[0] - colTo[0]) == 1) || (Math.Abs(rowFrom - rowTo) == 1 && Math.Abs(colFrom[0] - colTo[0]) == 2))
                        return true;
                    return false;
                case "blackknight":
                    if ((Math.Abs(rowFrom - rowTo) == 2 && Math.Abs(colFrom[0] - colTo[0]) == 1) || (Math.Abs(rowFrom - rowTo) == 1 && Math.Abs(colFrom[0] - colTo[0]) == 2))
                        return true;
                    return false;
                case "whitepawn":

                    if (rowFrom == 2)
                    {
                        if (rowFrom - rowTo == -2 && colFrom == colTo || rowFrom - rowTo == -1 && colFrom == colTo)
                        {
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
                            {
                                return false;
                            }
                            return true;
                        }
                        else if (rowFrom - rowTo == -1 && colFrom[0] - colTo[0] == -1)
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board, true))
                                return true;
                        return false;
                    }
                    else
                    {
                        if (rowFrom - rowTo == -1 && colFrom == colTo)
                        {
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
                            {
                                return false;
                            }
                            return true;
                        }
                        else if (rowFrom - rowTo == -1 && Math.Abs(colFrom[0] - colTo[0]) == 1)
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board, true))
                                return true;
                        return false;
                    }
                case "blackpawn":

                    if (rowFrom == 7)
                    {
                        if (rowFrom - rowTo == 2 && colFrom == colTo || rowFrom - rowTo == 1 && colFrom == colTo)
                        {
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
                            {
                                return false;
                            }
                            return true;
                        }
                        else if (rowFrom - rowTo == 1 && colFrom[0] - colTo[0] == 1)
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board, true))
                                return true;
                        return false;
                    }
                    else
                    {
                        if (rowFrom - rowTo == 1 && colFrom == colTo)
                        {
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
                            {
                                return false;
                            }
                            return true;
                        }
                        else if (rowFrom - rowTo == 1 && Math.Abs(colFrom[0] - colTo[0]) == 1)
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board, true))
                                return true;
                        return false;
                    }
                case "whiteking":
                    if (Math.Abs(rowFrom - rowTo) < 2 && Math.Abs(colFrom[0] - colTo[0]) < 2)
                        return true;
                    return false;
                case "blackking":
                    if (Math.Abs(rowFrom - rowTo) < 2 && Math.Abs(colFrom[0] - colTo[0]) < 2)
                        return true;
                    return false;
            }
            return false;
        }

        internal static bool StillInCheck(Dictionary<string, string> board, string king)
        {
            string kingLocation = null;
            foreach(var square in board)
            {
                if (square.Value.Contains(king))
                    kingLocation = square.Key;
            }
            foreach(var square in board)
            {
                if (square.Value != null && square.Value != "" && square.Value.Substring(0, 2) != king.Substring(0, 2))
                {
                    if (IsMoveLegal(square.Value.Replace(".png", ""), square.Key, kingLocation, board))
                        return true;
                }
            }
            return false;
        }

        private static bool IsMoveDiagonal(int rowFrom, string colFrom, int rowTo, string colTo)
        {
            int colDiff = Math.Abs(colArr.IndexOf(colFrom) - colArr.IndexOf(colTo));
            int rowDiff = Math.Abs(rowFrom - rowTo);
            return colDiff == rowDiff && colDiff != 0;
        }
        private static bool IsObstructedPawn(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board, bool IsPieceBeingTaken)
        {
            if (board.GetValueOrDefault(colTo + rowTo.ToString()) != null && board.GetValueOrDefault(colTo + rowTo.ToString()) != "")
                return true;
            return false;
        }
        private static bool IsObstructedPawn(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        { 
                if (board.GetValueOrDefault(colTo + rowTo.ToString()) != null && board.GetValueOrDefault(colTo + rowTo.ToString()) != "")
                    return true;
                return false;
 
        }

        private static bool IsObstructedBishop(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {
            if (colArr.IndexOf(colFrom) > colArr.IndexOf(colTo) && rowFrom > rowTo)
            {
                for (int i = rowFrom - 1; i > rowTo; i--)
                {
                    for(int j = colArr.IndexOf(colFrom) - 1; j > colArr.IndexOf(colTo); j--)
                    {
                        string compare = board.GetValueOrDefault(colArr[j] + i.ToString());
                        i -= 1;
                        if (compare != null && compare != "")
                            return true;
                    }
                }
            }
            else if (colArr.IndexOf(colFrom) > colArr.IndexOf(colTo) && rowFrom < rowTo)
            {
                for (int i = rowFrom + 1; i < rowTo; i++)
                {
                    for (int j = colArr.IndexOf(colFrom) - 1; j > colArr.IndexOf(colTo); j--)
                    {
                        string compare = board.GetValueOrDefault(colArr[j] + i.ToString());
                        i += 1;
                        if (compare != null && compare != "")
                            return true;
                    }
                }
            }

            else if (colArr.IndexOf(colFrom) < colArr.IndexOf(colTo) && rowFrom > rowTo)
            {
                for (int i = rowFrom - 1; i > rowTo; i--)
                {
                    for (int j = colArr.IndexOf(colFrom) + 1; j < colArr.IndexOf(colTo); j++)
                    {
                        string compare = board.GetValueOrDefault(colArr[j] + i.ToString());
                        i -= 1;
                        if (compare != null && compare != "")
                            return true;
                    }
                }
            }
            else if (colArr.IndexOf(colFrom) < colArr.IndexOf(colTo) && rowFrom < rowTo)
            {
                for (int i = rowFrom + 1; i < rowTo; i++)
                {
                    for (int j = colArr.IndexOf(colFrom) + 1; j < colArr.IndexOf(colTo); j++)
                    {
                        string compare = board.GetValueOrDefault(colArr[j] + i.ToString());
                        i += 1;
                        if (compare != null && compare != "")
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool IsObstructedRook(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {            
            string compare;
            if (rowFrom == rowTo)
            {
                if (colArr.IndexOf(colFrom) < colArr.IndexOf(colTo))
                {

                    for (int i = colArr.IndexOf(colFrom) + 1; i < colArr.IndexOf(colTo); i++)
                    {
                        compare = board.GetValueOrDefault(colArr[i] + rowFrom.ToString());
                        if (compare != null && compare != "")
                            return true;

                    }
                }
                else
                {
                    for (int i = colArr.IndexOf(colFrom) - 1; i > colArr.IndexOf(colTo); i--)
                    {
                        compare = board.GetValueOrDefault(colArr[i] + rowFrom.ToString());
                        if (compare != null && compare != "")
                            return true;

                    }
                }
            }
            else
            {
                if (rowFrom < rowTo)
                {

                    for (int i = rowFrom + 1; i < rowTo; i++)
                    {
                        compare = board.GetValueOrDefault(colFrom + i.ToString());
                        if (compare != null && compare != "")
                            return true;

                    }
                }
                else
                {
                    for (int i = rowFrom - 1; i > rowTo; i--)
                    {
                        compare = board.GetValueOrDefault(colFrom + i.ToString());
                        if (compare != null && compare != "")
                            return true;

                    }
                }
            }
           
            return false;
        }
        internal static bool IsCheckmate(Dictionary<string, string> board, string king)
        {
            // Can't be checkmate if not currently in check
            if (!StillInCheck(board, king))
                return false;

            string kingPrefix = king.Substring(0, 2); // "wh" or "bl"

            // Try every friendly piece
            foreach (var square in board)
            {
                if (square.Value == null || square.Value == "")
                    continue;

                if (square.Value.Substring(0, 2) != kingPrefix)
                    continue;

                string pieceType = square.Value.Replace(".png", "");
                string fromSquare = square.Key;

                // Try every square on the board as a destination
                foreach (var col in colArr)
                {
                    for (int row = 1; row <= 8; row++)
                    {
                        string toSquare = col + row.ToString();

                        if (!IsMoveLegal(pieceType, fromSquare, toSquare, board))
                            continue;

                        // Simulate the move on a copy of the board
                        var simulatedBoard = new Dictionary<string, string>(board);
                        simulatedBoard[toSquare] = simulatedBoard[fromSquare];
                        simulatedBoard[fromSquare] = "";

                        // If any move gets us out of check, it's not checkmate
                        if (!StillInCheck(simulatedBoard, king))
                            return false;
                    }
                }
            }

            // No move could escape check — checkmate
            return true;
        }
    }

}
