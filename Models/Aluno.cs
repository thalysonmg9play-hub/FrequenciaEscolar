namespace FrequenciaEscolar.Models;

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public int TurmaId { get; set; }

    // Navegação
    public Turma? Turma { get; set; }
}