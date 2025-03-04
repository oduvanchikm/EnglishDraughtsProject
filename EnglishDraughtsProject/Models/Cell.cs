namespace EnglishDraughtsProject.Models;

public class Cell
{
    public int X { get; set; }
    public int Y { get; set;  }
    public CellValueEnum.CellValue Value { get; set; }
    
    public Cell(Cell other)
    {
        this.Value = other.Value;
    }

    public Cell(int x, int y, CellValueEnum.CellValue value)
    {
        X = x;
        Y = y;
        Value = value;
    }
}