namespace PainelProjetos.Api.Services;

public record IndicadoresProjeto(
    decimal Saldo,
    decimal? Consumo,
    decimal? Desvio,
    StatusProjeto Status);