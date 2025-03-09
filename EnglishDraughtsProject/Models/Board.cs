using System.Buffers;

namespace EnglishDraughtsProject.Models;

public class Board
{
    private const int BoardSize = 8;

    public Cell[,] Cells { get; } = new Cell[BoardSize, BoardSize];

    private readonly CellPool _cellPool;

    public Board(CellPool cellPool)
    {
        _cellPool = cellPool;
        InitCells();
    }

    public void ResetBoard()
    {
        for (int i = 0; i < BoardSize; ++i)
        {
            for (int j = 0; j < BoardSize; ++j)
            {
                _cellPool.ReturnCell(Cells[i, j]);
            }
        }

        InitCells();
    }

    public Board CloneBoard()
    {
        Board cloneBoard = new Board(_cellPool);

        for (int i = 0; i < BoardSize; ++i)
        {
            for (int j = 0; j < BoardSize; ++j)
            {
                cloneBoard.Cells[i, j] = _cellPool.GetCell(Cells[i, j].X, Cells[i, j].Y, Cells[i, j].Value);
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
                Cells[i, j] = (i + j) % 2 == 0 ? _cellPool.GetCell(i, j, CellValueEnum.CellValue.Empty) : 
                    (j < 3) ? _cellPool.GetCell(i, j, CellValueEnum.CellValue.BlackChecker) : 
                    (j > 4) ? _cellPool.GetCell(i, j, CellValueEnum.CellValue.WhiteChecker) : 
                    _cellPool.GetCell(i, j, CellValueEnum.CellValue.Empty);
            }
        }
    }
}