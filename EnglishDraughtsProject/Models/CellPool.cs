using System;
using System.Collections.Generic;

namespace EnglishDraughtsProject.Models;

public class CellPool
{
    private readonly Stack<Cell> _stackCellPool = new Stack<Cell>();

    public CellPool(int initialSize)
    {
        for (int i = 0; i < initialSize; ++i)
        {
            _stackCellPool.Push(new Cell(0, 0, CellValueEnum.CellValue.Empty));
        }
    }

    public Cell GetCell(int x, int y, CellValueEnum.CellValue value)
    {
        if (_stackCellPool.Count > 0)
        {
            var cell = _stackCellPool.Pop();
            cell.X = x;
            cell.Y = y;
            cell.Value = value;
            return cell;
        }
        else
        {
            Console.WriteLine("Warning: Cell pool is empty. Creating a new Cell object.");
            return new Cell(x, y, value);
        }
    }
    
    public void ReturnCell(Cell cell)
    {
        cell.Value = CellValueEnum.CellValue.Empty;
        _stackCellPool.Push(cell);
    }
}