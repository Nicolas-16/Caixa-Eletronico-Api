using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System;

namespace ProjetoBancario.Api;

[ApiController]
[Route("api/[Controller]")]
public class LoginController : ControllerBase
{
    private readonly ContaService _contaService;
    public LoginController(ContaService contaService)
    {
        _contaService = contaService;
    }

    [HttpPost]

    public IActionResult FazerLogin([FromBody] FazerLoginRequest request)
    {
        if (request == null)
            return BadRequest("Dados da requisição são nulos");

        if (string.IsNullOrEmpty(request.NumeroConta) ||
                string.IsNullOrEmpty(request.Senha))
        {
            return BadRequest("Nome, CPF e senha são obrigatórios");
        }

        try
        {
            LoginResponse response = _contaService.Login(request.NumeroConta, request.Senha);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}