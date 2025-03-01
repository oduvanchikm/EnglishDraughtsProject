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
                if ((i + j) % 2 == 0)
                {
                    Cells[i, j] = new Cell(i, j, CellValueEnum.CellValue.Empty);
                }
                else 
                {
                    if (i < 3)
                    {
                        Cells[i, j] = new Cell(i, j, CellValueEnum.CellValue.BlackChecker);
                    }
                    else if (j > 4)
                    {
                        Cells[i, j] = new Cell(i, j, CellValueEnum.CellValue.WhiteChecker);
                    }
                    else
                    {
                        Cells[i, j] = new Cell(i, j, CellValueEnum.CellValue.Empty);
                    }
                }
            }
        }
    }
    
}