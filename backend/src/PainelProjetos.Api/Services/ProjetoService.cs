using Microsoft.EntityFrameworkCore;
using PainelProjetos.Api.Data;
using PainelProjetos.Api.Dtos;
using PainelProjetos.Api.Models;

namespace PainelProjetos.Api.Services;

public class ProjetoService
{
    private readonly AppDbContext _db;
    private readonly CalculadoraIndicadores _calculadora;

    public ProjetoService(AppDbContext db, CalculadoraIndicadores calculadora)
    {
        _db = db;
        _calculadora = calculadora;
    }

    public async Task<PainelProjetosDto> ListarAsync(StatusProjeto? status, int? clienteId, SituacaoProjeto? situacao)
    {
        // Por padrão, só projetos ativos (seção 3.2)
        var situacaoFiltro = situacao ?? SituacaoProjeto.Ativo;

        var query = _db.Projetos
            .AsNoTracking()
            .Where(p => p.Situacao == situacaoFiltro);

        if (clienteId.HasValue)
            query = query.Where(p => p.ClienteId == clienteId.Value);

        // As somas são feitas pelo próprio SQL Server
        var dados = await query
            .Select(p => new DadosProjeto
            {
                Id = p.Id,
                Nome = p.Nome,
                Cliente = p.Cliente.Nome,
                HorasVendidas = p.HorasVendidas,
                SincronizadoEm = p.SincronizadoEm,
                HorasPlanejadas = p.Alocacoes.Sum(a => (decimal?)a.HorasPlanejadas) ?? 0,
                HorasRealizadas = p.Apontamentos
                    .Where(a => a.StatusSincronizacao == StatusSincronizacao.Sincronizado)
                    .Sum(a => (decimal?)a.Horas) ?? 0,
                TemEtapas = p.Etapas.Any(),
                PesoConcluido = p.Etapas.Where(e => e.Concluida).Sum(e => (decimal?)e.Peso) ?? 0
            })
            .ToListAsync();

        var itens = dados
            .Select(d => new { Dto = MontarResumo(d), d.SincronizadoEm })
            .Where(x => status == null || x.Dto.Status == status)
            .ToList();

        var projetos = itens
            .Select(x => x.Dto)
            .OrderByDescending(p => p.Status) // críticos primeiro
            .ThenBy(p => p.Nome)
            .ToList();

        // O horário mais antigo entre os projetos retornados (seção 3.3)
        // O banco não guarda o fuso; marcamos como UTC, que é como foi gravado
        DateTime? sincronizadoEm = itens.Count > 0
            ? DateTime.SpecifyKind(itens.Min(x => x.SincronizadoEm), DateTimeKind.Utc)
            : null;

        return new PainelProjetosDto(sincronizadoEm, projetos);
    }

    private ProjetoResumoDto MontarResumo(DadosProjeto d)
    {
        // Sem etapas, não há avanço físico (seção 4.5)
        decimal? avancoFisico = d.TemEtapas ? d.PesoConcluido : null;

        var indicadores = _calculadora.Calcular(d.HorasVendidas, d.HorasRealizadas, avancoFisico);

        return new ProjetoResumoDto(
            d.Id, d.Nome, d.Cliente,
            d.HorasVendidas, d.HorasPlanejadas, d.HorasRealizadas,
            indicadores.Saldo, indicadores.Consumo, avancoFisico,
            indicadores.Status);
    }

    // Os números de cada projeto, como vêm do banco
    private sealed class DadosProjeto
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string Cliente { get; init; } = string.Empty;
        public decimal HorasVendidas { get; init; }
        public DateTime SincronizadoEm { get; init; }
        public decimal HorasPlanejadas { get; init; }
        public decimal HorasRealizadas { get; init; }
        public bool TemEtapas { get; init; }
        public decimal PesoConcluido { get; init; }
    }
}