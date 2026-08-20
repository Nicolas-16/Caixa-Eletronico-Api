using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario
{
    public class Transacao
    {
        public int Id { get; set; }
        public string NumeroConta { get; set; }
        public string Tipo { get; set; } // "Deposito" ou "Saque"
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }

        public Transacao(string numeroConta, string tipo, decimal valor)
        {
            NumeroConta = numeroConta;
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.Now;
        }
        public Transacao() { }
    }
}
