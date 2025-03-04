using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;

namespace EnglishDraughtsProject.Services;

public class AiGameLogicService : BaseGameLogicService
{
    private const int depth = 5;
    private const int sizeBoard = 8;

    public AiGameLogicService(Board board) : base(board)
    {
    }

    private async Task<Move> GetNextMoveAsync(Board board, bool isWhiteTurn)
    {
        return await Task.Run(() => GetNextMove(board, isWhiteTurn));
    }

    public override async Task<string> GetHintAsync()
    {
        Move move = await GetNextMoveAsync(board, _isWhiteTurn);
        return move.ToString();
    }

    public override bool Move(int fromX, int fromY, int toX, int toY)
    {
        Move bestMove = GetNextMove(board, _isWhiteTurn);

        if (bestMove == null)
        {
            return false;
        }
        
        ApplyMoveForAi(board, bestMove);

        _isWhiteTurn = !_isWhiteTurn;

        return true;
    }
    
    private Move GetNextMove(Board board, bool isWhiteTurn)
    {
        List<Move> moves = GetAllMoves(board, isWhiteTurn);

        if (moves.Count == 0)
        {
            return null;
        }
        
        int bestScore = int.MinValue;

        List<Move> jumpMoves = moves.Where(m => m.isJump).ToList();
        if (jumpMoves.Count > 0)
        {
            return jumpMoves[new Random().Next(jumpMoves.Count)];
        }
        
        Move bestMove = null;

        foreach (var move in moves)
        {
            Board clonedBoard = board.CloneBoard();
            
            int score = MinMaxAlgorithm(clonedBoard, !isWhiteTurn, depth, int.MinValue, int.MaxValue);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int MinMaxAlgorithm(Board board, bool isWhiteTurn, int depth, int minValue, int maxValue)
    {
        return 0;
    }
    
}