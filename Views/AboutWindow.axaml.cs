using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace FrequenciaEscolar.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void Fechar_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AbrirTerminal_Click(object? sender, PointerPressedEventArgs e)
    {
        var terminal = new TerminalWindow();
        terminal.Show();
    }
}