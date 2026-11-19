using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrequenciaEscolar.Models;
using FrequenciaEscolar.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Collections.Generic;

namespace FrequenciaEscolar.ViewModels;

public class AlunosViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Aluno> Alunos { get; } = new();
    public ObservableCollection<Aluno> AlunosFiltrados { get; } = new();
    public ObservableCollection<Turma> Turmas { get; } = new();

    private Aluno? _alunoSelecionado;
    public Aluno? AlunoSelecionado
    {
        get => _alunoSelecionado;
        set => SetProperty(ref _alunoSelecionado, value);
    }

    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set => SetProperty(ref _nome, value);
    }

    private string _matricula = string.Empty;
    public string Matricula
    {
        get => _matricula;
        set => SetProperty(ref _matricula, value);
    }

    private Turma? _turmaSelecionada;
    public Turma? TurmaSelecionada
    {
        get => _turmaSelecionada;
        set => SetProperty(ref _turmaSelecionada, value);
    }

    // Filtro da lista
    private Turma? _filtroTurma;
    public Turma? FiltroTurma
    {
        get => _filtroTurma;
        set
        {
            if (SetProperty(ref _filtroTurma, value))
                AplicarFiltro();
        }
    }

    private string _tituloFormulario = "Novo Aluno";
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

    public ICommand NovoAlunoCommand { get; }
    public ICommand EditarAlunoCommand { get; }
    public ICommand SalvarAlunoCommand { get; }
    public ICommand ExcluirAlunoCommand { get; }
    public ICommand LimparFiltroCommand { get; }
    public ICommand ImportarCsvCommand { get; }

    public AlunosViewModel()
    {
        _databaseService = new DatabaseService();

        NovoAlunoCommand = new RelayCommand(NovoAluno);
        EditarAlunoCommand = new RelayCommand(EditarAluno);
        SalvarAlunoCommand = new RelayCommand(async () => await SalvarAlunoAsync());
        ExcluirAlunoCommand = new RelayCommand(async () => await ExcluirAlunoAsync());
        LimparFiltroCommand = new RelayCommand(LimparFiltro);
        ImportarCsvCommand = new RelayCommand(async () => await ImportarCsvAsync());

        _ = CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        try
        {
            var turmas = await _databaseService.ObterTurmasAsync();
            Turmas.Clear();
            foreach (var t in turmas)
                Turmas.Add(t);

            var alunos = await _databaseService.ObterAlunosAsync();
            Alunos.Clear();
            foreach (var a in alunos)
                Alunos.Add(a);

            AplicarFiltro();
        }
        catch
        {
            Mensagem = "Erro ao carregar dados.";
        }
    }

    private void AplicarFiltro()
    {
        AlunosFiltrados.Clear();

        var lista = FiltroTurma == null
            ? Alunos
            : Alunos.Where(a => a.TurmaId == FiltroTurma.Id);

        foreach (var aluno in lista.OrderBy(a => a.Nome))
            AlunosFiltrados.Add(aluno);
    }

    private void LimparFiltro()
    {
        FiltroTurma = null;
    }

    private void NovoAluno()
    {
        ModoEdicao = false;
        TituloFormulario = "Novo Aluno";
        Nome = string.Empty;
        Matricula = string.Empty;
        TurmaSelecionada = null;
        AlunoSelecionado = null;
        Mensagem = string.Empty;
    }

    private void EditarAluno()
    {
        if (AlunoSelecionado == null)
        {
            Mensagem = "Selecione um aluno para editar.";
            return;
        }

        ModoEdicao = true;
        TituloFormulario = "Editar Aluno";
        Nome = AlunoSelecionado.Nome;
        Matricula = AlunoSelecionado.Matricula;
        TurmaSelecionada = Turmas.FirstOrDefault(t => t.Id == AlunoSelecionado.TurmaId);
        Mensagem = string.Empty;
    }

    private async Task SalvarAlunoAsync()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            Mensagem = "Informe o nome do aluno.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Matricula))
        {
            Mensagem = "Informe a matrícula do aluno.";
            return;
        }

        if (TurmaSelecionada == null)
        {
            Mensagem = "Selecione a turma do aluno.";
            return;
        }

        try
        {
            if (ModoEdicao && AlunoSelecionado != null)
            {
                AlunoSelecionado.Nome = Nome.Trim();
                AlunoSelecionado.Matricula = Matricula.Trim();
                AlunoSelecionado.TurmaId = TurmaSelecionada.Id;
                await _databaseService.AtualizarAlunoAsync(AlunoSelecionado);
                Mensagem = "Aluno atualizado com sucesso!";
            }
            else
            {
                var novoAluno = new Aluno
                {
                    Nome = Nome.Trim(),
                    Matricula = Matricula.Trim(),
                    TurmaId = TurmaSelecionada.Id
                };
                await _databaseService.AdicionarAlunoAsync(novoAluno);
                Mensagem = "Aluno cadastrado com sucesso!";
            }

            await CarregarDadosAsync();
            NovoAluno();
        }
        catch
        {
            Mensagem = "Erro ao salvar o aluno.";
        }
    }

    private async Task ExcluirAlunoAsync()
    {
        if (AlunoSelecionado == null)
        {
            Mensagem = "Selecione um aluno para excluir.";
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard(
            "Confirmar exclusão",
            $"Deseja realmente excluir o aluno \"{AlunoSelecionado.Nome}\"?",
            ButtonEnum.YesNo,
            Icon.Warning);

        var resultado = await box.ShowAsync();

        if (resultado != ButtonResult.Yes)
            return;

        try
        {
            await _databaseService.RemoverAlunoAsync(AlunoSelecionado);
            Mensagem = "Aluno excluído com sucesso!";
            await CarregarDadosAsync();
            NovoAluno();
        }
        catch
        {
            Mensagem = "Erro ao excluir o aluno.";
        }
    }

    private async Task ImportarCsvAsync()
    {
        try
        {
            var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            if (lifetime?.MainWindow == null)
            {
                Mensagem = "Não foi possível abrir o seletor de arquivo.";
                return;
            }

            var storageProvider = lifetime.MainWindow.StorageProvider;

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Selecionar arquivo CSV de alunos",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("CSV")
                    {
                        Patterns = new[] { "*.csv" }
                    }
                }
            });

            if (files == null || files.Count == 0)
                return;

            var file = files[0];

            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);

           var linhas = new List<string>();
    string? linha;
