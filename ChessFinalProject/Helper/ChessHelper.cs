
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Helper
{
    public static class ChessHelper
    {
        internal static bool IsMoveLegal(string pieceType, string selectedSquare, string square, Dictionary<string, string> Board)
        {
            var PieceOnSquare = Board.GetValueOrDefault(square);
            if (selectedSquare == square || PieceOnSquare != null && PieceOnSquare != "" && pieceType.Substring(0,2) == PieceOnSquare.Substring(0,2))
                return false;
            int rowFrom = int.Parse(selectedSquare.Substring(1, 1));
            string colFrom = selectedSquare.Substring(0, 1);
            int rowTo = int.Parse(square.Substring(1, 1));
            string colTo = square.Substring(0, 1);
            switch (pieceType)
            {
                case "whiterook":
                    if(IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if(rowFrom != rowTo && colFrom != colTo )
                        return false;
                    return true;
                case "blackrook":
                    if (IsObstructedRook(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    if (rowFrom != rowTo && colFrom != colTo)
                        return false;
                    return true;
                case "whitebishop":
                    if(rowFrom == rowTo || colFrom == colTo)
                        return false;
                    if(IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    return true;
                case "blackbishop":
                    if (rowFrom == rowTo || colFrom == colTo)
                        return false;
                    if (IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                        return false;
                    return true;
                case "whitequeen":
                    if (rowFrom != rowTo && colFrom != colTo)
                    {
                        if(IsObstructedBishop(rowFrom, colFrom, rowTo, colTo, Board))
                            return false;
                        return true;

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
                        return true;

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
                    IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board, true);
                    if (rowFrom == 2)
                    {
                        if (rowFrom - rowTo == -2 && colFrom == colTo || rowFrom - rowTo == -1 && colFrom == colTo)
                            return true;
                    }
                    else
                    {
                        if (rowFrom - rowTo == -1 && colFrom == colTo)
                        {
                            return true;
                        }

                    }  
                    return false;
            }
            return false;
        }

        private static void IsObstructedPawn(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board, bool IsWhite)
        {
            List<string> colArr = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

            if (IsWhite)
            {
                if (board.GetValueOrDefault(colArr[]))
            }
        }

        private static bool IsObstructedBishop(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {
            List<string> colArr = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            if (colArr.IndexOf(colFrom) > colArr.IndexOf(colTo) && rowFrom > rowTo)
            {
                for (int i = rowFrom - 1; i > rowTo; i--)
                {
                    for(int j = colArr.IndexOf(colFrom) - 1; j > colArr.IndexOf(colTo); j--)
                    {
                        string compare = board.GetValueOrDefault(colArr[j] + i.ToString());
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
                        if (compare != null && compare != "")
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool IsObstructedRook(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {
            List<string> colArr = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            
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
    }
}
