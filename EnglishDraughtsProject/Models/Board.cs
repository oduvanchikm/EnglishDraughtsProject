using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Models;

public class Board
{
    private const int BoardSize = 8;

    public Cell[,] Cells { get; } = new Cell[BoardSize, BoardSize];

    public Board()
    {
        InitCells();
    }
    
    public void ResetBoard()
    {
        InitCells();
    }

    public Board CloneBoard()
    {
        Board cloneBoard = new Board();

        for (int i = 0; i < BoardSize; ++i)
        {
            for (int j = 0; j < BoardSize; ++j)
            {
                cloneBoard.Cells[i, j] = new Cell(Cells[i, j]);
            }
        }
        
        return cloneBoard;
    }

    private void InitCells()
    {
        for (int i = 0; i < BoardSize; ++i)
        {
            for (int j = 0; j < BoardSize; ++j)
            {
                Cells[i, j] = (i + j) % 2 == 0 ? new Cell(i, j, CellValueEnum.CellValue.Empty) :
                    (j < 3) ? new Cell(i, j, CellValueEnum.CellValue.BlackChecker) :
                    (j > 4) ? new Cell(i, j, CellValueEnum.CellValue.WhiteChecker) :
                    new Cell(i, j, CellValueEnum.CellValue.Empty);
            }
        }
    }
}