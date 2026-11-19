using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FrequenciaEscolar.Services;
using System;

namespace FrequenciaEscolar;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Inicializa o banco de dados
        var databaseService = new DatabaseService();
        databaseService.InicializarAsync().GetAwaiter().GetResult();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.MainWindow
            {
                DataContext = new ViewModels.MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}