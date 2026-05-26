
using ChessFinalProject.Models;
using ChessFinalProject.Service.DBService.Firebase;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessFinalProject.Helper
{
    public static class ChessHelper
    {
        private static readonly List<string> colArr = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        internal static bool IsMoveLegal(string pieceType, string selectedSquare, string square, Dictionary<string, string> Board, Dictionary<string, bool> castlingPiecesMoved)
        {
            var PieceOnSquare = Board.GetValueOrDefault(square);
            if (selectedSquare == square || PieceOnSquare != null && PieceOnSquare != "" && pieceType.Substring(0,2) == PieceOnSquare.Substring(0,2))
                return false;
            int rowFrom = int.Parse(selectedSquare.Substring(1, 1));
            string colFrom = selectedSquare.Substring(0, 1);
            int rowTo = int.Parse(square.Substring(1, 1));
            string colTo = square.Substring(0, 1);
            bool Legal = CheckLegality(pieceType, rowFrom, colFrom, rowTo, colTo, Board, castlingPiecesMoved);
            return Legal;
        }
        private static bool CheckLegality(string pieceType, int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> Board, Dictionary<string, bool> castlingPiecesMoved)
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
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
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
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
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
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
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
                            if (IsObstructedPawn(rowFrom, colFrom, rowTo, colTo, Board))
                                return true;
                        return false;
                    }
                case "whiteking":
                    if (Math.Abs(rowFrom - rowTo) < 2 && Math.Abs(colFrom[0] - colTo[0]) < 2)
                        return true;
                    else if (Math.Abs(rowFrom - rowTo) == 0 && Math.Abs(colFrom[0] - colTo[0]) == 2 && castlingPiecesMoved != null)
                        return CanCastle(rowFrom, colFrom, rowTo, colTo, Board, castlingPiecesMoved, pieceType);
                    return false;
                case "blackking":
                    if (Math.Abs(rowFrom - rowTo) < 2 && Math.Abs(colFrom[0] - colTo[0]) < 2)
                        return true;
                    else if (Math.Abs(rowFrom - rowTo) == 0 && Math.Abs(colFrom[0] - colTo[0]) == 2 && castlingPiecesMoved != null)
                        return CanCastle(rowFrom, colFrom, rowTo, colTo, Board, castlingPiecesMoved, pieceType);
                    return false;
            }
            return false;
        }

        private static bool CanCastle(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board, Dictionary<string, bool> castlingPiecesMoved, string pieceType)
        {
            if(StillInCheck(board, pieceType))
                return false;
            bool isMovingRight = colFrom[0] < colTo[0];
            if (pieceType == "whiteking")
            {
                if (isMovingRight && !castlingPiecesMoved["E1"] && !castlingPiecesMoved["H1"] && string.IsNullOrEmpty(board["F1"]) && string.IsNullOrEmpty(board["G1"]))
                {
                    var simulatedBoard = new Dictionary<string, string>(board);
                    simulatedBoard["E1"] = "";
                    simulatedBoard["F1"] = "whiteking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "F1"))
                        return false;
                    simulatedBoard["F1"] = "";
                    simulatedBoard["G1"] = "whiteking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "G1"))
                        return false;
                    return true;
                }
                else if (!isMovingRight && !castlingPiecesMoved["E1"] && !castlingPiecesMoved["A1"] && string.IsNullOrEmpty(board["B1"]) && string.IsNullOrEmpty(board["C1"]) && string.IsNullOrEmpty(board["D1"]))
                {
                    var simulatedBoard = new Dictionary<string, string>(board);
                    simulatedBoard["E1"] = "";
                    simulatedBoard["D1"] = "whiteking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "D1"))
                        return false;
                    simulatedBoard["D1"] = "";
                    simulatedBoard["C1"] = "whiteking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "C1"))
                        return false;
                    return true;
                }
            }
            else
            {
                if (isMovingRight && !castlingPiecesMoved["E8"] && !castlingPiecesMoved["H8"] && string.IsNullOrEmpty(board["F8"]) && string.IsNullOrEmpty(board["G8"]))
                {
                    var simulatedBoard = new Dictionary<string, string>(board);
                    simulatedBoard["E8"] = "";
                    simulatedBoard["F8"] = "blackking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "F8"))
                        return false;
                    simulatedBoard["F8"] = "";
                    simulatedBoard["G8"] = "blackking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "G8"))
                        return false;
                    return true;
                }
                else if (!isMovingRight && !castlingPiecesMoved["E8"] && !castlingPiecesMoved["A8"] && string.IsNullOrEmpty(board["B8"]) && string.IsNullOrEmpty(board["C8"]) && string.IsNullOrEmpty(board["D8"]))
                {
                    var simulatedBoard = new Dictionary<string, string>(board);
                    simulatedBoard["E8"] = "";
                    simulatedBoard["D8"] = "blackking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "D8"))
                        return false;
                    simulatedBoard["D8"] = "";
                    simulatedBoard["C8"] = "blackking.png";
                    if (StillInCheck(simulatedBoard, pieceType, "C8"))
                        return false;
                    return true;
                }
            }
            return false;
        }

        internal static bool StillInCheck(Dictionary<string, string> board, string king)
        {
            var kingLocation = board.FirstOrDefault(x => x.Value.Contains(king)).Key;
            foreach(var square in board)
            {
                if (!string.IsNullOrEmpty(square.Value) && square.Value.Substring(0, 2) != king.Substring(0, 2))
                {
                    if (IsMoveLegal(square.Value.Replace(".png", ""), square.Key, kingLocation, board, null))
                        return true;
                }
            }
            return false;
        }
        internal static bool StillInCheck(Dictionary<string, string> board, string king, string kingLocation)
        {
            foreach (var square in board)
            {
                if (!string.IsNullOrEmpty(square.Value) && square.Value.Substring(0, 2) != king.Substring(0, 2))
                {
                    if (IsMoveLegal(square.Value.Replace(".png", ""), square.Key, kingLocation, board, null))
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
        private static bool IsObstructedPawn(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        { 
                if (string.IsNullOrEmpty(board.GetValueOrDefault(colTo + rowTo.ToString())))
                    return false;
                return true;
 
        }
        private static bool IsObstructedBishop(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {
            int colFromIdx = colArr.IndexOf(colFrom);
            int colToIdx = colArr.IndexOf(colTo);

            int rowStep = rowTo > rowFrom ? 1 : -1;
            int colStep = colToIdx > colFromIdx ? 1 : -1;

            int row = rowFrom + rowStep;
            int col = colFromIdx + colStep;

            while (row != rowTo && col != colToIdx)
            {
                string square = board.GetValueOrDefault(colArr[col] + row.ToString());
                if (!string.IsNullOrEmpty(square))
                    return true;

                row += rowStep;
                col += colStep;
            }

            return false;
        }
        private static bool IsObstructedRook(int rowFrom, string colFrom, int rowTo, string colTo, Dictionary<string, string> board)
        {
            int colFromIdx = colArr.IndexOf(colFrom);
            int colToIdx = colArr.IndexOf(colTo);

            if (rowFrom == rowTo)
            {
                int step = colToIdx > colFromIdx ? 1 : -1;
                int i = colFromIdx + step;
                while (i != colToIdx)
                {
                    if (!string.IsNullOrEmpty(board.GetValueOrDefault(colArr[i] + rowFrom.ToString())))
                        return true;
                    i += step;
                }
            }
            else
            {
                int step = rowTo > rowFrom ? 1 : -1;
                int i = rowFrom + step;
                while (i != rowTo)
                {
                    if (!string.IsNullOrEmpty(board.GetValueOrDefault(colFrom + i.ToString())))
                        return true;
                    i += step;
                }
            }

            return false;
        }
        internal static bool IsCheckmate(Dictionary<string, string> board, string king)
        {
            if (!StillInCheck(board, king))
                return false;
            string kingPrefix = king.Substring(0, 2); 

            foreach (var square in board)
            {
                if (square.Value == null || square.Value == "")
                    continue;

                if (square.Value.Substring(0, 2) != kingPrefix)
                    continue;

                string pieceType = square.Value.Replace(".png", "");
                string fromSquare = square.Key;

                foreach (var col in colArr)
                {
                    for (int row = 1; row <= 8; row++)
                    {
                        string toSquare = col + row.ToString();

                        if (!IsMoveLegal(pieceType, fromSquare, toSquare, board, null))
                            continue;
                        var simulatedBoard = new Dictionary<string, string>(board);
                        simulatedBoard[toSquare] = simulatedBoard[fromSquare];
                        simulatedBoard[fromSquare] = "";
                        if (!StillInCheck(simulatedBoard, king))
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
