namespace EnglishDraughtsProject.Models;

public class Move
{
    public int fromX { get; set; }
    public int fromY { get; set; }
    public int toX { get; set; }
    public int toY { get; set; }
    public CellValueEnum.CellValue value { get; set; }

    public bool isJump { get; set; } 

    public Move(int fromX, int fromY, int toX, int toY, bool isJump, CellValueEnum.CellValue value)
    {
        this.fromX = fromX;
        this.fromY = fromY;
        this.toX = toX;
        this.toY = toY;
        this.isJump = isJump;
        this.value = value;
    }
    
    public override string ToString() => value.ToString();
}