using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System.Security.Claims;
namespace ProjetoBancario.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class DepositoController : ControllerBase
{
    private readonly ContaService _contaService;
    public DepositoController (ContaService contaService)
    {
        _contaService = contaService;
    }
    [Authorize]
    [HttpPost("Deposito")]
    public IActionResult Deposito([FromBody] DepositoRequest request)
    {
        var numeroConta = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (numeroConta == null)
            return BadRequest("O número da conta não pode ser nulo.");

        if (request.Valor == null || request.Valor <0)
            return BadRequest("O valor do depósito não pode ser nulo ou negativo.");

        try
        {
            var resposta = _contaService.Deposito(request.Valor, numeroConta);
            return Ok(resposta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}
