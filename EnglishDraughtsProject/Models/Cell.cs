using System.ComponentModel;
using Avalonia.Media;

namespace EnglishDraughtsProject.Models;

public class Cell
{
    public int X { get; }
    public int Y { get; }
    public CellValueEnum.CellValue Value { get; set; }

    public Cell(int x, int y, CellValueEnum.CellValue value)
    {
        X = x;
        Y = y;
        Value = value;
    }

    public Cell(Cell other)
    {
        X = other.X;
        Y = other.Y;
        Value = other.Value;
    }
}