using System;

namespace FrequenciaEscolar.Models;

public class Frequencia
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public DateTime Data { get; set; }
    public bool Presente { get; set; }
    public string? Observacao { get; set; }

    // Navegação
    public Aluno? Aluno { get; set; }
}