using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Identity.Core;
using Microsoft.IdentityModel.Tokens;
using ProjetoBancario.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/*ContaService é onde vão ficar as regras de negócio, funções de login, criação de conta, saque, depósito etc...*/

namespace ProjetoBancario
{
    public class ContaService
    {
        private readonly IConfiguration _configuration;
        private readonly ContaRepository _contaRepository;
        public ContaService(ContaRepository contaRepository, IConfiguration configuration)
        {
            _contaRepository = contaRepository;
            _configuration = configuration;
        }

        public LoginResponse Login(string numeroConta, string senha)
        {

            if (!int.TryParse(numeroConta, out var numeroContaInteiro))
                throw new ArgumentException("O número da conta deve conter apenas dígitos.");

            if (!ContaRepository.ValidarNumero(numeroContaInteiro))
                throw new ArgumentException("Conta não encontrada.");
            
            var senhaBanco = ContaRepository.ValidarSenha(numeroContaInteiro);

            var passwordHasher = new PasswordHasher<Conta>();
            var validacao = passwordHasher.VerifyHashedPassword(null!, senhaBanco, senha);
            if (validacao == PasswordVerificationResult.Failed)
                throw new ArgumentException("Número da conta ou senha inválidos.");

            var token = GerarToken(numeroConta);

            return new LoginResponse { 
                Sucesso = true, 
                Mensagem = "Login bem-sucedido", 
                NumeroConta = numeroConta, 
                Token = token 
            };
        }
        public string GerarToken(string numeroConta)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]) );
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var claims = new[] {new Claim(ClaimTypes.NameIdentifier, numeroConta)};

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public static CriarContaResponse CriarConta(string nome, string cpf, string senha)
        {
            if (cpf.Length != 11)
                throw new ArgumentException("CPF precisa ter 11 dígitos.");

            if (senha.Length != 6)
                throw new ArgumentException("A senha precisa ter 6 dígitos.");

            if (ContaRepository.ValidarConta(nome, cpf) != null)
                throw new ArgumentException("CPF já cadastrado");

            var passwordHasher = new PasswordHasher<Conta>();

            var conta = new Conta
            {
                Nome = nome,
                Cpf = cpf,
                Senha = passwordHasher.HashPassword(null!, senha),
                Saldo = 0,
                NumeroConta = ContaRepository.ProximoNumero()
            };

            ContaRepository.Inserir(conta);

            return new CriarContaResponse { 
                Sucesso = true, 
                Mensagem = "Conta criada com sucesso", 
                numeroConta = conta.NumeroConta.ToString() 
            };
        }
        public GenericResponse Deposito(decimal valor, string numeroConta)
        {
            decimal saldo = _contaRepository.GetSaldo(numeroConta);
            saldo += valor;

            _contaRepository.SetSaldo(saldo, numeroConta);
            return new GenericResponse { Sucesso = true, Mensagem = "Depósito realizado!" };
            
        }

        public GenericResponse Transferencia(string contaOrigem, string contaDestino, decimal valor)
        {
            _contaRepository.Transferir(contaOrigem, contaDestino, valor);
            return new GenericResponse { Sucesso = true, Mensagem = "Transferência realizada!" };
        }

        public GenericResponse Saque(string MinhaConta, decimal Valor)
        {
            decimal saldo = _contaRepository.GetSaldo(MinhaConta);
            saldo -= Valor;
            _contaRepository.SetSaldo(saldo, MinhaConta);
            return new GenericResponse {Sucesso= true, Mensagem="Saque realizado com sucesso!" };
        }
            
        public ExtratoResponse Extrato(string conta)
        {
            var transacoes = _contaRepository.BuscarExtrato(conta);
            var saldo = _contaRepository.GetSaldo(conta);

            return new ExtratoResponse
            {
                Saldo = saldo,
                Transacoes = transacoes
            };
        }
    }
}
