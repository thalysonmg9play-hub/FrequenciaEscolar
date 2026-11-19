using FrequenciaEscolar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace FrequenciaEscolar.Data;

public class AppDbContext : DbContext
{
    public DbSet<Aluno> Alunos { get; set; } = null!;
    public DbSet<Turma> Turmas { get; set; } = null!;
    public DbSet<Frequencia> Frequencias { get; set; } = null!;

    private readonly string _dbPath;

    public AppDbContext()
    {
        var pasta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FrequenciaEscolar");

        Directory.CreateDirectory(pasta);
        _dbPath = Path.Combine(pasta, "frequencia.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relacionamento: Uma Turma tem muitos Alunos
        modelBuilder.Entity<Aluno>()
            .HasOne(a => a.Turma)
            .WithMany(t => t.Alunos)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento: Um Aluno tem muitas Frequências
        modelBuilder.Entity<Frequencia>()
            .HasOne(f => f.Aluno)
            .WithMany()
            .HasForeignKey(f => f.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // ==================== ÍNDICES PARA PERFORMANCE ====================
        
        // Impede frequência duplicada no mesmo dia + acelera buscas
        modelBuilder.Entity<Frequencia>()
            .HasIndex(f => new { f.AlunoId, f.Data })
            .IsUnique();

        // Acelera relatórios por período
        modelBuilder.Entity<Frequencia>()
            .HasIndex(f => f.Data);

        // Acelera busca por matrícula e filtro por turma
        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Matricula);

        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.TurmaId);

        base.OnModelCreating(modelBuilder);
    }
}