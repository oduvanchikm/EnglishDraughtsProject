using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Threading;
using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Views;

public partial class BoardView : UserControl
{
    private GameLogicService _gameLogicService;
    private AiGameLogicService _aiGameLogicService;
    private readonly UniformGrid _grid;
    private readonly Board _board;
    private Cell? _selectedCell;
    private CellValueEnum.CellValue _playerColor;

    public BoardView()
    {
        CellPool cellPool = new CellPool(128);
        _board = new Board(cellPool); 
        
        InitializeComponent();

        _gameLogicService = new GameLogicService(_board,
            new AiService("OPENAI_API_KEY", new LoggerFactory().CreateLogger<AiService>()),
            new LoggerFactory().CreateLogger<GameLogicService>());

        var playerSelection = new ComboBox
        {
            ItemsSource = new[] { "White", "Black" },
            SelectedIndex = 0, 
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 10)
        };
        playerSelection.SelectionChanged += PlayerSelection_Changed;

        var boardContainer = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#2F4F4F")),
            Padding = new Thickness(20),
            CornerRadius = new CornerRadius(12),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = _grid = new UniformGrid
            {
                Rows = 8,
                Columns = 8,
                Width = 640,
                Height = 640,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        // var hintButton = new Button
        // {
        //     Content = "Get Hint",
        //     HorizontalAlignment = HorizontalAlignment.Center,
        //     VerticalAlignment = VerticalAlignment.Bottom,
        //     Margin = new Thickness(0, 20, 0, 0)
        // };
        // hintButton.Click += HintButton_Click;

        Content = new StackPanel
        {
            Children = { playerSelection, boardContainer }
        };

        _playerColor = CellValueEnum.CellValue.WhiteChecker;
        _gameLogicService.SetIsWhiteTurn(true);
        
        _aiGameLogicService = new AiGameLogicService(_board,
            new LoggerFactory().CreateLogger<AiGameLogicService>());
        
        DrawBoard();
    }

    private void PlayerSelection_Changed(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine("[BoardView] : PlayerSelection_Changed called");

        var comboBox = sender as ComboBox;
        if (comboBox?.SelectedItem is string selectedColor)
        {
            _playerColor = selectedColor == "White"
                ? CellValueEnum.CellValue.WhiteChecker
                : CellValueEnum.CellValue.BlackChecker;

            Console.WriteLine($"[BoardView] : Player selected color: {selectedColor}");

            RestartGame();
        }
    }
    
