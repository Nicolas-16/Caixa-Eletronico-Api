using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*classe ContaRepository é apenas para fazer acessos e alterações ao banco*/
namespace ProjetoBancario
{
    public class ContaRepository
    {
        public void Inserir(Conta conta)
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

        public static Conta BuscarPorNumero(string numeroConta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT * FROM Contas WHERE NumeroConta == @NumeroConta;";
            return connection.QueryFirstOrDefault<Conta>(sql, new { NumeroConta = numeroConta });
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
