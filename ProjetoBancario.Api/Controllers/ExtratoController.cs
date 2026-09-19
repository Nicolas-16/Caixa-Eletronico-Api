using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System.Security.Claims;

namespace ProjetoBancario.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ExtratoController : ControllerBase
{
    private readonly ContaService _contaService;

    public ExtratoController(ContaService contaService)
    {
        _contaService = contaService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<ExtratoResponse> ObterExtrato()
    {
        var numeroConta = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (numeroConta is null)
            return Unauthorized();

        var extrato = _contaService.Extrato(numeroConta);

        return Ok(extrato);
    }

}
