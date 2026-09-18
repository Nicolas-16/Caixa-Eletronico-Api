using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System.Security.Claims;
namespace ProjetoBancario.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class TransferenciaController : ControllerBase
{
    private readonly ContaService _contaService;
    public TransferenciaController(ContaService contaService)
    {
        _contaService = contaService;
    }
    [Authorize]
    [HttpPost]
    public IActionResult Transferencia([FromBody] TransferenciaRequest request)
    {
        var MinhaConta = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (request.TransfConta == null|| MinhaConta == null)
            return BadRequest("Número da conta não pode ser vazio ou inválido.");
        if (request.Valor <= 0)
            return BadRequest("Valor não pode ser nulo.");

        try
        {
            var response = _contaService.Transferencia(MinhaConta, request.TransfConta, request.Valor);
            return Ok(response);
        }
        catch(Exception ex)
        {
            return StatusCode( 500, $"Erro interno: {ex.Message}");
        }
    }
}
