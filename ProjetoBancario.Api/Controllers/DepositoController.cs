using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
namespace ProjetoBancario.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class DepositoController : ControllerBase
{
    [HttpPost("Deposito")]
    public IActionResult Deposito([FromBody] DepositoRequest request)
    {
        if (request.Valor == null || request.Valor <0)
            return BadRequest("O valor do depósito não pode ser nulo ou negativo.");

        try
        {
            var resposta = ContaService.Deposito(request.Valor);
            return Ok(resposta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}
