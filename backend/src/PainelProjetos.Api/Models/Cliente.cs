namespace PainelProjetos.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    public string CodigoRm { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;

    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}