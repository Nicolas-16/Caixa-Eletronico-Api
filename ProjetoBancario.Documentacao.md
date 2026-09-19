# Documentação do Projeto Bancário

## 1. Modelo de Domínio

### Conta.cs
- Representa uma conta bancária
- Propriedades:
  - Id: Identificador único
  - Nome: Nome do titular
  - Cpf: CPF do titular
  - NumeroConta: Número da conta
  - Senha: Senha da conta
  - Saldo: Saldo disponível

### Transacao.cs
- Representa uma transação financeira
- Propriedades:
  - Id: Identificador único
  - NumeroConta: Número da conta associada
  - Tipo: Tipo de transação (Depósito/Saque)
  - Valor: Valor da transação
  - DataHora: Data e hora da transação

## 2. Camada de Acesso a Dados

### DataBase.cs
- Gerenciador de conexão com o banco de dados SQLite
- Fornece método GetConnection() para obter a conexão

### DatabaseInitializer.cs
- Inicializa o banco de dados criando as tabelas necessárias
- Cria as tabelas Contas, Transacoes e tentativas de login, se não existirem

### ContaRepository.cs
- Responsável por operações de acesso a dados de contas
- Métodos:
  - Inserir: Insere uma nova conta no banco
  - CadastrarConta: Cadastra uma nova conta
  - SetSaldo: Atualiza o saldo de uma conta
  - RegTrans: Registra uma transação
  - GetSaldo: Recupera o saldo de uma conta
  - BuscarExtrato: Busca o extrato de uma conta
  - ValidarSenha: Retorna a senha da conta
  - ProximoNumero: retorna o próximo número de conta válido
  - ValidarNumero: verifica se o numero da conta já existe no banco
  - Transferir: realiza a transferencia entre contas

## 3. Camada de Serviço

### ContaService.cs
- Contém a lógica de negócios para operações com contas
- Métodos:
  - Login: Autentica um usuário
  - CriarConta: Cria uma nova conta com validações
  - Transferir: Realiza transferência entre contas
  - Depositar: Deposita valor em uma conta
  - Sacar: Sacar valor de uma conta
  - Extrato: Exibe o extrato de uma conta

## 4. Camada de API

### DTOs
- CriarContaRequest: DTO para requisição de criação de conta
- CriarContaResponse: DTO retorna número da conta cadastrada
- DepositoRequest:DTO recebe valor para deposito
- ExtratoRequest: DTO recebe numero da conta pelo JWT
- ExtratoResponse: DTO devolve saldo e uma lista de transações
- FazerLoginRequest: DTO para login recebe número de conta e senha
- LoginResponse: DTO retorna o token de acesso
- GenericResponse: DTO para respostas genericas
- SaqueRequest: DTO recebe valor
- TransferenciaRequest: DTO recebe valor, conta de destino e a conta origem pelo JWT

### UsuariosController.cs
- Controlador da API para endpoints relacionados a usuários
- Gerencia requisições HTTP para operações de conta

### LoginController
- Controlador de API para endpoint recebendo o usuário e senha do DTO, validando, e encaminhando para o método login do ContaService.

### DepositoController
- Controlador de API para endpoint de deposit, recebe o DTO de deposito, valida, e encaminhar para o método Deposito do ContaService.

### ExtratoController
 - Controlador de API para endpoint de extrato, recebe DTO ExtratoRequest, bem como o token com a conta origem, valida e encaminha para o método Extrato do ContaService.

### SaqueController
- Controlador de API para endopoint de extrato, recebe o DTO SaqueRequest, bem como o token com a conta origem, valida e encaminha para o método Saque do ContaService.

### TransferenciaController
- Controlador de API para endopoint de transferencia, recebe o DTO TransferenciaRequest, bem como o token com a conta origem, valida e encaminha para o método Transferencia do ContaService.


## 5. Arquitetura
- O projeto segue um padrão de arquitetura de camadas:
  1. Camada de Domínio (Modelo)
  2. Camada de Acesso a Dados
  3. Camada de Serviço (Business Logic)
  4. Camada de API (Controllers)
- Utiliza SQLite como banco de dados
- Utiliza Dapper como micro ORM
- A camada de serviço contém a lógica de regras de negócio
- A camada de API expõe as funcionalidades via endpoints HTTP

## 6. Funcionalidades Principais
- Autenticação de usuário
- Criação de conta
- Depósito
- Saque
- Transferência
- Extrato
- Gerenciamento de transações

## 7. Considerações
- O projeto foi convertido de um console app para uma API
- A camada de serviço contém a lógica principal do sistema
- O banco de dados é gerenciado via SQLite com Dapper.