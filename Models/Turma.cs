using System.Collections.Generic;

namespace FrequenciaEscolar.Models;

public class Turma
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string AnoLetivo { get; set; } = string.Empty;

    // Navegação
    public List<Aluno> Alunos { get; set; } = new();
}