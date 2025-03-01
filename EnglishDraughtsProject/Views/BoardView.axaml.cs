using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Input;
using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Views;

public partial class BoardView : UserControl
{
    private readonly GameLogicService _gameLogicService;
    private readonly UniformGrid _grid;
    private Cell? _selectedCell = null;

    public BoardView()
    {
        InitializeComponent();

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
        
        var hintButton = new Button
        {
            Content = "Get Hint",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 20, 0, 0)
        };
        hintButton.Click += HintButton_Click;

        Content = new Grid
        {
            Children =
            {
                new Border
                {
                    Background = new LinearGradientBrush
                    {
                        GradientStops =
                        {
                            new GradientStop(Colors.LightSteelBlue, 0),
                            new GradientStop(Colors.White, 1)
                        },
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative)
                    }
                },
                boardContainer,
                hintButton
            }
        };

        _gameLogicService = new GameLogicService(new Board(), new AiService("OPENAI_API_KEY", new LoggerFactory().CreateLogger<AiService>()));
        DrawBoard();
    }
    
    private async void HintButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var hint = await _gameLogicService.GetHintAsync();
        var parentWindow = this.VisualRoot as Window;

        if (parentWindow != null)
        {
            var hintDialog = new HintDialog(hint);
            await hintDialog.ShowDialog(parentWindow);
        }
        else
        {
            Console.WriteLine("Error: Cannot find parent window.");
        }
    }

    private void DrawBoard()
    {
        _grid.Children.Clear();

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
    }

    private string GetCheckerSymbol(int x, int y)
    {
        var cell = _gameLogicService.board.Cells[x, y];
        return cell.Value switch
        {
            CellValueEnum.CellValue.WhiteChecker => "⚪",
            CellValueEnum.CellValue.BlackChecker => "⚫",
            CellValueEnum.CellValue.WhiteKing => "◉",
            CellValueEnum.CellValue.BlackKing => "◎",
            _ => ""
        };
    }

    private void OnCellClick(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border || border.Tag is not (int x, int y)) return;

        var clickedCell = _gameLogicService.board.Cells[x, y];

        if (_selectedCell == null)
        {
            if (clickedCell.Value == CellValueEnum.CellValue.Empty)
            {
                return;
            }
            if ((_gameLogicService._isWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.BlackChecker) ||
                (!_gameLogicService._isWhiteTurn && clickedCell.Value == CellValueEnum.CellValue.WhiteChecker))
            {
                return;
            }
            _selectedCell = clickedCell;
        }
        else
        {
            if (_gameLogicService.Move(_selectedCell.X, _selectedCell.Y, x, y))
            {
                _selectedCell = null;
                DrawBoard();
            }
            else
            {
                _selectedCell = null;
            }
        }
    }
}
