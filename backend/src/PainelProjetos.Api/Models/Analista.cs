namespace PainelProjetos.Api.Models;

public class Analista
{
    public int Id { get; set; }
    public string CodigoRm { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }

    public ICollection<Alocacao> Alocacoes { get; set; } = new List<Alocacao>();
    public ICollection<Apontamento> Apontamentos { get; set; } = new List<Apontamento>();
}