using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System;

namespace ProjetoBancario.Api;

[ApiController]
[Route("api/[Controller]")]
public class LoginController : ControllerBase
{
    [HttpPost("login")]

    public IActionResult FazerLogin([FromBody] FazerLoginRequest request)
    {
        if (request == null)
            return BadRequest("Dados da requisição são nulos");

        if (string.IsNullOrEmpty(request.NumeroConta) ||
                string.IsNullOrEmpty(request.SenhaHash))
        {
            return BadRequest("Nome, CPF e senha são obrigatórios");
        }

        try
        {
            var response = ContaService.Login(request.NumeroConta, request.SenhaHash);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}