using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ProjetoBancario.DTOs;

/*ContaService é onde vão ficar as regras de negócio, funções de login, criação de conta, saque, depósito etc...*/

namespace ProjetoBancario
{
    public class ContaService
    {
        public static GenericResponse Login(string numeroConta, string senha)
        {
            if (numeroConta.Length != 6 || senha.Length != 6)
                throw new ArgumentException("A senha e/ou número da conta deve conter 6 dígitos.");
            try
            {
                var validacao = ContaRepository.ValidarNumero(int.Parse(numeroConta));
                if (validacao == false)
                    throw new ArgumentException("Conta não encontrada.");
            }
            catch
            {
                throw new ArgumentException("O número da conta não foi digitado adequadamente.");
            }
            
            var senhaBanco = ContaRepository.ValidarSenha(numeroConta);

            if (senha != senhaBanco)
                throw new ArgumentException("As senhas não coincidem.");

            return new GenericResponse { Sucesso = true, Mensagem = "Login efetuado com sucesso." };
        }

        public static GenericResponse CriarConta(string nome, string cpf, string senha)
        {
            if (cpf.Length != 11)
                throw new ArgumentException("CPF precisa ter 11 dígitos.");

            if (senha.Length != 6)
                throw new ArgumentException("A senha precisa ter 6 dígitos.");

            if (ContaRepository.ValidarConta(nome, cpf) != null)
                throw new ArgumentException("CPF já cadastrado");

            var conta = new Conta
            {
                Nome = nome,
                Cpf = cpf,
                Senha = senha,
                Saldo = 0,
                NumeroConta = ContaRepository.ProximoNumero()
            };

            ContaRepository.Inserir(conta);
            return new GenericResponse { Sucesso = true, Mensagem = "Conta criada com sucesso" };
        }
        public static GenericResponse Deposito(double valor)
        {

            return new GenericResponse { Sucesso = true, Mensagem = "Deposito realizado."};
        }
            /*
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
            return conta; 
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
        }*/
    }
}
