using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

/*A classe Conta fica apenas como modelo de atributos de vão ser instanciados pelas outras classes.*/
namespace ProjetoBancario
{
    public class Conta
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public int NumeroConta { get; set; }
        public string Senha { get; set; }
        public decimal Saldo { get; set; }

    }
}
