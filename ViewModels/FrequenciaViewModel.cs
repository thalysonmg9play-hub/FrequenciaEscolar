using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrequenciaEscolar.Models;
using FrequenciaEscolar.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequenciaEscolar.ViewModels;

public class AlunoFrequenciaItem : ObservableObject
{
    public Aluno Aluno { get; set; } = null!;

    private bool _presente = true;
    public bool Presente
    {
        get => _presente;
        set => SetProperty(ref _presente, value);
    }

    private string? _observacao;
    public string? Observacao
    {
        get => _observacao;
        set => SetProperty(ref _observacao, value);
    }
}

public class FrequenciaViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Turma> Turmas { get; } = new();
    public ObservableCollection<AlunoFrequenciaItem> AlunosFrequencia { get; } = new();

    private Turma? _turmaSelecionada;
    public Turma? TurmaSelecionada
    {
        get => _turmaSelecionada;
        set
        {
            if (SetProperty(ref _turmaSelecionada, value))
            {
                _ = CarregarAlunosDaTurmaAsync();
            }
        }
    }

    private DateTime? _dataSelecionada = DateTime.Today;
public DateTime? DataSelecionada
{
    get => _dataSelecionada;
    set
    {
        if (SetProperty(ref _dataSelecionada, value))
        {
            _ = CarregarAlunosDaTurmaAsync();
        }
    }
}

    private string _mensagemStatus = string.Empty;
    public string MensagemStatus
    {
        get => _mensagemStatus;
        set => SetProperty(ref _mensagemStatus, value);
    }

    public ICommand SalvarFrequenciaCommand { get; }
    public ICommand MarcarTodosPresentesCommand { get; }
    public ICommand MarcarTodosAusentesCommand { get; }

    public FrequenciaViewModel()
    {
        _databaseService = new DatabaseService();

        SalvarFrequenciaCommand = new RelayCommand(async () => await SalvarFrequenciaAsync());
        MarcarTodosPresentesCommand = new RelayCommand(MarcarTodosPresentes);
        MarcarTodosAusentesCommand = new RelayCommand(MarcarTodosAusentes);

        _ = CarregarTurmasAsync();
    }

    private async Task CarregarTurmasAsync()
    {
        try
        {
            var turmas = await _databaseService.ObterTurmasAsync();
            Turmas.Clear();
            foreach (var t in turmas)
                Turmas.Add(t);
        }
        catch
        {
            MensagemStatus = "Erro ao carregar turmas.";
        }
    }

    private async Task CarregarAlunosDaTurmaAsync()
    {
        AlunosFrequencia.Clear();
        MensagemStatus = string.Empty;

        if (TurmaSelecionada == null)
            return;

        try
        {
            var alunos = await _databaseService.ObterAlunosPorTurmaAsync(TurmaSelecionada.Id);
            if (DataSelecionada == null) return;
var frequenciasDoDia = await _databaseService.ObterFrequenciasPorDataAsync(DataSelecionada.Value);

            foreach (var aluno in alunos)
            {
                var freqExistente = frequenciasDoDia.FirstOrDefault(f => f.AlunoId == aluno.Id);

                AlunosFrequencia.Add(new AlunoFrequenciaItem
                {
                    Aluno = aluno,
                    Presente = freqExistente?.Presente ?? true,
                    Observacao = freqExistente?.Observacao
                });
            }

            if (AlunosFrequencia.Count == 0)
                MensagemStatus = "Nenhum aluno cadastrado nesta turma.";
        }
        catch
        {
            MensagemStatus = "Erro ao carregar alunos.";
        }
    }

    private void MarcarTodosPresentes()
    {
        foreach (var item in AlunosFrequencia)
            item.Presente = true;
    }

    private void MarcarTodosAusentes()
    {
        foreach (var item in AlunosFrequencia)
            item.Presente = false;
    }

    private async Task SalvarFrequenciaAsync()
    {
        if (TurmaSelecionada == null || AlunosFrequencia.Count == 0)
        {
            MensagemStatus = "Selecione uma turma com alunos.";
            return;
        }

        try
        {
            var lista = AlunosFrequencia.Select(item => new Frequencia
            {
                AlunoId = item.Aluno.Id,
                Data = DataSelecionada?.Date ?? DateTime.Today,
                Presente = item.Presente,
                Observacao = item.Observacao
            });

            await _databaseService.SalvarFrequenciasAsync(lista);
            MensagemStatus = $"Frequência salva com sucesso! ({DataSelecionada:dd/MM/yyyy})";
        }
        catch
        {
            MensagemStatus = "Erro ao salvar a frequência.";
        }
    }
}