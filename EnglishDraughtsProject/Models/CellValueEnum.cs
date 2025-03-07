namespace EnglishDraughtsProject.Models;

public class CellValueEnum
{
    public enum CellValue
    {
        Empty,
        WhiteChecker,
        BlackChecker,
        WhiteKing,
        BlackKing
    }

    public enum ResultEnum
    {
        WhiteWin,
        BlackWin,
        Draw
    }
}