    private void RestartGame()
    {
        // _logger.LogInformation("[BoardView] : Restarting the game.");
        Console.WriteLine("[BoardView] : Restarting the game.");

        
        _gameLogicService = new GameLogicService(_board,
            new AiService("OPENAI_API_KEY", new LoggerFactory().CreateLogger<AiService>()),
            new LoggerFactory().CreateLogger<GameLogicService>());

        _aiGameLogicService = new AiGameLogicService(_board,
            new LoggerFactory().CreateLogger<AiGameLogicService>());

        DrawBoard();

        _gameLogicService.SetIsWhiteTurn(_playerColor == CellValueEnum.CellValue.WhiteChecker);
        
        Console.WriteLine($"[BoardView RestartGame ] : Current turn: {(_gameLogicService.IsWhiteTurn ? "White" : "Black")}");
        
        if (!_gameLogicService.IsWhiteTurn)
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                await Dispatcher.UIThread.InvokeAsync(async () => await ExecuteAiMove());
            });
        }

        if (!_gameLogicService.IsWhiteTurn) 
        {
            // _logger.LogInformation("[BoardView] : AI's turn to move.");
            Console.WriteLine("[BoardView] : AI's turn to move.");
            ExecuteAiMove().ConfigureAwait(false);
        }
    }
    
    private void DrawBoard()
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _grid.Children.Clear();
        
            Console.WriteLine("[BoardView] : Drawing board.");
        
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    var cellColor = (x + y) % 2 == 0 ? Brushes.BurlyWood : Brushes.SaddleBrown;
                    var border = new Border
                    {
                        Background = cellColor,
                        Width = 80,
                        Height = 80,
                        CornerRadius = new CornerRadius(6),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Child = new TextBlock
                        {
                            Text = GetCheckerSymbol(x, y),
                            FontSize = 36,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = (x + y) % 2 == 0 ? Brushes.Black : Brushes.White
                        },
                        Tag = (x, y)
                    };
                    
                    border.PointerPressed += OnCellClick;
                    _grid.Children.Add(border);
                }
            }
        });
    }


    private string GetCheckerSymbol(int x, int y)
    {
        var cell = _gameLogicService.Board.Cells[x, y];
        return cell.Value switch
        {
            CellValueEnum.CellValue.WhiteChecker => "⚪",
            CellValueEnum.CellValue.BlackChecker => "⚫",
            CellValueEnum.CellValue.WhiteKing => "\ud83d\udd34",
            CellValueEnum.CellValue.BlackKing => "\ud83d\udd35",
            _ => ""
        };
    }

    private async void OnCellClick(object? sender, PointerPressedEventArgs e)
    {
        if (!_gameLogicService.IsWhiteTurn) return;

        if (sender is not Border border || border.Tag is not (int x, int y)) return;

        var clickedCell = _gameLogicService.Board.Cells[x, y];

        if (_selectedCell == null)
        {
            if (clickedCell.Value == CellValueEnum.CellValue.Empty) return;
            if ((_gameLogicService.IsWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.BlackChecker) ||
                (!_gameLogicService.IsWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.WhiteChecker)) return;

            _selectedCell = clickedCell;
        }
        else
        {
            if (_gameLogicService.Move(_selectedCell.X, _selectedCell.Y, x, y))
            {
                HighlightMove(_selectedCell.X, _selectedCell.Y, x, y);
                _selectedCell = null;
                DrawBoard();

                _gameLogicService.SetIsWhiteTurn(false); 
                Console.WriteLine("[OnCellClick      ] : Player moved. Now AI's turn.");
                
                await Task.Delay(500);
                await ExecuteAiMove();
            }
            else
            {
                _selectedCell = null;
            }
        }
    }

    private async Task ExecuteAiMove()
    {
        if (_gameLogicService.IsWhiteTurn) return;

        var aiMove = await _aiGameLogicService.GetHintAsync();

        if (!string.IsNullOrEmpty(aiMove))
        {
            var moveParts = aiMove.Split(" to ");

            if (moveParts.Length == 2)
            {
                var fromCoordinates = moveParts[0].Substring(5);
                var (fromX, fromY) = ParseCoordinates(fromCoordinates);

                var toPart = moveParts[1].Split("),")[0];
                var (toX, toY) = ParseCoordinates(toPart.Trim());

                if (_aiGameLogicService.Move(fromX, fromY, toX, toY))
                {
                    DrawBoard();
                    
                    Console.WriteLine($"[BoardView ExecuteAiMove] : Current turn: {(_gameLogicService.IsWhiteTurn ? "White" : "Black")}");

                    _gameLogicService.SetIsWhiteTurn(true);
                }
            }
        }
    }

    private void HighlightMove(int fromX, int fromY, int toX, int toY)
    {
        foreach (Control child in _grid.Children)
        {
            if (child is Border border && border.Tag is (int x, int y))
            {
                bool isHighlighted = (x == fromX && y == fromY) || (x == toX && y == toY);
                border.BorderBrush = isHighlighted ? Brushes.Red : Brushes.Black;
                border.BorderThickness = new Thickness(isHighlighted ? 3 : 1);
            }
        }
    }

    private (int, int) ParseCoordinates(string input)
    {
        var trimmed = input.Trim('(', ')').Trim();
        var parts = trimmed.Split(',');
        return (int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
    }

}