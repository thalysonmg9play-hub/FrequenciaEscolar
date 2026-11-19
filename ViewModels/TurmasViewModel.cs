using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrequenciaEscolar.Models;
using FrequenciaEscolar.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequenciaEscolar.ViewModels;

public class TurmasViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Turma> Turmas { get; } = new();

    private Turma? _turmaSelecionada;
    public Turma? TurmaSelecionada
    {
        get => _turmaSelecionada;
        set => SetProperty(ref _turmaSelecionada, value);
    }

    private string _nomeTurma = string.Empty;
    public string NomeTurma
    {
        get => _nomeTurma;
        set => SetProperty(ref _nomeTurma, value);
    }

    private string _anoLetivo = string.Empty;
    public string AnoLetivo
    {
        get => _anoLetivo;
        set => SetProperty(ref _anoLetivo, value);
    }

    private string _tituloFormulario = "Nova Turma";
    public string TituloFormulario
    {
        get => _tituloFormulario;
        set => SetProperty(ref _tituloFormulario, value);
    }

    private bool _modoEdicao;
    public bool ModoEdicao
    {
        get => _modoEdicao;
        set => SetProperty(ref _modoEdicao, value);
    }

    private string _mensagem = string.Empty;
    public string Mensagem
    {
        get => _mensagem;
        set => SetProperty(ref _mensagem, value);
    }

    public ICommand NovaTurmaCommand { get; }
    public ICommand EditarTurmaCommand { get; }
    public ICommand SalvarTurmaCommand { get; }
    public ICommand ExcluirTurmaCommand { get; }

    public TurmasViewModel()
    {
        _databaseService = new DatabaseService();

        NovaTurmaCommand = new RelayCommand(NovaTurma);
        EditarTurmaCommand = new RelayCommand(EditarTurma);
        SalvarTurmaCommand = new RelayCommand(async () => await SalvarTurmaAsync());
        ExcluirTurmaCommand = new RelayCommand(async () => await ExcluirTurmaAsync());

        _ = CarregarTurmasAsync();
    }

    private async Task CarregarTurmasAsync()
    {
        try
        {
            var lista = await _databaseService.ObterTurmasAsync();
            Turmas.Clear();
            foreach (var turma in lista)
                Turmas.Add(turma);
        }
        catch
        {
            Mensagem = "Erro ao carregar turmas.";
        }
    }

    private void NovaTurma()
    {
        ModoEdicao = false;
        TituloFormulario = "Nova Turma";
        NomeTurma = string.Empty;
        AnoLetivo = string.Empty;
        TurmaSelecionada = null;
        Mensagem = string.Empty;
    }

    private void EditarTurma()
    {
        if (TurmaSelecionada == null)
        {
            Mensagem = "Selecione uma turma para editar.";
            return;
        }

        ModoEdicao = true;
        TituloFormulario = "Editar Turma";
        NomeTurma = TurmaSelecionada.Nome;
        AnoLetivo = TurmaSelecionada.AnoLetivo;
        Mensagem = string.Empty;
    }

    private async Task SalvarTurmaAsync()
    {
        if (string.IsNullOrWhiteSpace(NomeTurma))
        {
            Mensagem = "Informe o nome da turma.";
            return;
        }

        if (string.IsNullOrWhiteSpace(AnoLetivo))
        {
            Mensagem = "Informe o ano letivo.";
            return;
        }

        try
        {
            if (ModoEdicao && TurmaSelecionada != null)
            {
                TurmaSelecionada.Nome = NomeTurma.Trim();
                TurmaSelecionada.AnoLetivo = AnoLetivo.Trim();
                await _databaseService.AtualizarTurmaAsync(TurmaSelecionada);
                Mensagem = "Turma atualizada com sucesso!";
            }
            else
            {
                var novaTurma = new Turma
                {
                    Nome = NomeTurma.Trim(),
                    AnoLetivo = AnoLetivo.Trim()
                };
                await _databaseService.AdicionarTurmaAsync(novaTurma);
                Mensagem = "Turma cadastrada com sucesso!";
            }

            await CarregarTurmasAsync();
            NovaTurma();
        }
        catch
        {
            Mensagem = "Erro ao salvar a turma.";
        }
    }

    private async Task ExcluirTurmaAsync()
    {
        if (TurmaSelecionada == null)
        {
            Mensagem = "Selecione uma turma para excluir.";
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard(
            "Confirmar exclusão",
            $"Deseja realmente excluir a turma \"{TurmaSelecionada.Nome}\"?\n\nOs alunos vinculados também poderão ser afetados.",
            ButtonEnum.YesNo,
            Icon.Warning);

        var resultado = await box.ShowAsync();

        if (resultado != ButtonResult.Yes)
            return;

        try
        {
            await _databaseService.RemoverTurmaAsync(TurmaSelecionada);
            Mensagem = "Turma excluída com sucesso!";
            await CarregarTurmasAsync();
            NovaTurma();
        }
        catch
        {
            Mensagem = "Erro ao excluir a turma.";
        }
    }
}