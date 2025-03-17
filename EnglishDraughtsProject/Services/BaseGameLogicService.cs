using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Services;

public abstract class BaseGameLogicService(Board board, ILogger<BaseGameLogicService> logger) : IGameLogicService
{
    protected readonly Board board = board;
    protected bool _isWhiteTurn = true;
    private const int sizeBoard = 8;

    public Board Board => board;
    public bool IsWhiteTurn => _isWhiteTurn;
    
    
    private CellValueEnum.PlayerColor GetPlayerColor(CellValueEnum.CellValue cellValue)
    {
        Console.WriteLine("GetPlayerColor nnnnn");
        switch (cellValue)
        {
            case CellValueEnum.CellValue.WhiteChecker:
            case CellValueEnum.CellValue.WhiteKing:
                Console.WriteLine("GetPlayerColor white");
                return CellValueEnum.PlayerColor.White;
            
            case CellValueEnum.CellValue.BlackChecker:
            case CellValueEnum.CellValue.BlackKing:
                Console.WriteLine("GetPlayerColor black");
                return CellValueEnum.PlayerColor.Black;
            
            default:
                throw new ArgumentException("GetPlayerColor Invalid cell value", nameof(cellValue));
        }
    }

    public abstract Task<string> GetHintAsync();
    public abstract bool Move(int fromX, int fromY, int toX, int toY);

    private bool IsInsideBoard(int x, int y)
    {
        bool inside = x >= 0 && x < sizeBoard && y >= 0 && y < sizeBoard; 
        logger.LogDebug("[IsInsideBoard] : Checking if ({X}, {Y}) is inside board: {Result}", x, y, inside);
        // Console.WriteLine("[BaseGameLogicService] : Checking if ({0}, {1}) is inside board: {2}", x, y, inside);
        return inside;
    }

    protected bool CanMove(Cell from, Cell to)
    {
        logger.LogInformation("[BaseGameLogicService CanMove] : Checking if move is valid from ({FromX}, {FromY}) to ({ToX}, {ToY})", from.X, from.Y, to.X, to.Y);
        Console.WriteLine("[BaseGameLogicService CanMove] : Move from ({0}, {1}) to ({2}, {3})", from.X, from.Y, to.X, to.Y);
        
        if (from.Value == CellValueEnum.CellValue.Empty || to.Value != CellValueEnum.CellValue.Empty)
        {
            logger.LogWarning("[BaseGameLogicService CanMove] : Move is invalid due to empty from cell or occupied to cell.");
            Console.WriteLine("[BaseGameLogicService CanMove] : Move is invalid due to empty from cell or occupied to cell.");
            return false;
        }
        
        CellValueEnum.PlayerColor playerColor = GetPlayerColor(from.Value);

        bool isKing = from.Value == CellValueEnum.CellValue.WhiteKing ||
                      from.Value == CellValueEnum.CellValue.BlackKing;

        int directionX = from.X - to.X;
        int directionY = from.Y - to.Y;

        if (!isKing)
        {
            if ((playerColor == CellValueEnum.PlayerColor.White && directionY <= 0) ||
                (playerColor == CellValueEnum.PlayerColor.Black && directionY >= 0))
            {
                logger.LogWarning("[BaseGameLogicService CanMove] : Checker cannot move in that direction.");
                Console.WriteLine("[BaseGameLogicService CanMove] : Checker cannot move in that direction.");
                return false;
            }
        }

        if (Math.Abs(directionX) == 1 && Math.Abs(directionY) == 1)
        {
            return true;
        }

        if (Math.Abs(directionX) == 2 && Math.Abs(directionY) == 2)
        {
            int middleX = (from.X + to.X) / 2;
            int middleY = (from.Y + to.Y) / 2;

            if (board.Cells[middleX, middleY].Value != CellValueEnum.CellValue.Empty
                && board.Cells[middleX, middleY].Value != from.Value)
            {
                logger.LogInformation("[BaseGameLogicService CanMove] : Valid jump move.");
                Console.WriteLine("[BaseGameLogicService CanMove] : Valid jump move.");
                return true;
            }
        }

        logger.LogWarning("[BaseGameLogicService CanMove] : Move is invalid.");
        Console.WriteLine("[BaseGameLogicService CanMove] : Move is invalid.");
        return false;
    }

    private bool CheckCanJump(int x, int y)
    {
        logger.LogInformation("[BaseGameLogicService ] : Checking if piece at ({X}, {Y}) can jump", x, y);
        Console.WriteLine("[BaseGameLogicService CheckCanJump] : Checking if piece at ({0}, {1}) can jump", x, y);

        var cell = board.Cells[x, y];

        int[] directions = { -2, 2 };

        bool isKing = cell.Value == CellValueEnum.CellValue.WhiteKing ||
                      cell.Value == CellValueEnum.CellValue.BlackKing;

        foreach (var directionX in directions)
        {
            foreach (var directionY in directions)
            {
                int newX = x + directionX;
                int newY = y + directionY;
                int midX = x + directionX / 2;
                int midY = y + directionY / 2;

                if (IsInsideBoard(newX, newY) &&
                    board.Cells[newX, newY].Value == CellValueEnum.CellValue.Empty &&
                    board.Cells[midX, midY].Value != CellValueEnum.CellValue.Empty &&
                    board.Cells[midX, midY].Value != cell.Value)
                {
                    if (isKing || (cell.Value == CellValueEnum.CellValue.WhiteChecker && directionY < 0) ||
                        (cell.Value == CellValueEnum.CellValue.BlackChecker && directionY > 0))
                    {
                        logger.LogInformation("[BaseGameLogicService] : Piece can jump.");
                        Console.WriteLine("[BaseGameLogicService CheckCanJump] : Piece can jump.");
                        return true;
                    }
                }
            }
        }
        
        logger.LogInformation("[BaseGameLogicService] : No jump possible.");
        Console.WriteLine("[BaseGameLogicService CheckCanJump] : No jump possible.");
        return false;
    }

    protected bool PlayerHasAvailableJump(bool isWhiteTurn)
    {
        logger.LogInformation("[BaseGameLogicService] : Checking if player has available jump moves. Player: {PlayerColor}", isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : Checking if player has available jump moves. Player: {0}", isWhiteTurn ? "White" : "Black");
        
        for (int x = 0; x < sizeBoard; x++)
        {
            for (int y = 0; y < sizeBoard; y++)
            {
                Console.WriteLine("new aboba");
                var cell = board.Cells[x, y];
                
                
                if (cell.Value == CellValueEnum.CellValue.Empty)
                {
                    Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : Cell is empty, skipping.");
                    continue;
                }
                
                CellValueEnum.PlayerColor playerColor = GetPlayerColor(cell.Value);
                Console.WriteLine("new aboba2");

                if ((isWhiteTurn && playerColor == CellValueEnum.PlayerColor.White) ||
                    (!isWhiteTurn && playerColor == CellValueEnum.PlayerColor.Black))
                {
                    Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : bbbb");
                    if (CheckCanJump(x, y))
                    {
                        logger.LogInformation("[BaseGameLogicService] : Player has available jump.");
                        Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : Checking if player has available jump.");
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : Player has not available jump.");
                }
            }
        }

        logger.LogInformation("[BaseGameLogicService] : Player does not have any available jump.");
        Console.WriteLine("[BaseGameLogicService PlayerHasAvailableJump] : Player does not have any available jump.");
        return false;
    }

    protected List<Move> GetAllMoves(Board board, bool isWhiteTurn)
    {
        logger.LogInformation("[BaseGameLogicService] : Getting all possible moves for {PlayerColor} player.", isWhiteTurn ? "White" : "Black");
        Console.WriteLine("[BaseGameLogicService GetAllMoves] : Getting all possible moves for {0} player.", isWhiteTurn ? "White" : "Black.");
        List<Move> moves = new List<Move>();

        bool isAvailableJump = PlayerHasAvailableJump(isWhiteTurn);

        for (int i = 0; i < sizeBoard; ++i)
        {
            for (int j = 0; j < sizeBoard; ++j)
            {
                var cell = board.Cells[i, j];
                CellValueEnum.PlayerColor playerColor = GetPlayerColor(cell.Value);

                if ((isWhiteTurn && playerColor == CellValueEnum.PlayerColor.White) ||
                    (!isWhiteTurn && playerColor == CellValueEnum.PlayerColor.Black))
                {
                    var movesForCell = GetMovesForOneChecker(board, i, j);

                    foreach (var move in movesForCell)
                    {
                        if (move.isJump)
                        {
                            if (!isAvailableJump)
                            {
                                moves.Clear();
                                isAvailableJump = true;
                            }

                            moves.Add(move);
                        }
                        else
                        {
                            moves.Add(move);
                        }
                    }
                }
            }
        }
        
        logger.LogInformation("[BaseGameLogicService] : Total moves found: {MoveCount}", moves.Count);
        Console.WriteLine("[BaseGameLogicService GetAllMoves] : Total moves found: {0}", moves.Count);
        return moves;
    }

    private List<Move> GetMovesForOneChecker(Board board, int x, int y)
    {
        logger.LogInformation("[BaseGameLogicService] : Getting possible moves for piece at ({X}, {Y})", x, y);
        Console.WriteLine("[BaseGameLogicService GetMovesForOneChecker] : Getting possible moves for piece at ({0}, {1})", x, y);
        List<Move> moves = new List<Move>();

        bool isKing = board.Cells[x, y].Value == CellValueEnum.CellValue.WhiteKing ||
                      board.Cells[x, y].Value == CellValueEnum.CellValue.BlackKing;
        
        int[] directionOx = { -1, 1 };

        int[] direction = (board.Cells[x, y].Value == CellValueEnum.CellValue.WhiteChecker ||
                           board.Cells[x, y].Value == CellValueEnum.CellValue.BlackKing) ? new[] { -1, 1 } :
            board.Cells[x, y].Value == CellValueEnum.CellValue.WhiteChecker ? new[] { -1 } : new[] { 1 };

        if (!isKing)
        {
            foreach (var directionY in direction)
            {
                foreach (var directionX in directionOx)
                {
                    int jumpX = x + 2 * directionX;
                    int jumpY = y + 2 * directionY;
                    int newX = x + directionX;
                    int newY = y + directionY;

                    if (IsInsideBoard(newX, newY) && board.Cells[newX, newY].Value == CellValueEnum.CellValue.Empty)
                    {
                        moves.Add(new Move(x, y, newX, newY, false, board.Cells[x, y].Value));
                    }

                    if (IsInsideBoard(jumpX, jumpY) &&
                        board.Cells[newX, newY].Value != CellValueEnum.CellValue.Empty &&
                        board.Cells[newX, newY].Value != board.Cells[x, y].Value &&
                        board.Cells[jumpX, jumpY].Value == CellValueEnum.CellValue.Empty)
                    {
                        moves.Add(new Move(x, y, jumpX, jumpY, true, board.Cells[x, y].Value));
                    }
                }
            }
        }

        logger.LogInformation("[BaseGameLogicService] : Found {MoveCount} moves for piece at ({X}, {Y})", moves.Count, x, y);
        Console.WriteLine("[BaseGameLogicService GetMovesForOneChecker] : Found {0} moves for piece at ({1}, {2})", moves.Count, x, y);
        return moves;
    }


    protected void ApplyMove(Board board, int fromX, int fromY, int toX, int toY, bool isJump)
    {
        logger.LogInformation("[BaseGameLogicService] : Applying move from ({FromX}, {FromY}) to ({ToX}, {ToY}). Jump: {IsJump}", fromX, fromY, toX, toY, isJump);
        Console.WriteLine("[BaseGameLogicService ApplyMove] : Applying move from ({0}, {1}) to ({2}, {3}). Jump: {4}", fromX, fromY, toX, toY, isJump);

        var fromXYCell = board.Cells[fromX, fromY];
        var toXYCell = board.Cells[toX, toY];

        if (isJump)
        {
            int middleX = (fromX + toX) / 2;
            int middleY = (fromY + toY) / 2;
            board.Cells[middleX, middleY].Value = CellValueEnum.CellValue.Empty;
            logger.LogInformation("[BaseGameLogicService] : Jumped over piece at ({MiddleX}, {MiddleY})", middleX, middleY);
            Console.WriteLine("[BaseGameLogicService ApplyMove] : Jumped over piece at ({0}, {1})", middleX, middleY);
        }

        toXYCell.Value = fromXYCell.Value;
        fromXYCell.Value = CellValueEnum.CellValue.Empty;

        if (toY == (_isWhiteTurn ? 0 : 7))
        {
            toXYCell.Value = _isWhiteTurn ? CellValueEnum.CellValue.WhiteKing : CellValueEnum.CellValue.BlackKing;
            logger.LogInformation("[BaseGameLogicService] : Piece promoted to king at ({ToX}, {ToY})", toX, toY);
            Console.WriteLine("[BaseGameLogicService ApplyMove] : Piece promoted to king at ({0}, {1})", toX, toY);
        }

        if (!(isJump && CheckCanJump(toX, toY)))
        {
            _isWhiteTurn = !_isWhiteTurn;
            logger.LogInformation("[BaseGameLogicService] : Turn switched. It is now {PlayerColor}'s turn.", _isWhiteTurn ? "White" : "Black");
            Console.WriteLine("[BaseGameLogicService ApplyMove] : Turn switched. It is now {0}'s turn.", _isWhiteTurn ? "White" : "Black");
        }
    }
    
    protected void ApplyMoveForAi(Board board, Move move)
    {
        logger.LogInformation("[BaseGameLogicService] : AI is applying move from ({FromX}, {FromY}) to ({ToX}, {ToY})", move.fromX, move.fromY, move.toX, move.toY);
        Console.WriteLine("[BaseGameLogicService ApplyMoveForAi] : AI is applying move from ({0}, {1}) to ({2}, {3})", move.fromX, move.fromY, move.toX, move.toY);
        ApplyMove(board, move.fromX, move.fromY, move.toX, move.toY, move.isJump);
    }
}