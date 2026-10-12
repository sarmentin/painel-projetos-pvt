using PainelProjetos.Api.Models;

namespace PainelProjetos.Api.Data;

// Dados de exemplo: representam o que já teria sido sincronizado do RM
// e registrado no portal. Os 4 projetos são os da seção 4.4 do DESIGN.md.
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Só popula se o banco estiver vazio
        if (db.Projetos.Any())
            return;

        var sincronizadoEm = DateTime.UtcNow;

        var metalurgica = new Cliente { CodigoRm = "C-001", Nome = "Metalúrgica Serra", Cnpj = "12.345.678/0001-90" };
        var hospital = new Cliente { CodigoRm = "C-002", Nome = "Hospital Vida", Cnpj = "98.765.432/0001-10" };

        var joao = NovoAnalista("00101", "João Silva");
        var maria = NovoAnalista("00102", "Maria Souza");
        var jose = NovoAnalista("00103", "José Santos");

        // Saudável: consumo 30%, avanço físico 30%
        var portal = NovoProjeto("PRJ-0001", "Portal do Colaborador", hospital, sincronizadoEm);
        AdicionarEtapa(portal, "Levantamento", 30, true);
        AdicionarEtapa(portal, "Desenvolvimento", 50, false);
        AdicionarEtapa(portal, "Testes", 20, false);
        Apontar(portal, joao, 30);
        Apontar(portal, maria, 20);
        Apontar(portal, jose, 10);
        Apontar(portal, jose, 8, StatusSincronizacao.Pendente); // pendente: não conta no realizado

        // Atenção: consumo 82,5%, avanço físico 80%
        var contabil = NovoProjeto("PRJ-0002", "Migração Contábil", hospital, sincronizadoEm);
        AdicionarEtapa(contabil, "Levantamento", 30, true);
        AdicionarEtapa(contabil, "Desenvolvimento", 50, true);
        AdicionarEtapa(contabil, "Testes", 20, false);
        Apontar(contabil, joao, 80);
        Apontar(contabil, maria, 50);
        Apontar(contabil, jose, 35);

        // Crítico por consumo: 105%, avanço físico 90%
        var saude = NovoProjeto("PRJ-0003", "Implantação RM Saúde", hospital, sincronizadoEm);
        AdicionarEtapa(saude, "Levantamento", 20, true);
        AdicionarEtapa(saude, "Desenvolvimento", 50, true);
        AdicionarEtapa(saude, "Homologação", 20, true);
        AdicionarEtapa(saude, "Go-live", 10, false);
        Apontar(saude, joao, 110);
        Apontar(saude, maria, 60);
        Apontar(saude, jose, 40);

        // Crítico por desvio: consumo 85%, avanço físico 60%
        var folha = NovoProjeto("PRJ-0004", "Implantação RM Folha", metalurgica, sincronizadoEm);
        AdicionarEtapa(folha, "Levantamento", 20, true);
        AdicionarEtapa(folha, "Desenvolvimento", 40, true);
        AdicionarEtapa(folha, "Testes", 30, false);
        AdicionarEtapa(folha, "Go-live", 10, false);
        Apontar(folha, joao, 90);
        Apontar(folha, maria, 50);
        Apontar(folha, jose, 30);

        var projetos = new[] { portal, contabil, saude, folha };

        // Mesma alocação em todos: 100 + 50 + 30 = 180h planejadas
        foreach (var projeto in projetos)
        {
            Alocar(projeto, joao, 100);
            Alocar(projeto, maria, 50);
            Alocar(projeto, jose, 30);
        }

        db.Projetos.AddRange(projetos);
        db.SaveChanges();
    }

    private static Analista NovoAnalista(string codigoRm, string nome) => new()
    {
        CodigoRm = codigoRm,
        Nome = nome,
        Email = $"{nome.Split(' ')[0].ToLower()}@pvt.com.br",
        Ativo = true
    };

    private static Projeto NovoProjeto(string codigoRm, string nome, Cliente cliente, DateTime sincronizadoEm) => new()
    {
        CodigoRm = codigoRm,
        Nome = nome,
        Cliente = cliente,
        HorasVendidas = 200,
        DataInicio = new DateOnly(2026, 6, 1),
        DataFimPrevista = new DateOnly(2026, 12, 31),
        Situacao = SituacaoProjeto.Ativo,
        SincronizadoEm = sincronizadoEm
    };

    private static void AdicionarEtapa(Projeto projeto, string nome, decimal peso, bool concluida) =>
        projeto.Etapas.Add(new Etapa
        {
            Nome = nome,
            Peso = peso,
            Concluida = concluida,
            DataConclusao = concluida ? new DateOnly(2026, 9, 1) : null
        });

    private static void Alocar(Projeto projeto, Analista analista, decimal horas) =>
        projeto.Alocacoes.Add(new Alocacao
        {
            Analista = analista,
            HorasPlanejadas = horas,
            DataInicio = projeto.DataInicio,
            DataFim = projeto.DataFimPrevista
        });

    // Divide o total em apontamentos diários de até 8h,
    // respeitando a regra de no máximo 24h por apontamento
    private static void Apontar(Projeto projeto, Analista analista, decimal totalHoras,
        StatusSincronizacao status = StatusSincronizacao.Sincronizado)
    {
        var data = projeto.DataInicio;
        var restante = totalHoras;

        while (restante > 0)
        {
            var horas = Math.Min(8m, restante);
            projeto.Apontamentos.Add(new Apontamento
            {
                Analista = analista,
                Data = data,
                Horas = horas,
                Descricao = "Atividades do projeto",
                StatusSincronizacao = status,
                CodigoRm = status == StatusSincronizacao.Sincronizado
                    ? "AP-" + Guid.NewGuid().ToString("N")[..8]
                    : null
            });

            data = data.AddDays(1);
            restante -= horas;
        }
    }
}