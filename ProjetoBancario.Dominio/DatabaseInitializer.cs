using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Microsoft.Data.Sqlite;

/*Inicialização do Banco de dados e as tabelas, caso não estejam criadas.*/

namespace ProjetoBancario
{
    public class DataBaseInitializer
    {
        public static void Initialize()
        {
            using var connection = DataBase.GetConnection();
            connection.Open();

            connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Contas (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Cpf TEXT NOT NULL,
                NumeroConta INTEGER NOT NULL UNIQUE,
                Senha TEXT NOT NULL,
                Saldo REAL NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Transacoes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NumeroConta TEXT NOT NULL,
                Tipo TEXT NOT NULL,
                Valor REAL NOT NULL,
                DataHora TEXT NOT NULL,
                FOREIGN KEY (NumeroConta) REFERENCES Contas(NumeroConta)
            ); 
            CREATE TABLE IF NOT EXISTS LoginAttempts (
            Id INTEGER PRIMARY KEY,
            LgnAtp INTEGER DEFAULT 0,
            Locked DATETIME NULL,
            FOREIGN KEY (Id) REFERENCES Contas(Id)
            );");
        }
    }
}
