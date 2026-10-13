using PainelProjetos.Api.Services;

namespace PainelProjetos.Tests;

public class CalculadoraIndicadoresTests
{
    private readonly CalculadoraIndicadores _calculadora = new(new LimitesStatus());

    // Os 4 projetos da tabela 4.4 do DESIGN.md
    [Theory]
    [InlineData(200, 60, 30, StatusProjeto.Saudavel)]
    [InlineData(200, 165, 80, StatusProjeto.Atencao)]
    [InlineData(200, 210, 90, StatusProjeto.Critico)]
    [InlineData(200, 170, 60, StatusProjeto.Critico)]
    public void Classifica_os_projetos_do_documento(double vendido, double realizado, double avanco, StatusProjeto esperado)
    {
        var resultado = _calculadora.Calcular((decimal)vendido, (decimal)realizado, (decimal)avanco);

        Assert.Equal(esperado, resultado.Status);
    }

    [Fact]
    public void Calcula_saldo_consumo_e_desvio()
    {
        var resultado = _calculadora.Calcular(200, 170, 60);

        Assert.Equal(30m, resultado.Saldo);
        Assert.Equal(85m, resultado.Consumo);
        Assert.Equal(25m, resultado.Desvio);
    }

    // Limites exatos, com 200h vendidas: atingir o limite já dispara o status
    [Theory]
    [InlineData(158, 79, StatusProjeto.Saudavel)]  // consumo 79%, desvio 0
    [InlineData(160, 80, StatusProjeto.Atencao)]   // consumo 80%, desvio 0
    [InlineData(200, 100, StatusProjeto.Critico)]  // consumo 100%, desvio 0
    [InlineData(100, 40, StatusProjeto.Atencao)]   // consumo 50%, desvio 10
    [InlineData(100, 30, StatusProjeto.Critico)]   // consumo 50%, desvio 20
    public void Respeita_os_limites_exatos(double realizado, double avanco, StatusProjeto esperado)
    {
        var resultado = _calculadora.Calcular(200, (decimal)realizado, (decimal)avanco);

        Assert.Equal(esperado, resultado.Status);
    }

    [Fact]
    public void Projeto_sem_horas_vendidas_fica_em_atencao()
    {
        var resultado = _calculadora.Calcular(0, 10, 50);

        Assert.Equal(StatusProjeto.Atencao, resultado.Status);
        Assert.Null(resultado.Consumo);
    }

    [Fact]
    public void Projeto_sem_etapas_ignora_a_regra_de_desvio()
    {
        // Consumo de 50% sem avanço físico: se o avanço fosse tratado como 0,
        // o desvio seria 50 e o projeto pareceria crítico
        var resultado = _calculadora.Calcular(200, 100, null);

        Assert.Equal(StatusProjeto.Saudavel, resultado.Status);
        Assert.Null(resultado.Desvio);
    }

    [Fact]
    public void Desvio_negativo_e_saudavel()
    {
        // Entregou 90% gastando 30%: a entrega está à frente do gasto
        var resultado = _calculadora.Calcular(200, 60, 90);

        Assert.Equal(StatusProjeto.Saudavel, resultado.Status);
        Assert.Equal(-60m, resultado.Desvio);
    }

    [Fact]
    public void Usa_os_limites_configurados()
    {
        var calculadora = new CalculadoraIndicadores(new LimitesStatus { ConsumoAtencao = 50 });

        // Consumo de 60%: saudável no padrão, atenção com o limite em 50%
        var resultado = calculadora.Calcular(200, 120, 60);

        Assert.Equal(StatusProjeto.Atencao, resultado.Status);
    }
}