while ((linha = await reader.ReadLineAsync()) != null)
{
    if (!string.IsNullOrWhiteSpace(linha))
        linhas.Add(linha);
}

            if (linhas.Count < 2)
            {
                Mensagem = "O arquivo CSV está vazio ou inválido.";
                return;
            }

            int sucessos = 0;
            int erros = 0;

            var turmas = await _databaseService.ObterTurmasAsync();

            // Pula o cabeçalho
            for (int i = 1; i < linhas.Count; i++)
            {
                var colunas = linhas[i].Split(',');

                if (colunas.Length < 3)
                {
                    erros++;
                    continue;
                }

                var nome = colunas[0].Trim().Trim('"');
                var matricula = colunas[1].Trim().Trim('"');
                var nomeTurma = colunas[2].Trim().Trim('"');

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(matricula))
                {
                    erros++;
                    continue;
                }

                var turma = turmas.FirstOrDefault(t =>
                    t.Nome.Equals(nomeTurma, StringComparison.OrdinalIgnoreCase));

                if (turma == null)
                {
                    erros++;
                    continue;
                }

                var novoAluno = new Aluno
                {
                    Nome = nome,
                    Matricula = matricula,
                    TurmaId = turma.Id
                };

                await _databaseService.AdicionarAlunoAsync(novoAluno);
                sucessos++;
            }

            await CarregarDadosAsync();
            Mensagem = $"Importação concluída! {sucessos} aluno(s) cadastrado(s). {erros} linha(s) com erro.";
        }
        catch (Exception ex)
        {
            Mensagem = $"Erro ao importar: {ex.Message}";
        }
    }
}