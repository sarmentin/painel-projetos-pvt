using Microsoft.AspNetCore.Mvc;
using PainelProjetos.Api.Dtos;
using PainelProjetos.Api.Models;
using PainelProjetos.Api.Services;

namespace PainelProjetos.Api.Controllers;

[ApiController]
[Route("api/projetos")]
public class ProjetosController : ControllerBase
{
    private readonly ProjetoService _service;

    public ProjetosController(ProjetoService service)
    {
        _service = service;
    }

    // GET /api/projetos?status=Critico&clienteId=1&situacao=Ativo
    [HttpGet]
    public async Task<ActionResult<PainelProjetosDto>> Listar(
        [FromQuery] StatusProjeto? status,
        [FromQuery] int? clienteId,
        [FromQuery] SituacaoProjeto? situacao)
    {
        var resultado = await _service.ListarAsync(status, clienteId, situacao);
        return Ok(resultado);
    }
}