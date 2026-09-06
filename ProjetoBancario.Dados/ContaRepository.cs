using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

/*classe ContaRepository é apenas para fazer acessos e alterações no banco*/
namespace ProjetoBancario
{
    public class ContaRepository
    {
        public static void Inserir(Conta conta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"
        INSERT INTO Contas
        (Nome, Cpf, NumeroConta, Senha, Saldo)
        VALUES
        (@Nome, @Cpf, @NumeroConta, @Senha, @Saldo);";

            connection.Execute(sql, conta);
        }

        public static bool ValidarNumero(string numeroConta)//verifica se o numero da conta já existe no banco.
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT COUNT(*) FROM Contas WHERE NumeroConta == @NumeroConta;";
            var count = connection.ExecuteScalar<int>(sql, new {NumeroConta = numeroConta});
            return count == 0;

        }
        public static int ProximoNumero()//retorna o próximo número válido de conta.
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT MAX(NumeroConta) FROM Contas;";
            var MaxNumero = connection.ExecuteScalar<int?>(sql);

            if (MaxNumero == null || MaxNumero == 0)
            {
                // quando o número for nulo, a primeira conta vai ser 100000.
                return 100000;
                
            }
            return MaxNumero.Value+1;
        }
        public static Conta ValidarConta(string nome, string cpf)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT * FROM Contas WHERE Nome == @Nome OR Cpf == @Cpf";
            return connection.QueryFirstOrDefault<Conta>(sql, new { @Nome = nome, @Cpf = cpf});
        }
        public static void CadastrarConta(Conta conta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"INSERT INTO Contas (Nome, Cpf, NumeroConta, Senha, Saldo) VALUES (@Nome, @Cpf, @NumeroConta, @Senha, @Saldo)";
            connection.Execute(sql, conta);

        }
        public static void SetSaldo(Conta conta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"UPDATE Contas SET Saldo = @Saldo WHERE NumeroConta = @NumeroConta;";
            connection.Execute(sql, conta);


        }
        public static void RegTrans(Transacao transacao)//ainda não fuciona, está em implementação.
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"INSERT INTO Transacoes (NumeroConta, Tipo, Valor, DataHora) VALUES (@NumeroConta, @Tipo, @Valor, @DataHora );";
            connection.Execute(sql, transacao);
        }

        public static Conta GetSaldo(Conta conta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT * FROM Contas WHERE NumeroConta == @NumeroConta;";
            return connection.QueryFirstOrDefault<Conta>(sql, conta);
        }
        public static List<Transacao> BuscarExtrato(string numeroConta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT Id, NumeroConta, Tipo, Valor, DataHora FROM Transacoes WHERE NumeroConta = @NumeroConta AND DataHora >= datetime('now', '-30 days') ORDER BY DataHora DESC;";
            return connection.Query<Transacao>(sql, new { NumeroConta = numeroConta }).ToList();
        }
    }
}
