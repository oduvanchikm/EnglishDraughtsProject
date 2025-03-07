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
    private Cell? _selectedCell = null;
    private CellValueEnum.CellValue PlayerColor;
    private bool _isPlayerTurn;
    // private readonly ILogger<BoardView> _logger;

    public BoardView()
    {
        InitializeComponent();

        // _logger = logger;
        
        _board = new Board();

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

        PlayerColor = CellValueEnum.CellValue.WhiteChecker;
        _isPlayerTurn = true;
        
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
            PlayerColor = selectedColor == "White"
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

        _isPlayerTurn = PlayerColor == CellValueEnum.CellValue.WhiteChecker; 

        if (!_isPlayerTurn) 
        {
            // _logger.LogInformation("[BoardView] : AI's turn to move.");
            Console.WriteLine("[BoardView] : AI's turn to move.");
            ExecuteAiMove().ConfigureAwait(false);
        }
    }
    
    private async void HintButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
        if (!_isPlayerTurn)
        {
            // _logger.LogInformation("[BoardView] : !AI's turn to move.");
            Console.WriteLine("[BoardView] : !AI's turn to move.");
            return;
        }
        
        // _logger.LogInformation("[BoardView] : Hint button clicked.");
        Console.WriteLine("[BoardView] : Hint button clicked.");

        var hint = await _gameLogicService.GetHintAsync();
        var parentWindow = this.VisualRoot as Window;

        if (parentWindow != null)
        {
            var hintDialog = new HintDialog(hint);
            await hintDialog.ShowDialog(parentWindow);
        }
        else
        {
            // _logger.LogInformation("[BoardView] : Error: Cannot find parent window.");
            Console.WriteLine("[BoardView] : Error: Cannot find parent window.");
        }
    }

    // private void DrawBoard()
    // {
    //     try
    //     {
    //         _grid.Children.Clear();
    //
    //         // _logger.LogInformation("[BoardView] : Drawing board.");
    //         Console.WriteLine("[BoardView] : Drawing board.");
    //
    //         for (int y = 0; y < 8; ++y)
    //         {
    //             for (int x = 0; x < 8; ++x)
    //             {
    //                 var cellColor = (x + y) % 2 == 0 ? Brushes.BurlyWood : Brushes.SaddleBrown;
    //                 var border = new Border
    //                 {
    //                     Background = cellColor,
    //                     Width = 80,
    //                     Height = 80,
    //                     CornerRadius = new CornerRadius(6),
    //                     BorderBrush = Brushes.Black,
    //                     BorderThickness = new Thickness(1),
    //                     Child = new TextBlock
    //                     {
    //                         Text = GetCheckerSymbol(x, y),
    //                         FontSize = 36,
    //                         HorizontalAlignment = HorizontalAlignment.Center,
    //                         VerticalAlignment = VerticalAlignment.Center,
    //                         Foreground = (x + y) % 2 == 0 ? Brushes.Black : Brushes.White
    //                     },
    //                     Tag = (x, y)
    //                 };
    //
    //                 border.PointerPressed += OnCellClick;
    //                 _grid.Children.Add(border);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("[BoardView] EXCEPTION!!! : " + ex.Message);
    //     }
    //     
    // }
    
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
            CellValueEnum.CellValue.WhiteKing => "◉",
            CellValueEnum.CellValue.BlackKing => "◎",
            _ => ""
        };
    }

    private async void OnCellClick(object? sender, PointerPressedEventArgs e)
    {
        if (!_isPlayerTurn) return;
        
        Console.WriteLine($"[BoardView] : _isPlayerTurn = {_isPlayerTurn}");

        if (sender is not Border border || border.Tag is not (int x, int y)) return;

        var clickedCell = _gameLogicService.Board.Cells[x, y];

        if (_selectedCell == null)
        {
            if (clickedCell.Value == CellValueEnum.CellValue.Empty) return;
            if ((_gameLogicService.IsWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.BlackChecker) ||
                (!_gameLogicService.IsWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.WhiteChecker)) return;
            _selectedCell = clickedCell;
            // _logger.LogInformation("[BoardView] : Cell ({}, {}) selected.", x, y);
            Console.WriteLine("[BoardView] : Cell ({0}, {1}) selected.", x, y);
        }
        else
        {
            if (_gameLogicService.Move(_selectedCell.X, _selectedCell.Y, x, y))
            {
                // _logger.LogInformation("[BoardView] : Move from ({}, {}) to ({}, {}) successful.", _selectedCell.X, _selectedCell.Y, x, y);
                Console.WriteLine("[BoardView] : Move from ({0}, {1}) to ({2}, {3}) successful.", _selectedCell.X, _selectedCell.Y, x, y);

                HighlightMove(_selectedCell.X, _selectedCell.Y, x, y);
                _selectedCell = null;
                DrawBoard();
                
                _isPlayerTurn = false;

                // await Task.Delay(500);
                await ExecuteAiMove();
            }
            else
            {
                // _logger.LogWarning("[BoardView] : Invalid move from ({}, {}) to ({}, {}).", _selectedCell.X, _selectedCell.Y, x, y);
                Console.WriteLine("[BoardView] : Invalid move from ({0}, {1}) to ({2}, {3}).", _selectedCell.X, _selectedCell.Y, x, y);

                _selectedCell = null;
            }
        }
    }

    private async Task ExecuteAiMove()
    {
        try
        {
            if (_isPlayerTurn) return;
        
            Console.WriteLine($"[BoardView] : _isPlayerTurn = {_isPlayerTurn}");
        
            // _logger.LogInformation("[BoardView] : AI is making a move.");
            Console.WriteLine("[BoardView] : AI is making a move.");

            var aiMove = await _aiGameLogicService.GetHintAsync();
        
            Console.WriteLine("[BoardView] : AI move 52525252: " + aiMove);
        
            if (!string.IsNullOrEmpty(aiMove))
            {
                var moveParts = aiMove.Split(" to ");

                if (moveParts.Length == 2)
                {
                    Console.WriteLine("Parsing move...");

                    var fromCoordinates = moveParts[0].Substring(5);
                    Console.WriteLine("Parsing move from: " + fromCoordinates);
                    var (fromX, fromY) = ParseCoordinates(fromCoordinates);
                    Console.WriteLine("Parsing move to: " + fromX + " " + fromY);

                    var toPart = moveParts[1].Split("),")[0];
                    Console.WriteLine("Parsing move from: " + toPart);
                    var (toX, toY) = ParseCoordinates(toPart.Trim());
                    Console.WriteLine("Parsing move to: " + toX + " " + toY);

                    Console.WriteLine($"AI move: from ({fromX}, {fromY}) to ({toX}, {toY})");

                    if (_gameLogicService.Move(fromX, fromY, toX, toY))
                    {
                        Console.WriteLine("[BoardView] : AI moved from ({0}, {1}) to ({2}, {3}) ", fromX, fromY, toX, toY);
                        DrawBoard();
                    }
                }
            }

        
            _isPlayerTurn = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void HighlightMove(int fromX, int fromY, int toX, int toY)
    {
        foreach (Border border in _grid.Children)
        {
            if (border.Tag is (int x, int y))
            {
                border.BorderBrush = (x == fromX && y == fromY) || (x == toX && y == toY) ? Brushes.Red : Brushes.Black;
                border.BorderThickness = new Thickness((x == fromX && y == fromY) || (x == toX && y == toY) ? 3 : 1);
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
