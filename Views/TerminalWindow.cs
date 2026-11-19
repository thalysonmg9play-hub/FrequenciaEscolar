using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

namespace FrequenciaEscolar.Views;

public class TerminalWindow : Window
{
    public TerminalWindow()
    {
        Title = "Terminal - Recursos Utilizados";
        Width = 720;
        Height = 520;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Background = new SolidColorBrush(Color.Parse("#0C0C0C"));
        CanResize = true;

        var scroll = new ScrollViewer
        {
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Margin = new Avalonia.Thickness(15)
        };

        var asciiText = new TextBlock
        {
            FontFamily = new FontFamily("Consolas, Courier New, monospace"),
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.Parse("#00FF41")),
            TextWrapping = TextWrapping.NoWrap
        };

        asciiText.Text =
@"╔══════════════════════════════════════════════════════════════╗
║           FREQUÊNCIA ESCOLAR - STACK TECNOLÓGICO             ║
╚══════════════════════════════════════════════════════════════╝

   █████╗ ██╗   ██╗ █████╗ ██╗      ██████╗ ███╗   ██╗██╗ █████╗ 
  ██╔══██╗██║   ██║██╔══██╗██║     ██╔═══██╗████╗  ██║██║██╔══██╗
  ███████║██║   ██║███████║██║     ██║   ██║██╔██╗ ██║██║███████║
  ██╔══██║╚██╗ ██╔╝██╔══██║██║     ██║   ██║██║╚██╗██║██║██╔══██║
  ██║  ██║ ╚████╔╝ ██║  ██║███████╗╚██████╔╝██║ ╚████║██║██║  ██║
  ╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝╚═╝  ╚═╝

┌─────────────────────── RECURSOS UTILIZADOS ───────────────────────┐
│                                                                   │
│  • Avalonia UI .................. Framework multiplataforma       │
│  • .NET 10 ...................... Runtime e linguagem C#         │
│  • Entity Framework Core ........ ORM                            │
│  • SQLite ....................... Banco de dados local           │
│  • CommunityToolkit.Mvvm ........ Padrão MVVM                    │
│  • MessageBox.Avalonia .......... Diálogos                       │
│                                                                   │
│  Desenvolvido com auxílio de Grok IA (xAI)                       │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘

> Sistema pronto para uso educacional
> Feche a janela para sair";

        scroll.Content = asciiText;
        Content = scroll;
    }
}