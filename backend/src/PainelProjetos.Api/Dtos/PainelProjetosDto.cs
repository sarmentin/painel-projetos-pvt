using PainelProjetos.Api.Services;

namespace PainelProjetos.Api.Dtos;

// Formato da resposta do GET /api/projetos (seção 3.3 do DESIGN.md)
public record PainelProjetosDto(
    DateTime? SincronizadoEm,
    IReadOnlyList<ProjetoResumoDto> Projetos);

public record ProjetoResumoDto(
    int Id,
    string Nome,
    string Cliente,
    decimal HorasVendidas,
    decimal HorasPlanejadas,
    decimal HorasRealizadas,
    decimal Saldo,
    decimal? Consumo,
    decimal? AvancoFisico,
    StatusProjeto Status);