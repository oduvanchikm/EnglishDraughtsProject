using System;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;

namespace EnglishDraughtsProject.Services;

public class GameLogicService : BaseGameLogicService
{
    private readonly AiService _aiService;

    public GameLogicService(Board board, AiService aiService) : base(board)
    {
        _aiService = aiService;
    }
    
    public override async Task<string> GetHintAsync()
    {
        return await _aiService.GetHintAsync(board, _isWhiteTurn);
    }

    public override bool Move(int fromX, int fromY, int toX, int toY)
    {
        var fromXYCell = board.Cells[fromX, fromY];
        var toXYCell = board.Cells[toX, toY];

        if (!CanMove(fromXYCell, toXYCell))
        {
            return false;
        }

        int directionX = fromXYCell.X - toXYCell.X;
        int directionY = fromXYCell.Y - toXYCell.Y;
        bool isJump = Math.Abs(directionX) == 2 && Math.Abs(directionY) == 2;

        if (isJump)
        {
            int middleX = (fromXYCell.X + toXYCell.X) / 2;
            int middleY = (fromXYCell.Y + toXYCell.Y) / 2;

            board.Cells[middleX, middleY].Value = CellValueEnum.CellValue.Empty;
        }

        toXYCell.Value = fromXYCell.Value;
        fromXYCell.Value = CellValueEnum.CellValue.Empty;

        if (toY == (_isWhiteTurn ? 0 : 7))
        {
            toXYCell.Value = _isWhiteTurn ? CellValueEnum.CellValue.WhiteKing : CellValueEnum.CellValue.BlackKing;
        }
        
        if (!(isJump && CheckCanJump(toX, toY)))
        {
            _isWhiteTurn = !_isWhiteTurn; 
        }

        return true;
    }
}