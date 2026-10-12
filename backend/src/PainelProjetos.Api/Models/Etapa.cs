namespace PainelProjetos.Api.Models;

public class Etapa
{
    public int Id { get; set; }
    public int ProjetoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public bool Concluida { get; set; }
    public DateOnly? DataConclusao { get; set; }

    public Projeto Projeto { get; set; } = null!;
}