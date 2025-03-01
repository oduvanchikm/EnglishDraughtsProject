using System;
using EnglishDraughtsProject.Models;
using HarfBuzzSharp;

namespace EnglishDraughtsProject.Services;

public class GameLogicService
{
    private readonly Board board;

    public GameLogicService(Board board)
    {
        this.board = board;
    }

    private bool CanMove(Cell from, Cell to)
    {
        if (from.Value == CellValueEnum.CellValue.Empty || to.Value != CellValueEnum.CellValue.Empty)
        {
            return false;
        }
        
        int directionX = from.X - to.X;
        int directionY = from.Y - to.Y;

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

    public void Move(int fromX, int fromY, int toX, int toY)
    {
        var fromXYCell = board.Cells[fromX, fromY];
        var toXYCell = board.Cells[toX, toY];

        if (!CanMove(fromXYCell, toXYCell))
        {
            throw new Exception("Can't move cell");
        }
        
        int directionX = fromXYCell.X - toXYCell.X;
        int directionY = fromXYCell.Y - toXYCell.Y;
        
        if (Math.Abs(directionX) == 2 && Math.Abs(directionY) == 2)
        {
            int middleX = (fromXYCell.X + toXYCell.X) / 2;
            int middleY = (fromXYCell.Y + toXYCell.Y) / 2;

            board.Cells[middleX, middleY].Value = CellValueEnum.CellValue.Empty;
        }
        
        toXYCell.Value = fromXYCell.Value;
        fromXYCell.Value = CellValueEnum.CellValue.Empty;
    }
}