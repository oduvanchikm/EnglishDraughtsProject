using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;

namespace EnglishDraughtsProjectTests;

public class UnitTest2
{
    private readonly GameLogicService _gameLogicService;
    private readonly Board _board;

    public UnitTest2()
    {
        _board = new Board();
        _gameLogicService = new GameLogicService(_board, null);
    }

    [Fact]
    public void Test1()
    {
        var board = new Board();
        board.Cells[2, 3] = new Cell(2, 3, CellValueEnum.CellValue.WhiteChecker);
        
        var gameLogic = new GameLogicService(board, null);
        
        var result = gameLogic.Move(2, 3, 3, 4);
        Assert.True(result, "The move must be valid if the cell is free");
    }
    
    [Fact]
    public void Test2()
    {
        var board = new Board();
        board.Cells[2, 3] = new Cell(2, 3, CellValueEnum.CellValue.WhiteChecker);
        board.Cells[3, 4] = new Cell(3, 4, CellValueEnum.CellValue.BlackChecker); 
        
        var gameLogic = new GameLogicService(board, null);

        var result = gameLogic.Move(2, 3, 4, 5); 

        Assert.True(result);
        Assert.Equal(CellValueEnum.CellValue.Empty, board.Cells[3, 4].Value);
    }
    
    [Fact]
    public void Test3()
    {
        var board = new Board();
        board.Cells[2, 3] = new Cell(2, 3, CellValueEnum.CellValue.WhiteChecker);
        board.Cells[3, 4] = new Cell(3, 4, CellValueEnum.CellValue.BlackChecker); 
        
        var gameLogic = new GameLogicService(board, null);

        var result = gameLogic.Move(2, 3, 3, 4);

        Assert.False(result);
    }

}