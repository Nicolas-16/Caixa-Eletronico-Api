// Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using ProjetoBancario.DTOs;
using System;

namespace ProjetoBancario.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {

        [HttpPost("criar-conta")]
        public IActionResult CriarConta([FromBody] CriarContaRequest request)
        {
            if (request == null)
                return BadRequest("Dados da requisição são nulos");

            // Validação básica (exemplo)
            if (string.IsNullOrEmpty(request.Nome) ||
                string.IsNullOrEmpty(request.Cpf) ||
                string.IsNullOrEmpty(request.Senha))
            {
                return BadRequest("Nome, CPF e senha são obrigatórios");
            }

            try
            {
                var resposta = ContaService.CriarConta(request.Nome, request.Cpf, request.Senha);
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}