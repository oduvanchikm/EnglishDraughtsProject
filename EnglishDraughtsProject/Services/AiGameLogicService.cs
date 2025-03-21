using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Services;

public class AiGameLogicService(Board board, ILogger<AiGameLogicService> logger) : BaseGameLogicService(board, logger)
{
    private const int Depth = 5;
    private const int sizeBoard = 8;

    private async Task<Move> GetNextMoveAsync(Board board, bool isWhiteTurn)
    {
        return await Task.Run(() => GetNextMove(board, isWhiteTurn));
    }

    public override async Task<string> GetHintAsync()
    {
        logger.LogInformation("[AiGameLogicService] : Getting hint for the current board state.");
        Console.WriteLine("[AiGameLogicService GetHintAsync] : Getting hint for the current board state.");
        Console.WriteLine("[AiGameLogicService GetHintAsync] : {0}" , _isWhiteTurn);
        Move move = await GetNextMoveAsync(board, _isWhiteTurn);
        
        
        if (move == null)
        {
            logger.LogWarning("[AiGameLogicService] : No valid move found for hint.");
            Console.WriteLine("[AiGameLogicService GetHintAsync] : No valid move found for hint.");
            return null;
        }
        
        logger.LogInformation("[AiGameLogicService] : Hint provided: {Hint}", move.ToString());
        Console.WriteLine("[AiGameLogicService GetHintAsync] : Hint provided: {0}", move.ToString());
        return move.ToString();
    }

