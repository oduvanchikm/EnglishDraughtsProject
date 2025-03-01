using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace EnglishDraughtsProject.Views;

public partial class HintDialog : Window
{
    public HintDialog()
    {
        InitializeComponent();
        this.AttachDevTools();
    }

    public HintDialog(string hint) : this()
    {
        var hintTextBlock = this.FindControl<TextBlock>("HintText");
        if (hintTextBlock != null)
        {
            hintTextBlock.Text = hint;
        }
        else
        {
            Console.WriteLine("HintText not found.");
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}