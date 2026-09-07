// DTOs/CriarContaRequest.cs
namespace ProjetoBancario.DTOs;
    public class CriarContaRequest
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }