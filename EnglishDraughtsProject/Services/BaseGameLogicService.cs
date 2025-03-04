using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;

namespace EnglishDraughtsProject.Services;

public abstract class BaseGameLogicService : IGameLogicService
{
    protected readonly Board board;
    protected bool _isWhiteTurn = true;
    private const int sizeBoard = 8;

    public Board Board => board;
    public bool IsWhiteTurn => _isWhiteTurn;

    protected BaseGameLogicService(Board board)
    {
        this.board = board;
    }

    public abstract Task<string> GetHintAsync();
    public abstract bool Move(int fromX, int fromY, int toX, int toY);

    protected bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    protected bool CanMove(Cell from, Cell to)
    {
        if (from.Value == CellValueEnum.CellValue.Empty || to.Value != CellValueEnum.CellValue.Empty)
        {
            return false;
        }

        bool isKing = from.Value == CellValueEnum.CellValue.WhiteKing ||
                      from.Value == CellValueEnum.CellValue.BlackKing;

        int directionX = from.X - to.X;
        int directionY = from.Y - to.Y;

        if (!isKing)
        {
            if ((from.Value == CellValueEnum.CellValue.WhiteChecker && directionY <= 0) ||
                (from.Value == CellValueEnum.CellValue.BlackChecker && directionY >= 0))
            {
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
                return true;
            }
        }

        return false;
    }

    protected bool CheckCanJump(int x, int y)
    {
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
                        return true;
                    }
                }
            }
        }

        return false;
    }

    protected bool PlayerHasAvailableJump(bool isWhiteTurn)
    {
        for (int x = 0; x < sizeBoard; x++)
        {
            for (int y = 0; y < sizeBoard; y++)
            {
                var cell = board.Cells[x, y];

                if ((isWhiteTurn && (cell.Value == CellValueEnum.CellValue.WhiteChecker ||
                                     cell.Value == CellValueEnum.CellValue.WhiteKing)) ||
                    (!isWhiteTurn && (cell.Value == CellValueEnum.CellValue.BlackChecker ||
                                      cell.Value == CellValueEnum.CellValue.BlackKing)))
                {
                    if (CheckCanJump(x, y))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    protected List<Move> GetAllMoves(Board board, bool isWhiteTurn)
    {
        List<Move> moves = new List<Move>();

        bool isAvailableJump = PlayerHasAvailableJump(isWhiteTurn);

        for (int i = 0; i < sizeBoard; ++i)
        {
            for (int j = 0; j < sizeBoard; ++j)
            {
                var cell = board.Cells[i, j];

                if ((isWhiteTurn && (cell.Value == CellValueEnum.CellValue.WhiteChecker
                                     || cell.Value == CellValueEnum.CellValue.WhiteKing)) ||
                    (!isWhiteTurn && (cell.Value == CellValueEnum.CellValue.BlackChecker ||
                                      cell.Value == CellValueEnum.CellValue.BlackKing)))
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

        return moves;
    }

    private List<Move> GetMovesForOneChecker(Board board, int x, int y)
    {
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

        return moves;
    }
    
    protected void ApplyMoveForAi(Board board, Move move)
    {
        int fromX = move.fromX;
        int fromY = move.fromY;
        int toX = move.toX;
        int toY = move.toY;
        bool isJump = move.isJump;
        var value = move.value;

        var toXYCell = new Cell(toX, toY, value);
        var fromXYCell = new Cell(fromX, fromY, value);

        if (isJump)
        {
            int middleX = (fromX + toX) / 2;
            int middleY = (fromY + toY) / 2;

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
    }
}