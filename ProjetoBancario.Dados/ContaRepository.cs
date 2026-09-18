using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Net;
using System.Text;
using System.Transactions;

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

        public static bool ValidarNumero(int numeroConta)//verifica se o numero da conta já existe no banco.
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT COUNT(*) FROM Contas WHERE NumeroConta == @NumeroConta;";
            var count = connection.ExecuteScalar<int>(sql, new {NumeroConta = numeroConta});
            return count > 0;

        }
        public static string ValidarSenha(int numeroConta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT Senha FROM Contas WHERE NumeroConta = @NumeroConta";
            return connection.QueryFirstOrDefault<string>(sql, new { NumeroConta = numeroConta});

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
        public void SetSaldo(decimal valorDecimal, string numeroConta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            var novoSaldoEmCentavos = decimal.ToInt64(valorDecimal * 100m);
            using var transaction = connection.BeginTransaction();

            try
            {
                const string saldoAtualSql = "SELECT Saldo FROM Contas WHERE NumeroConta = @NumeroConta;";
                var saldoAnteriorEmCentavos = connection.ExecuteScalar<long>(
                    saldoAtualSql,
                    new { NumeroConta = numeroConta },
                    transaction
                );

                const string atualizarSaldoSql = "UPDATE Contas SET Saldo = @Saldo WHERE NumeroConta = @NumeroConta;";
                var contasAtualizadas = connection.Execute(
                    atualizarSaldoSql,
                    new { Saldo = novoSaldoEmCentavos, NumeroConta = numeroConta },
                    transaction
                );

                if (contasAtualizadas != 1)
                    throw new ArgumentException("Conta não encontrada.");

                var diferencaEmCentavos = novoSaldoEmCentavos - saldoAnteriorEmCentavos;

                if (diferencaEmCentavos > 0)
                    RegTrans(connection, transaction, numeroConta, "Entrada", diferencaEmCentavos / 100m);
                else if (diferencaEmCentavos < 0)
                    RegTrans(connection, transaction, numeroConta, "Saída", -diferencaEmCentavos / 100m);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static void RegTrans(
            IDbConnection connection,
            IDbTransaction transaction,
            string numeroConta,
            string tipo,
            decimal valor)
        {
            const string sql = """
                INSERT INTO Transacoes (NumeroConta, Tipo, Valor, DataHora)
                VALUES (@NumeroConta, @Tipo, @Valor, @DataHora);
                """;

            connection.Execute(sql, new
            {
                NumeroConta = numeroConta,
                Tipo = tipo,
                Valor = valor,
                DataHora = DateTime.UtcNow
            }, transaction);
        }

        public decimal GetSaldo(string numeroConta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT Saldo FROM Contas WHERE NumeroConta == @NumeroConta;";
            long valor = connection.ExecuteScalar<long>(sql, new { NumeroConta= numeroConta});
            return valor / 100m;
        }
        public void Transferir(string contaOrigem, string contaDestino, decimal valorDecimal)
        {
            if (contaOrigem == contaDestino)
                throw new ArgumentException("A conta de destino deve ser diferente da conta de origem.");

            using var connection = DataBase.GetConnection();
            connection.Open();

            long valor = decimal.ToInt64(valorDecimal * 100m);//convertendo pra guardar como inteiro no banco.

            //realizando a atualização das contas juntas pra garantir o princípio da atomicidade.
            using var transaction = connection.BeginTransaction();
            try 
            {
                var origemAtualizada = connection.Execute(
                    """
                    UPDATE Contas
                    SET Saldo = Saldo - @Valor
                    WHERE NumeroConta = @ContaOrigem AND Saldo >= @Valor;
                    """,
                    new { Valor = valor, ContaOrigem = contaOrigem },
                    transaction
                );

                if (origemAtualizada != 1)
                    throw new ArgumentException("Conta de origem não encontrada ou saldo insuficiente.");

                var destinoAtualizado = connection.Execute(
                    """
                    UPDATE Contas
                    SET Saldo = Saldo + @Valor
                    WHERE NumeroConta = @ContaDestino;
                    """,
                    new { Valor = valor, ContaDestino = contaDestino },
                    transaction
                );

                if (destinoAtualizado != 1)
                    throw new ArgumentException("Conta de destino não encontrada.");

                RegTrans(connection, transaction, contaOrigem, "Saída", valorDecimal);
                RegTrans(connection, transaction, contaDestino, "Entrada", valorDecimal);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public List<Transacao> BuscarExtrato(string conta)
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            string sql = @"SELECT Id, NumeroConta, Tipo, Valor, DataHora FROM Transacoes WHERE NumeroConta = @Conta AND DataHora >= datetime('now', '-30 days') ORDER BY DataHora DESC;";
            return connection.Query<Transacao>(sql, new { Conta = conta }).ToList();
        }
    }
}
