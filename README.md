# Patas Limpas — Agendamento de banho e tosa

Aplicativo de estudo em **C# e Windows Forms** para cadastrar clientes e pets, consultar horários, agendar banho/tosa e cancelar reservas. Os dados são persistidos em **MySQL ou MariaDB**, usando **MySqlConnector**.

## Demonstração web

A versão original em C# permanece neste repositório. A pasta [`docs/`](docs/) contém uma adaptação independente em HTML, CSS e JavaScript para experimentar o fluxo no navegador, inclusive no celular.

**[Abrir demonstração](https://iedahirari.github.io/P2-Topicos-Banho-e-Tosa/)**

Use apenas dados fictícios: cadastros e agendamentos ficam no `localStorage` deste navegador. Não há servidor, autenticação ou conexão com o banco da versão Windows. Os dados não são compartilhados entre dispositivos e podem ser apagados ao limpar os dados do navegador. A demonstração não deve ser usada para reservas reais; a gravação local também não oferece garantia de exclusão mútua entre abas simultâneas.

1. Abra **Cliente e pets** e cadastre um cliente com um a quatro pets.
2. Em **Agendar cuidado**, consulte seu telefone e uma data.
3. Escolha pet, serviço e horário e confirme. O cancelamento fica na lista de agendamentos do cliente.
4. Para editar, consulte o telefone e clique em **Editar cadastro**. O telefone e os pets existentes são preservados para manter os vínculos.

Para executar localmente, a partir da raiz: `python -m http.server 8080 --directory docs`, e abra `http://localhost:8080`. Teste das regras web: `node --test tests/web.test.mjs` (Node.js 24).

A publicação é feita por [`.github/workflows/pages.yml`](.github/workflows/pages.yml). Em **Settings → Pages → Source**, selecione **GitHub Actions**. O workflow testa as regras e publica somente `docs/` quando essa pasta muda na branch `main`.

## Funcionalidades

- Cadastro de cliente com telefone, endereço e até quatro pets.
- Atualização de cliente e pets existentes, mantendo seus identificadores e agendamentos.
- Consulta dos pets pelo telefone do cliente.
- Consulta dos horários de uma data.
- Agendamento de banho, tosa ou ambos.
- Cancelamento com confirmação, restrito ao cliente consultado.
- Validação de campos, datas, horários e disponibilidade.
- Gravação de cliente e pets em uma transação: uma falha não deixa cadastro parcial.
- Restrição no banco que impede duas reservas para o mesmo horário, inclusive em tentativas simultâneas.

## Requisitos

- Windows.
- **SDK .NET 10**, com suporte ao Windows Forms, para compilar.
- MySQL 8+ ou MariaDB 10.4+ com InnoDB.
- Opcional: Visual Studio compatível com .NET 10 e a carga de trabalho **Desenvolvimento para desktop com .NET**.

O .NET 6 e os quatro drivers/pacotes conflitantes do projeto original foram substituídos por .NET 10 e um único driver MySQL.

## Preparar o banco

O repositório não inclui dados reais nem senha de banco.

1. Crie um banco vazio e um usuário para o aplicativo, usando uma conta administrativa do MySQL/MariaDB. Substitua a senha de exemplo por uma senha sua:

```sql
CREATE DATABASE agendamento CHARACTER SET utf8mb4;
CREATE USER 'banhotosa'@'localhost' IDENTIFIED BY 'ESCOLHA_UMA_SENHA';
GRANT SELECT, INSERT, UPDATE, DELETE ON agendamento.* TO 'banhotosa'@'localhost';
```

2. Importe [database/schema.sql](database/schema.sql) no banco criado, ainda com a conta administrativa.

No cliente de linha de comando do MySQL/MariaDB, aberto a partir da raiz do repositório:

```sql
USE agendamento;
SOURCE database/schema.sql;
```

Também é possível selecionar o banco no phpMyAdmin e usar **Importar**.

O esquema cria as tabelas `Clientes`, `Pets` e `Agendamentos`, com chaves estrangeiras e restrições de unicidade. Não contém `DROP TABLE` nem altera dados existentes.

**Banco antigo:** o projeto original não fornecia um esquema e misturava tabelas/colunas incompatíveis. Este script não converte uma instalação antiga. Faça backup e use um banco vazio separado para esta versão; copiar registros antigos exige conferir e mapear o esquema anterior.

## Configurar a conexão

No PowerShell, defina a conexão na mesma sessão em que vai executar o programa:

```powershell
$env:BANHOTOSA_CONNECTION_STRING = "Server=localhost;Database=agendamento;User ID=banhotosa;Password=SUA_SENHA;Connection Timeout=5"
```

Se o servidor usar outra porta, acrescente `Port=3307`, por exemplo. A variável também precisa estar disponível no processo do Visual Studio se você executar pela IDE.

A conexão é lida de **BANHOTOSA_CONNECTION_STRING**. Não há senha padrão nem criação automática do banco. A aplicação orienta a configuração quando a conexão está ausente ou não funciona.

## Compilar e executar

A partir da raiz:

```powershell
dotnet restore agendamento-banhotosa/agendamento-banhotosa.sln --locked-mode
dotnet build agendamento-banhotosa/agendamento-banhotosa.sln --configuration Release --no-restore
dotnet run --project agendamento-banhotosa/agendamento-banhotosa --configuration Release --no-build
```

Ou abra [agendamento-banhotosa.sln](agendamento-banhotosa/agendamento-banhotosa.sln) no Visual Studio.

O executável compilado fica em:

```text
agendamento-banhotosa/agendamento-banhotosa/bin/Release/net10.0-windows/agendamento-banhotosa.exe
```

Para verificar avisos de segurança nas dependências:

```powershell
dotnet list agendamento-banhotosa/agendamento-banhotosa package --vulnerable --include-transitive
```

## Como usar

### Cadastrar

1. Clique em **Fazer cadastro**.
2. Preencha nome, telefone com DDD, endereço, número, bairro, cidade e CEP.
3. Informe nome, raça e idade em anos do primeiro pet.
4. Use os campos **+ 1 PET** para acrescentar até três pets.
5. Clique em **Salvar**.

Telefone aceita 10 ou 11 dígitos, com DDD; espaços, parênteses e hífen são permitidos na digitação. CEP exige 8 dígitos. Idades são números inteiros não negativos. O telefone é único por cliente.

Para um telefone já cadastrado, use **Atualizar cadastro** na tela principal. O formulário carrega os dados antes da edição; o telefone e os pets existentes são preservados. É possível editar os pets ou adicionar novos até o limite de quatro; não há exclusão de pets nesta versão.

### Consultar e agendar

1. Informe o telefone do cliente.
2. Escolha a data e clique em **Consultar**.
3. Selecione o pet e marque **Banho**, **Tosa** ou ambos.
4. Marque um horário disponível.
5. Clique em **AGENDAR**.

Mudar o telefone ou a data invalida a consulta anterior para evitar agendar com informações de outra consulta.

### Cancelar

Consulte o telefone e a data, marque um agendamento do próprio cliente e clique em **Cancelar agendamento**. Confirme a operação. O cancelamento libera o horário para uma nova reserva.

### Regras da agenda

- Atendimento de segunda a sábado, das **9h às 17h**.
- Inícios de atendimento de hora em hora: **9h a 16h**.
- Uma reserva por horário, independentemente do pet ou do serviço.
- Banho, tosa e a combinação ocupam o mesmo intervalo de uma hora.
- Domingos, datas passadas, horários já encerrados e horários ocupados não aceitam novas reservas.
- Só agendamentos futuros podem ser cancelados.

Essas são as regras simplificadas deste projeto acadêmico. Não há múltiplos profissionais, duração variável por serviço, autenticação de usuários, pagamentos ou notificações.

## Testes

O projeto [tests/Checks.csproj](tests/Checks.csproj) usa verificações executáveis, sem pacote adicional de testes:

```powershell
dotnet run --project tests/Checks.csproj --configuration Release
```

Sem conexão de teste, executa validações e instancia os formulários; a integração com banco é informada como não executada.

Para testar persistência, configure uma conexão para **um servidor de testes** com permissão de criar e remover bancos e triggers:

```powershell
$env:BANHOTOSA_TEST_CONNECTION_STRING = "Server=localhost;User ID=USUARIO_DE_TESTES;Password=SENHA_DE_TESTES"
dotnet run --project tests/Checks.csproj --configuration Release
```

O teste cria um banco com nome aleatório iniciado por `banhotosa_test_`, aplica o esquema e remove **somente esse banco criado pelo teste** ao terminar. Não reutiliza o banco do aplicativo.

São verificados cadastro, atualização sem perda dos vínculos, pet de outro cliente, telefone duplicado, rollback quando a gravação de um pet falha, duas reservas concorrentes, cancelamento e reutilização da vaga.

### Validação realizada

Em **24/11/2026**, no Windows:

- Build Release com SDK .NET 10.0.404: **zero erros e zero avisos**.
- **37 verificações aprovadas**, incluindo integração em MariaDB isolado.
- Auditoria NuGet sem vulnerabilidades conhecidas nas dependências consultadas.

Os formulários foram instanciados nos testes. A navegação completa por cliques ainda precisa de uma conferência manual; não há teste visual automatizado.

## Estrutura

```text
├── agendamento-banhotosa/
│   ├── agendamento-banhotosa.sln
│   └── agendamento-banhotosa/
│       ├── Program.cs              # Inicialização do Windows Forms
│       ├── Form1.cs                # Consulta, agendamento e cancelamento
│       ├── Cadastro.cs             # Cadastro e atualização de cliente/pets
│       ├── Modelos.cs              # Dados e validações
│       ├── Repositorio.cs          # Consultas parametrizadas e transações
│       └── *.Designer.cs / *.resx   # Layout e recursos das telas
├── database/schema.sql
└── tests/
    ├── Checks.csproj
    └── Program.cs
```

Arquivos locais do Visual Studio, resultados de compilação e credenciais não devem ser enviados ao Git. A `.gitignore` inclui `.vs/`, `bin/`, `obj/`, `*.user` e `.env`.
