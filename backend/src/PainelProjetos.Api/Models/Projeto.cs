namespace PainelProjetos.Api.Models;

public class Projeto
{
    public int Id { get; set; }
    public string CodigoRm { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal HorasVendidas { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFimPrevista { get; set; }
    public SituacaoProjeto Situacao { get; set; }
    public DateTime SincronizadoEm { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public ICollection<Alocacao> Alocacoes { get; set; } = new List<Alocacao>();
    public ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();
    public ICollection<Apontamento> Apontamentos { get; set; } = new List<Apontamento>();
}