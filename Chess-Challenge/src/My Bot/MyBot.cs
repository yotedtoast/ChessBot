using ChessChallenge.API;
using System;
using System.Transactions;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    int i = 0;

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

            //Sets the current move value to the value of the 2 depth capture chain
            i = 0;
            currentMoveValue = EvaluateCaptureValue(board, move, 900);



            board.MakeMove(move);
            //Gives half a pawn worth of weight to checks, can be evaluated later on to scale with number of pieces gone
            if (board.IsInCheck()) { currentMoveValue += 50; }
            //If the next move will cause a repition, avoid the move
            if (board.IsRepeatedPosition()) { currentMoveValue -= 110; }
            //Pawn moves should be supported to avoid 50 move rule
            if (move.MovePieceType == PieceType.Pawn) { currentMoveValue += 10; }
            board.UndoMove(move);

            //In case of promotions, all non-queen promotions should have a very strong negative weight
            if (move.IsPromotion && (move.PromotionPieceType != PieceType.Queen)) { continue; }


            // Sets current best move
            if (currentMoveValue > highestMoveValue)
            {
                moveToPlay = move;
                highestMoveValue = currentMoveValue;
            }


        }
        //Console.WriteLine("Move value: " + highestMoveValue);
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

    //Takes a move and assigns a value based on the worst case scenario recapture
    int EvaluateCaptureValue(Board board, Move m)
    {
        int captureValue = pieceValues[(int)m.CapturePieceType];
        int bestRecaptureValue = 0;
        board.MakeMove(m);
        //By evaluating more than just captures, I can literally just check the value of every move
        Move[] possibleRecaptures = board.GetLegalMoves();
        if (possibleRecaptures == null)
        {
            return captureValue;
        }
        foreach (Move move in possibleRecaptures)
        {
            if (pieceValues[(int)move.CapturePieceType] > bestRecaptureValue)
            {
                bestRecaptureValue = pieceValues[(int)move.CapturePieceType];
            }
        }
        board.UndoMove(m);
        return captureValue - bestRecaptureValue;
    }

    int EvaluateCaptureValue(Board board, Move m, int recursions)
    {
        
        int captureValue = pieceValues[(int)m.CapturePieceType];
        int bestRecaptureValue = 0;
        board.MakeMove(m);
        //By evaluating more than just captures, I can literally just check the value of every move
        Move[] possibleRecaptures = board.GetLegalMoves();

        if ((possibleRecaptures == null) || (possibleRecaptures.Length == 0) )
        {
            board.UndoMove(m);
            return captureValue;
        }
        if (board.IsInCheckmate()) 
        {
            board.UndoMove(m);
            return 1000;
        }
        if (board.IsInsufficientMaterial()) 
        {
            board.UndoMove(m);
            return 0; 
        }
        Move bestRecaptureMove = possibleRecaptures[0];

        if (recursions == i) { return 0; }
        foreach (Move move in possibleRecaptures)
        {
            if (EvaluateCaptureValue(board, move) > bestRecaptureValue)
            {
                bestRecaptureValue = pieceValues[(int)move.CapturePieceType];
            }
        }
        board.UndoMove(m);
        return captureValue - bestRecaptureValue;
    }

    int EvaluatePieceAdvantage(PieceList[] pieceLists)
    {
        int total = 0;
        foreach (PieceList pieces in pieceList) 
        {
            if (pieces.IsWhitePieceList)
        }
    }
}