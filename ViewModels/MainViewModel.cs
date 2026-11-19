using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrequenciaEscolar.Services;
using FrequenciaEscolar.Views;
using System.Windows.Input;

namespace FrequenciaEscolar.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    private string _titulo = "Sistema de Frequência Escolar";
    public string Titulo
    {
        get => _titulo;
        set => SetProperty(ref _titulo, value);
    }

    private string _statusMensagem = "Pronto";
    public string StatusMensagem
    {
        get => _statusMensagem;
        set => SetProperty(ref _statusMensagem, value);
    }

    private object? _conteudoAtual;
    public object? ConteudoAtual
    {
        get => _conteudoAtual;
        set => SetProperty(ref _conteudoAtual, value);
    }

    public ICommand IrParaTurmasCommand { get; }
    public ICommand IrParaAlunosCommand { get; }
    public ICommand IrParaFrequenciaCommand { get; }
    public ICommand IrParaInicioCommand { get; }
    public ICommand IrParaRelatorioCommand { get; }
    public ICommand IrParaSobreCommand { get; }

    public MainViewModel()
    {
        _databaseService = new DatabaseService();
        ConteudoAtual = null;

        IrParaTurmasCommand = new RelayCommand(IrParaTurmas);
        IrParaAlunosCommand = new RelayCommand(IrParaAlunos);
        IrParaFrequenciaCommand = new RelayCommand(IrParaFrequencia);
        IrParaInicioCommand = new RelayCommand(IrParaInicio);
        IrParaRelatorioCommand = new RelayCommand(IrParaRelatorio);
        IrParaSobreCommand = new RelayCommand(AbrirSobre);
    }

    private void IrParaTurmas()
    {
        StatusMensagem = "Turmas";
        var view = new Views.TurmasView
        {
            DataContext = new TurmasViewModel()
        };
        ConteudoAtual = view;
    }

    private void IrParaAlunos()
    {
        StatusMensagem = "Alunos";
        var view = new Views.AlunosView
        {
            DataContext = new AlunosViewModel()
        };
        ConteudoAtual = view;
    }

    private void IrParaFrequencia()
    {
        StatusMensagem = "Frequência";
        var view = new Views.FrequenciaView
        {
            DataContext = new FrequenciaViewModel()
        };
        ConteudoAtual = view;
    }

    private void IrParaInicio()
    {
        StatusMensagem = "Pronto";
        ConteudoAtual = null;
    }

    private void IrParaRelatorio()
    {
        StatusMensagem = "Relatório de Faltas";
        var view = new Views.RelatorioView
        {
            DataContext = new RelatorioViewModel()
        };
        ConteudoAtual = view;
    }

    private void AbrirSobre()
    {
        var about = new AboutWindow();
        about.Show();
    }
}