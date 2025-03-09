namespace EnglishDraughtsProject.Models;

public class Move(int fromX, int fromY, int toX, int toY, bool isJump, CellValueEnum.CellValue value)
{
    public int fromX { get; set; } = fromX;
    public int fromY { get; set; } = fromY;
    public int toX { get; set; } = toX;
    public int toY { get; set; } = toY;
    private CellValueEnum.CellValue value { get; set; } = value;

    public bool isJump { get; set; } = isJump;

    public override string ToString()
    {
        return $"From ({fromX}, {fromY}) to ({toX}, {toY}), IsJump: {isJump}, Value: {value}";
    }
}