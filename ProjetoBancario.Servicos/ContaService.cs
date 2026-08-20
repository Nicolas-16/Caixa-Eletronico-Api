using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;

/*ContaService é onde vão ficar as regras de negócio, funções de login, criação de conta, saque, depósito etc...*/

namespace ProjetoBancario
{
    public class ContaService
    {
        public static Conta Login()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.Write("Número da conta: ");
                string numeroConta = Console.ReadLine();
                Console.Write("senha: ");
                string Senha = Console.ReadLine();
                var conta = new Conta();

                conta = ContaRepository.BuscarPorNumero(numeroConta);

                if (conta == null)
                {
                    Console.WriteLine("Conta não encontrada.");
                }
                else
                {
                    if (conta.Senha == Senha)
                    {
                        return conta;
                    }
                    else
                    {
                        Console.WriteLine("Senha incorreta, tente novamente.");
                    }
                }
            }
            Console.WriteLine("Usuário Bloqueado!");
            return null;
        }

        public static void CriarConta()
        {
            var conta = new Conta();

            Console.WriteLine("Insira os dados para o cadastro da conta.");
            while (true)
            {
                Console.Write("Digite seu nome: ");
                string nome = Console.ReadLine();
                if (nome.Length < 44)
                {
                    conta.Nome = nome;
                    break;
                }
                Console.WriteLine("O nome precisa conter menos de 44 caracteres.");
            }
            while (true)
            {
                Console.Write("Digite seu cpf (apenas números): ");
                string cpf = Console.ReadLine();
                if (cpf.Length == 11 && cpf.All(char.IsDigit))//char.IsDigit pra validar se foi digitado apenas números
                {
                    conta.Cpf = cpf;
                    break;
                }
                Console.WriteLine("seu CPF deve conter 11 digitos.");
            }
            while (true)
            {
                Console.Write("Digite sua senha: ");
                string senha = Console.ReadLine();
                if (senha.Length == 6)
                {
                    conta.Senha = senha;
                    break;
                }
                Console.WriteLine("Sua senha deve conter 6 números.");
            }
            //gerando numero da conta de forma aleatória.
            var random = new Random();
            string numeroConta;
            Conta contaExistente;

            do
            {
                numeroConta = random.Next(0, 1000000).ToString("D6");
                contaExistente = ContaRepository.BuscarPorNumero(numeroConta);

            } while (contaExistente != null); //caso já exista gerar novamente!

            conta.NumeroConta = numeroConta;

            ContaRepository.CadastrarConta(conta);

            Console.WriteLine();
            Console.WriteLine($"Conta cadastrada com sucesso!\nNome: {conta.Nome}\nCPF: {conta.Cpf}\nConta: {conta.NumeroConta}\nSenha (não mostre a ninguém): {conta.Senha}");
            Console.WriteLine();
            return;
        }
        public static Conta Transferir(Conta conta)
        {
            var contaTransferencia = new Conta();//Conta que vai receber o valor

            do
            {
                Console.Write("Qual o valor da transferência: ");
                decimal valor = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                if (valor > 0)
                {
                    if (valor <= conta.Saldo)
                    {
                        Console.Write("Numero da conta para transferir: ");
                        contaTransferencia.NumeroConta = Console.ReadLine();
                        ContaRepository.BuscarPorNumero(contaTransferencia.NumeroConta);

                        conta.Saldo -= valor;
                        ContaRepository.SetSaldo(conta);//Saida do valor da sua conta logada
                        string tipo = "Saida";
                        var transacaoSaida = new Transacao(conta.NumeroConta, tipo, valor);
                        ContaRepository.RegTrans(transacaoSaida);

                        contaTransferencia.Saldo += valor;
                        ContaRepository.SetSaldo(contaTransferencia);//Entrada do valor na conta para deposito
                        tipo = "Entrada";
                        var transacaoEntrada = new Transacao(contaTransferencia.NumeroConta, tipo, valor);
                        ContaRepository.RegTrans(transacaoEntrada);


                        Console.WriteLine("Transfêrencia realizado!");

                        return conta;
                    }
                    Console.WriteLine($"O valor não pode ser maior que o seu saldo atual: R${conta.Saldo}");
                }
                else
                {
                    Console.WriteLine("O valor precisa ser maior que zero.");
                }

                while (true)
                {
                    string opcao;
                    Console.WriteLine("1- Digitar novo valor.\n2- Sair.");
                    opcao = Console.ReadLine();
                    if (opcao == "1")
                    {
                        break;//volta para o valor do deposito.
                    }
                    if (opcao == "2")
                    {
                        return conta;//volta para a tela inicial.
                    }
                    Console.WriteLine("Opção inválida!");
                }

            } while (true);

        }
        public static Conta Depositar(Conta conta)
        {
            while (true)
            {
                Console.Write("Qual o valor do deposito: ");
                decimal valor = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                if (valor > 0)
                {
                    conta.Saldo += valor;
                    ContaRepository.SetSaldo(conta);
                    string tipo = "Entrada";
                    var transacao = new Transacao(conta.NumeroConta, tipo, valor);
                    ContaRepository.RegTrans(transacao);
                    Console.WriteLine("Deposito Realizado!");
                    return conta;
                }
                Console.WriteLine("Valor precisa ser maior que zero.");
                while (true)
                {
                    Console.WriteLine("1- Digitar novo valor.\n2- Sair.");
                    Console.Write("Digite uma opção: ");
                    string opcao = Console.ReadLine();
                    switch (opcao)
                    {
                        case "1":
                            break;
                        case "2":
                            return conta;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            }
        }

        public static Conta Sacar(Conta conta)
        {
            string opcao = "0";
            while (true)
            {
                Console.Write("Digite o valor que deseja sacar: ");
                decimal valor = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                if (valor <= conta.Saldo)
                {
                    conta.Saldo -= valor;
                    ContaRepository.SetSaldo(conta);
                    Console.WriteLine("Saque realizado!");

                    string tipo = "Saida";
                    var transacao = new Transacao(conta.NumeroConta, tipo, valor);
                    ContaRepository.RegTrans(transacao);

                    conta = ContaRepository.GetSaldo(conta);
                    Console.WriteLine($"Saldo atualizado: {conta.Saldo}");
                    return conta;
                }

                while (true)
                {
                    Console.WriteLine("Saldo insuficiente.\n 1- Informar novo valor.\n2- Sair.");
                    opcao = Console.ReadLine();
                    switch (opcao)
                    {
                        case "1":
                            break;
                        case "2":
                            return conta;
                    }
                    Console.Write("Opção inválida.");
                }
            }
        }

        public static Conta Extrato(Conta conta)
        {

            var Extrato = ContaRepository.BuscarExtrato(conta.NumeroConta);
            Console.WriteLine("Data e hora      |Tipo    | Valor   ");
            Console.WriteLine();
            foreach (var i in Extrato)
            {
                Console.WriteLine($"{i.DataHora:dd/MM/yyyy HH:mm} |{i.Tipo,-7} |R$ {i.Valor}");
            }
            Console.WriteLine($"Saldo atual: {conta.Saldo}");
            return conta;
        }
    }
}
