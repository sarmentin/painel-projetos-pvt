namespace PainelProjetos.Api.Models;

public class Alocacao
{
    public int Id { get; set; }
    public int ProjetoId { get; set; }
    public int AnalistaId { get; set; }
    public decimal HorasPlanejadas { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }

    public Projeto Projeto { get; set; } = null!;
    public Analista Analista { get; set; } = null!;
}