    public override bool Move(int fromX, int fromY, int toX, int toY)
    {
        logger.LogInformation("[AiGameLogicService] : Attempting to move a piece from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);
        Console.WriteLine("[AiGameLogicService Move] : Attempting to move a piece from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);
        
        Move bestMove = GetNextMove(board, !_isWhiteTurn);

        if (bestMove == null)
        {
            logger.LogWarning("[AiGameLogicService] : No valid move found for the AI.");
            Console.WriteLine("[AiGameLogicService Move] : No valid move found for the AI.");
            return false;
        }
        
        logger.LogInformation("[AiGameLogicService] : Best move found: {BestMove}", bestMove.ToString());
        Console.WriteLine("[AiGameLogicService Move] : Best move found: {0}", bestMove.ToString());

        ApplyMoveForAi(board, bestMove);

        _isWhiteTurn = !_isWhiteTurn;
        
        logger.LogInformation("[AiGameLogicService] : Move applied. It's now the turn of the {TurnColor}.", _isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[AiGameLogicService Move] : Move applied. It's now the turn of the {0}.", _isWhiteTurn ? "White" : "Black");

        return true;
    }

    private Move GetNextMove(Board board, bool isWhiteTurn)
    {
        logger.LogInformation("[AiGameLogicService] : Getting the next move for {PlayerColor} player.", isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[AiGameLogicService GetNextMove] : Getting the next move for {0} player.", isWhiteTurn ? "White" : "Black");

        List<Move> moves = GetAllMoves(board, isWhiteTurn);

        if (moves.Count == 0)
        {
            logger.LogWarning("[AiGameLogicService] : No available moves for the {PlayerColor} player.", isWhiteTurn ? "White" : "Black");
            Console.WriteLine("[AiGameLogicService GetNextMove] : No available moves for the {0} player.", isWhiteTurn ? "White" : "Black");
            return null;
        }

        int bestScore = isWhiteTurn ? int.MinValue : int.MaxValue;

        List<Move> jumpMoves = moves.Where(m => m.isJump).ToList();
        if (jumpMoves.Count > 0)
        {
            logger.LogInformation("[AiGameLogicService] : Jump moves available. Returning a random jump move.");
            Console.WriteLine("[AiGameLogicService GetNextMove] : Returning a random jump move.");
            return jumpMoves[new Random().Next(jumpMoves.Count)];
        }

        Move bestMove = null;

        foreach (var move in moves)
        {
            Board clonedBoard = board.CloneBoard();

            int score = MinMaxAlgorithm(clonedBoard, !isWhiteTurn, Depth, int.MinValue, int.MaxValue);
            
            logger.LogInformation("[AiGameLogicService] : Move evaluated: {Move}. Score: {Score}", move.ToString(), score);
            Console.WriteLine("[AiGameLogicService GetNextMove] : Move evaluated: {0}. Score: {1}", move.ToString(), score);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }
        
        if (bestMove != null)
        {
            logger.LogInformation("[AiGameLogicService] : Best move selected: {BestMove}", bestMove.ToString());
            Console.WriteLine("[AiGameLogicService GetNextMove] : Best move selected: {0}", bestMove.ToString());
        }
        else
        {
            logger.LogWarning("[AiGameLogicService] : No best move selected.");
            Console.WriteLine("[AiGameLogicService GetNextMove] : No best move selected.");
        }

        return bestMove;
    }

    private int MinMaxAlgorithm(Board board, bool isWhiteTurn, int depth, int alpha, int beta)
    {
        logger.LogInformation("[AiGameLogicService] : Running MinMax algorithm at depth {Depth} for {PlayerColor} player.", depth, isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[AiGameLogicService MinMaxAlgorithm] : Running MinMax algorithm at depth {0} for {1} player.", depth, isWhiteTurn ? "White" : "Black");
        
        if (depth == 0)
        {
            int evaluation = EvaluateBoard(board);
            logger.LogInformation("[AiGameLogicService] : Board evaluated at depth 0. Score: {Score}", evaluation);
            Console.WriteLine("[AiGameLogicService MinMaxAlgorithm] : Board evaluated at depth 0. Score: {0}", evaluation);
            return evaluation;
        }

        List<Move> moves = GetAllMoves(board, isWhiteTurn);

        if (moves.Count == 0) 
        {
            return isWhiteTurn ? int.MinValue : int.MaxValue;
        }

        if (isWhiteTurn)
        {
            int maxEval = int.MinValue;
            foreach (var move in moves)
            {
                Board clonedBoard = board.CloneBoard();
                ApplyMoveForAi(clonedBoard, move);
            
                int eval = MinMaxAlgorithm(clonedBoard, !isWhiteTurn, depth - 1, alpha, beta);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in moves)
            {
                Board clonedBoard = board.CloneBoard();
                ApplyMoveForAi(clonedBoard, move);
            
                int eval = MinMaxAlgorithm(clonedBoard, !isWhiteTurn, depth - 1, alpha, beta);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha) break;
            }
            return minEval;
        }
    }


    private bool IsCenterPosition(int x, int y)
    {
        return x >= 2 && x <= 5 && y >= 2 && y <= 5;
    }

    private int EvaluateBoard(Board board)
    {
        logger.LogInformation("[AiGameLogicService] : Evaluating the board.");
        Console.WriteLine("[AiGameLogicService IsCenterPosition] : Evaluating the board.");

        int whiteCheckerCount = 0;
        int blackCheckerCount = 0;

        int whiteKingCount = 0;
        int blackKingCount = 0;

        int whiteCheckersInCenter = 0;
        int blackCheckersInCenter = 0;

        for (int i = 0; i < sizeBoard; i++)
        {
            for (int j = 0; j < sizeBoard; j++)
            {
                var cell = board.Cells[i, j];
                bool isCenter = IsCenterPosition(i, j);

                switch (cell.Value)
                {
                    case CellValueEnum.CellValue.WhiteChecker:
                        whiteCheckerCount++;
                        if (isCenter) whiteCheckersInCenter++;
                        break;
                    
                    case CellValueEnum.CellValue.BlackChecker:
                        blackCheckerCount++;
                        if (isCenter) blackCheckersInCenter++;
                        break;
                    
                    case CellValueEnum.CellValue.WhiteKing:
                        whiteKingCount++;
                        break;
                    
                    case CellValueEnum.CellValue.BlackKing:
                        blackKingCount++;
                        break;
                }
            }
        }
        
        int score = (whiteCheckerCount - blackCheckerCount) * 100 + (whiteKingCount - blackKingCount) * 200
            + (whiteCheckersInCenter - blackCheckersInCenter) * 10;
        
        logger.LogInformation("[AiGameLogicService] : Board evaluation complete. Score: {Score}", score);
        Console.WriteLine("[AiGameLogicService IsCenterPosition] : Board evaluation complete. Score: {0}", score);
        
        return score;
    }
}