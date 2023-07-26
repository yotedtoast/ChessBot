using ChessChallenge.API;
using System;
using System.Transactions;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();

        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
        int currentMoveValue = 0;
        int highestMoveValue = 0;

        foreach (Move move in allMoves)
        {
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move))
            {
                moveToPlay = move;
                break;
            }

            currentMoveValue = GetCaptureValue(board, move) - MostValuableCapture(board, move);
            if (currentMoveValue > highestMoveValue) {moveToPlay = move;}


        }

        return moveToPlay;
    }

    // Test if this move gives checkmate
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    //Takes a board and determines the most valuable capture given that board
    int MostValuableCapture(Board board, Move m)
    {
        board.MakeMove(m);
        int highest = 0;
        Move[] possibleCaptures = board.GetLegalMoves(true);
        if (possibleCaptures == null)   return 0;
        foreach (Move move in possibleCaptures)
        {
            if (GetCaptureValue(board, move) > highest)
            {
                highest = pieceValues[(int)move.CapturePieceType];
            }
        }
        board.UndoMove(m);
        return 0;
    }

    int GetCaptureValue(Board board, Move move)
    {
        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        return pieceValues[(int)capturedPiece.PieceType];
    }


}