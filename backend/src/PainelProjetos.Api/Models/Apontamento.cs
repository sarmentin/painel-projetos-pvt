namespace PainelProjetos.Api.Models;

public class Apontamento
{
    public int Id { get; set; }
    public int ProjetoId { get; set; }
    public int AnalistaId { get; set; }
    public DateOnly Data { get; set; }
    public decimal Horas { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public StatusSincronizacao StatusSincronizacao { get; set; }
    public string? CodigoRm { get; set; }
    public string? MotivoRejeicao { get; set; }

    public Projeto Projeto { get; set; } = null!;
    public Analista Analista { get; set; } = null!;
}