using System.Collections;
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