namespace EnglishDraughtsProject.Models;

public class Cell(int x, int y, CellValueEnum.CellValue value)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public CellValueEnum.CellValue Value { get; set; } = value;

    public Cell(Cell other) : this(other.X, other.Y, other.Value)
    {
    }
}