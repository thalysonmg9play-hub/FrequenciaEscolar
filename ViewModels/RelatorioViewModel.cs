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

public class FaltaItem
{
    public string NomeAluno { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string NomeTurma { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string? Observacao { get; set; }
}

public class RelatorioViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Turma> Turmas { get; } = new();
    public ObservableCollection<FaltaItem> Faltas { get; } = new();

    private Turma? _turmaSelecionada;
    public Turma? TurmaSelecionada
    {
        get => _turmaSelecionada;
        set => SetProperty(ref _turmaSelecionada, value);
    }

    private DateTime? _dataInicio = DateTime.Today.AddDays(-30);
    public DateTime? DataInicio
    {
        get => _dataInicio;
        set => SetProperty(ref _dataInicio, value);
    }

    private DateTime? _dataFim = DateTime.Today;
    public DateTime? DataFim
    {
        get => _dataFim;
        set => SetProperty(ref _dataFim, value);
    }

    private string _mensagemStatus = string.Empty;
    public string MensagemStatus
    {
        get => _mensagemStatus;
        set => SetProperty(ref _mensagemStatus, value);
    }

    public ICommand GerarRelatorioCommand { get; }

    public RelatorioViewModel()
    {
        _databaseService = new DatabaseService();
        GerarRelatorioCommand = new RelayCommand(async () => await GerarRelatorioAsync());
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

    private async Task GerarRelatorioAsync()
    {
        Faltas.Clear();
        MensagemStatus = "Gerando relatório...";

        if (DataInicio == null || DataFim == null)
        {
            MensagemStatus = "Selecione as datas.";
            return;
        }

        if (DataInicio > DataFim)
        {
            MensagemStatus = "A data início não pode ser maior que a data fim.";
            return;
        }

        try
        {
            // Agora usa 1 única consulta otimizada
            var faltas = await _databaseService.ObterFaltasPorPeriodoAsync(
                DataInicio.Value,
                DataFim.Value,
                TurmaSelecionada?.Id);

            var resultado = faltas.Select(f => new FaltaItem
            {
                NomeAluno = f.Aluno?.Nome ?? "—",
                Matricula = f.Aluno?.Matricula ?? "—",
                NomeTurma = f.Aluno?.Turma?.Nome ?? "—",
                Data = f.Data,
                Observacao = f.Observacao
            }).ToList();

            foreach (var item in resultado)
                Faltas.Add(item);

            MensagemStatus = resultado.Count == 0
                ? "Nenhuma falta encontrada no período."
                : $"{resultado.Count} falta(s) encontrada(s).";
        }
        catch
        {
            MensagemStatus = "Erro ao gerar o relatório.";
        }
    }
}