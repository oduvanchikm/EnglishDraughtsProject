using System;
using EnglishDraughtsProject.Models;

namespace EnglishDraughtsProject.Services;

public class GameLogicService
{
    public readonly Board board;
    public bool _isWhiteTurn = true;

    public GameLogicService(Board board)
    {
        this.board = board;
    }
    
    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    private bool CanMove(Cell from, Cell to)
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

    private bool CheckCanJump(int x, int y)
    {
        var cell = board.Cells[x, y];
        
        int[] directions = { -2, 2 };
        
        bool isKing = cell.Value == CellValueEnum.CellValue.WhiteKing || cell.Value == CellValueEnum.CellValue.BlackKing;


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

    public bool Move(int fromX, int fromY, int toX, int toY)
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