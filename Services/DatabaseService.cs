using FrequenciaEscolar.Data;
using FrequenciaEscolar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrequenciaEscolar.Services;

public class DatabaseService
{
    private AppDbContext CreateContext() => new AppDbContext();

    /// <summary>
    /// Garante que o banco de dados e as tabelas existam.
    /// </summary>
    public async Task InicializarAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    // ==================== TURMAS ====================

    public async Task<List<Turma>> ObterTurmasAsync()
    {
        await using var context = CreateContext();
        return await context.Turmas
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<Turma?> ObterTurmaPorIdAsync(int id)
    {
        await using var context = CreateContext();
        return await context.Turmas
            .AsNoTracking()
            .Include(t => t.Alunos)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AdicionarTurmaAsync(Turma turma)
    {
        await using var context = CreateContext();
        context.Turmas.Add(turma);
        await context.SaveChangesAsync();
    }

    public async Task AtualizarTurmaAsync(Turma turma)
    {
        await using var context = CreateContext();
        context.Turmas.Update(turma);
        await context.SaveChangesAsync();
    }

    public async Task RemoverTurmaAsync(Turma turma)
    {
        await using var context = CreateContext();
        context.Turmas.Remove(turma);
        await context.SaveChangesAsync();
    }

    // ==================== ALUNOS ====================

    public async Task<List<Aluno>> ObterAlunosAsync()
    {
        await using var context = CreateContext();
        return await context.Alunos
            .AsNoTracking()
            .Include(a => a.Turma)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<List<Aluno>> ObterAlunosPorTurmaAsync(int turmaId)
    {
        await using var context = CreateContext();
        return await context.Alunos
            .AsNoTracking()
            .Where(a => a.TurmaId == turmaId)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task AdicionarAlunoAsync(Aluno aluno)
    {
        await using var context = CreateContext();
        context.Alunos.Add(aluno);
        await context.SaveChangesAsync();
    }

    public async Task AtualizarAlunoAsync(Aluno aluno)
    {
        await using var context = CreateContext();
        context.Alunos.Update(aluno);
        await context.SaveChangesAsync();
    }

    public async Task RemoverAlunoAsync(Aluno aluno)
    {
        await using var context = CreateContext();
        context.Alunos.Remove(aluno);
        await context.SaveChangesAsync();
    }

    // ==================== FREQUÊNCIAS ====================

    public async Task<List<Frequencia>> ObterFrequenciasPorDataAsync(DateTime data)
    {
        await using var context = CreateContext();
        return await context.Frequencias
            .AsNoTracking()
            .Include(f => f.Aluno)
            .Where(f => f.Data.Date == data.Date)
            .ToListAsync();
    }

    public async Task<List<Frequencia>> ObterFrequenciasPorAlunoAsync(int alunoId)
    {
        await using var context = CreateContext();
        return await context.Frequencias
            .AsNoTracking()
            .Where(f => f.AlunoId == alunoId)
            .OrderByDescending(f => f.Data)
            .ToListAsync();
    }

    /// <summary>
    /// Método otimizado para relatório de faltas (1 única consulta).
    /// </summary>
    public async Task<List<Frequencia>> ObterFaltasPorPeriodoAsync(
        DateTime inicio,
        DateTime fim,
        int? turmaId = null)
    {
        await using var context = CreateContext();

        var query = context.Frequencias
            .AsNoTracking()
            .Include(f => f.Aluno)
                .ThenInclude(a => a!.Turma)
            .Where(f => !f.Presente
                     && f.Data.Date >= inicio.Date
                     && f.Data.Date <= fim.Date);

        if (turmaId.HasValue)
        {
            query = query.Where(f => f.Aluno!.TurmaId == turmaId.Value);
        }

        return await query
            .OrderBy(f => f.Data)
            .ThenBy(f => f.Aluno!.Nome)
            .ToListAsync();
    }

    public async Task SalvarFrequenciaAsync(Frequencia frequencia)
    {
        await using var context = CreateContext();

        var existente = await context.Frequencias
            .FirstOrDefaultAsync(f =>
                f.AlunoId == frequencia.AlunoId &&
                f.Data.Date == frequencia.Data.Date);

        if (existente != null)
        {
            existente.Presente = frequencia.Presente;
            existente.Observacao = frequencia.Observacao;
        }
        else
        {
            context.Frequencias.Add(frequencia);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Salvamento em lote otimizado (muito mais rápido).
    /// </summary>
    public async Task SalvarFrequenciasAsync(IEnumerable<Frequencia> frequencias)
    {
        var lista = frequencias.ToList();
        if (lista.Count == 0) return;

        await using var context = CreateContext();

        var datas = lista.Select(f => f.Data.Date).Distinct().ToList();
        var alunosIds = lista.Select(f => f.AlunoId).Distinct().ToList();

        // Busca todos os registros existentes de uma vez
        var existentes = await context.Frequencias
            .Where(f => alunosIds.Contains(f.AlunoId) && datas.Contains(f.Data.Date))
            .ToListAsync();

        foreach (var freq in lista)
        {
            var existente = existentes.FirstOrDefault(e =>
                e.AlunoId == freq.AlunoId && e.Data.Date == freq.Data.Date);

            if (existente != null)
            {
                existente.Presente = freq.Presente;
                existente.Observacao = freq.Observacao;
            }
            else
            {
                context.Frequencias.Add(freq);
            }
        }

        // Apenas 1 SaveChanges no final
        await context.SaveChangesAsync();
    }
}