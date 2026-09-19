using System;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace ProjetoBancario.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class SaqueController : ControllerBase
{
    private readonly ContaService _contaService;
    public SaqueController(ContaService contaService)
    {
        _contaService = contaService;
    }

    [Authorize]
    [HttpPost]
    public IActionResult Saque([FromBody] SaqueRequest request)
    {
        var MinhaConta = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (MinhaConta == null)
            return BadRequest("número da conta não informado");
        if (request.Valor == 0)
            return BadRequest("O valor do saque não pode ser zero.");

        try
        {
            var response = _contaService.Saque(MinhaConta, request.Valor);
            return Ok(response);
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"erro interno: {ex}");
        }
    }
}
