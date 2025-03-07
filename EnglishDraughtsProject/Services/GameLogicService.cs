using System;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Services;

public class GameLogicService : BaseGameLogicService
{
    private readonly AiService _aiService;
    private readonly ILogger<GameLogicService> _logger;

    public GameLogicService(Board board, AiService aiService, ILogger<GameLogicService> logger) : base(board, logger)
    {
        _aiService = aiService;
        _logger = logger;
    }
    
    public override async Task<string> GetHintAsync()
    {
        _logger.LogInformation("[GameLogicService] : Requesting hint for the current player ({0})", _isWhiteTurn ? "White" : "Black");
        // Console.WriteLine("[GameLogicService] : Requesting hint for the current player ({0})", _isWhiteTurn ? "White" : "Black");
        var hint = await _aiService.GetHintAsync(board, _isWhiteTurn);
        _logger.LogInformation("[GameLogicService] : Hint provided: {0}", hint);
        // Console.WriteLine("[GameLogicService] : Hint provided: {0}", hint);
        
        return hint;
    }

    public override bool Move(int fromX, int fromY, int toX, int toY)
    {
        var fromXYCell = board.Cells[fromX, fromY];
        var toXYCell = board.Cells[toX, toY];
        
        _logger.LogInformation("[GameLogicService] : Attempting move from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);
        Console.WriteLine("[GameLogicService] : Attempting move from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);

        if (!CanMove(fromXYCell, toXYCell))
        {
            _logger.LogWarning("[GameLogicService] : Invalid move from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);
            Console.WriteLine("[GameLogicService] : Invalid move from ({0}, {1}) to ({2}, {3})", fromX, fromY, toX, toY);
            return false;
        }

        int directionX = fromXYCell.X - toXYCell.X;
        int directionY = fromXYCell.Y - toXYCell.Y;
        bool isJump = Math.Abs(directionX) == 2 && Math.Abs(directionY) == 2;

        if (PlayerHasAvailableJump(_isWhiteTurn) && !isJump)
        {
            _logger.LogWarning("[GameLogicService] : Player ({0}) must jump but attempted a regular move", _isWhiteTurn ? "White" : "Black");
            Console.WriteLine("[GameLogicService] : Player ({0}) must jump but attempted a regular move", _isWhiteTurn ? "White" : "Black");
            return false;
        }
        
        _logger.LogInformation("[GameLogicService] : Move is valid. Applying move from ({0}, {1}) to ({2}, {3}), jump: {4}",
            fromX, fromY, toX, toY, isJump ? "Yes" : "No");
        Console.WriteLine("[GameLogicService] : Move is valid. Applying move from ({0}, {1}) to ({2}, {3}), jump: {4}",
            fromX, fromY, toX, toY, isJump ? "Yes" : "No");

        ApplyMove(board, fromX, fromY, toX, toY, isJump);

        _logger.LogInformation("[GameLogicService] : Move applied successfully. Player turn is now ({0})", _isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[GameLogicService] : Move applied successfully. Player turn is now ({0})", _isWhiteTurn ? "White" : "Black");
        
        return true;
    }
}