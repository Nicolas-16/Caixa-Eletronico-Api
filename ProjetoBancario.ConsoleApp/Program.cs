using System;
using System.ComponentModel;
using System.Text;
using System.Linq;

/*A classe Program vai conter os menus de acesso do usuário*/

namespace ProjetoBancario
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            DataBaseInitializer.Initialize();

            string opcao = "0";


            while (opcao != "3")
            {
                //tela de login incial
                Console.WriteLine();
                Console.WriteLine("Bem vindo ao SeuBanco \n1- Fazer Login.\n2- Criar Conta.\n3- Sair.");
                Console.Write("Digite uma opção: ");
                opcao = Console.ReadLine();
                Console.WriteLine();

                switch (opcao)
                {
                    case "1":

                        var ContaValida = ContaService.Login();
                        if (ContaValida != null)
                        {
                            TelaInicial(ContaValida);
                        }
                        break;
                    case "2":
                        Console.Write("Digite seu nome: ");
                        string nome = Console.ReadLine();
                        Console.Write("Digite seu cpf: ");
                        string cpf = Console.ReadLine();
                        Console.Write("Digite sua senha: ");
                        string senha = Console.ReadLine();

                        ContaService.CriarConta(nome, cpf, senha);
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Não entendi, por favor digite novamente.");
                        break;
                }
            }
            Console.WriteLine("Finalizando programa...");
        }
        public static void TelaInicial(Conta conta)//tela inicial apos o login
        {
            string opcao = "0";
            while (opcao != "5")
            {
                Console.WriteLine();
                Console.WriteLine("Qual operação deseja realizar? \n1- Transferência\n2- Deposito.\n3- Saque.\n4- Tirar Extrato.\n5- Sair.");
                Console.Write("Digite uma opção: ");
                opcao = Console.ReadLine();
                switch (opcao)
                {
                    case "1":
                        //ContaService.Transferir(conta);
                        break;
                    case "2":
                        //ContaService.Depositar(conta);
                        break;
                    case "3":
                        //ContaService.Sacar(conta);
                        break;
                    case "4":
                        //ContaService.Extrato(conta);
                        break;
                    case "5":
                        return;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

            }
            return;
        }

    }

}
