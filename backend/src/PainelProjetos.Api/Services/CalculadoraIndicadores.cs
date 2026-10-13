namespace PainelProjetos.Api.Services;

// Regras de negócio da seção 4 do DESIGN.md.
// Não acessa o banco: recebe números e devolve os indicadores.
public class CalculadoraIndicadores
{
    private readonly LimitesStatus _limites;

    public CalculadoraIndicadores(LimitesStatus limites)
    {
        _limites = limites;
    }

    public IndicadoresProjeto Calcular(decimal horasVendidas, decimal horasRealizadas, decimal? avancoFisico)
    {
        var saldo = horasVendidas - horasRealizadas;

        // Sem horas vendidas: não há como calcular consumo (seção 4.5)
        if (horasVendidas <= 0)
            return new IndicadoresProjeto(saldo, null, null, StatusProjeto.Atencao);

        var consumo = horasRealizadas / horasVendidas * 100;

        // Sem etapas não há avanço físico, então não há desvio (seção 4.5)
        decimal? desvio = avancoFisico.HasValue ? consumo - avancoFisico.Value : null;

        var status = ClassificarStatus(consumo, desvio);

        return new IndicadoresProjeto(
            saldo,
            Math.Round(consumo, 2),
            desvio.HasValue ? Math.Round(desvio.Value, 2) : null,
            status);
    }

    // "O pior status vence": testa do mais grave para o mais leve
    private StatusProjeto ClassificarStatus(decimal consumo, decimal? desvio)
    {
        if (consumo >= _limites.ConsumoCritico || desvio >= _limites.DesvioCritico)
            return StatusProjeto.Critico;

        if (consumo >= _limites.ConsumoAtencao || desvio >= _limites.DesvioAtencao)
            return StatusProjeto.Atencao;

        return StatusProjeto.Saudavel;
    }
}