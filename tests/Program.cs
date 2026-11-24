using agendamento_banhotosa;
using MySqlConnector;

internal static class Program
{
    private static int checks;
    private static readonly DateTime Segunda = new(2030, 1, 7);
    private static readonly Cliente Exemplo = new(0, "Ana", "11999990000", "Rua A", "10",
        "Centro", "Suzano", "08600000", [new Pet(0, "Rex", "SRD", 2)]);

    private static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception("FALHOU: " + name);
        checks++;
    }

    private static void Rejeita(Action action, string name)
    {
        try { action(); }
        catch (ArgumentException) { checks++; return; }
        throw new Exception("Entrada inválida aceita: " + name);
    }

    private static async Task RejeitaAsync(Func<Task> action, string name)
    {
        try { await action(); }
        catch (InvalidOperationException) { checks++; return; }
        throw new Exception("Operação indevida aceita: " + name);
    }

    [STAThread]
    private static async Task Main()
    {
        Check(Validacao.Telefone("(11) 99999-0000") == "11999990000", "telefone formatado");
        Check(Validacao.Telefone("(11) 3333-4444") == "1133334444", "telefone fixo");
        Rejeita(() => Validacao.Telefone("11999990000 OR 1=1"), "telefone com SQL");
        Rejeita(() => Validacao.Telefone("123"), "telefone incompleto");
        Check(Validacao.Texto("  Ana  ", "Nome", 5) == "Ana", "espaços externos");
        Rejeita(() => Validacao.Texto("   ", "Nome", 5), "nome vazio");
        Rejeita(() => Validacao.Texto("a\nb", "Nome", 5), "controle no texto");
        Rejeita(() => Validacao.Cliente(Exemplo with { Cep = "123" }), "CEP");
        Rejeita(() => Validacao.Cliente(Exemplo with { Pets = [] }), "nenhum pet");
        Rejeita(() => Validacao.Cliente(Exemplo with { Pets = Enumerable.Repeat(Exemplo.Pets[0], 5).ToArray() }), "cinco pets");
        Rejeita(() => Validacao.Cliente(Exemplo with { Pets = [Exemplo.Pets[0] with { Idade = -1 }] }), "idade negativa");
        Check(Validacao.Horarios(Segunda).Count() == 8, "oito horários por dia");
        Check(!Validacao.Horarios(Segunda.AddDays(-1)).Any(), "domingo sem atendimento");
        Validacao.Agendamento(Segunda.AddHours(9), "Banho e tosa", Segunda);
        checks++;
        Rejeita(() => Validacao.Agendamento(Segunda.AddHours(9), "Banho", Segunda.AddHours(10)), "horário passado");
        Rejeita(() => Validacao.Agendamento(Segunda.AddHours(8), "Banho", Segunda), "antes da abertura");
        Rejeita(() => Validacao.Agendamento(Segunda.AddHours(17), "Banho", Segunda), "depois do fechamento");
        Rejeita(() => Validacao.Agendamento(Segunda.AddHours(9.5), "Banho", Segunda), "horário fracionado");
        Rejeita(() => Validacao.Agendamento(Segunda.AddHours(9), "", Segunda), "sem serviço");

        using (var form = new Form1())
        {
            Check(form.Controls.Find("comboBox1", true).Single() is ComboBox { DropDownStyle: ComboBoxStyle.DropDownList }, "pet precisa ser escolhido");
            Check(form.Controls.Find("DateTimePicker", true).Single() is DateTimePicker { MinDate: var data } && data == DateTime.Today, "data mínima");
        }
        using (var form = new Cadastro(Exemplo with { Id = 1, Pets = [new Pet(1, "Rex", "SRD", 2), new Pet(2, "Lua", "SRD", 3)] }))
        {
            Check(form.Controls.Find("txtTelefone", true).Single() is MaskedTextBox { ReadOnly: true }, "identidade do cliente preservada");
            Check(!form.Controls.Find("checkBox1", true).Single().Enabled, "pets existentes não são removidos por engano");
        }

        SynchronizationContext.SetSynchronizationContext(null);
        var servidor = Environment.GetEnvironmentVariable("BANHOTOSA_TEST_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(servidor))
            Console.WriteLine("Integração não executada: configure BANHOTOSA_TEST_CONNECTION_STRING para um servidor de testes.");
        else
            await IntegracaoAsync(servidor);
        Console.WriteLine($"OK: {checks} verificações aprovadas.");
    }

    private static async Task IntegracaoAsync(string servidor)
    {
        var nome = "banhotosa_test_" + Guid.NewGuid().ToString("N");
        var opcoes = new MySqlConnectionStringBuilder(servidor) { Database = "" };
        await using var admin = new MySqlConnection(opcoes.ConnectionString);
        await admin.OpenAsync();
        using var criar = new MySqlCommand($"CREATE DATABASE {nome} CHARACTER SET utf8mb4", admin);
        await criar.ExecuteNonQueryAsync();
        try
        {
            opcoes.Database = nome;
            await using var db = new MySqlConnection(opcoes.ConnectionString);
            await db.OpenAsync();
            using var esquema = new MySqlCommand(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "schema.sql")), db);
            await esquema.ExecuteNonQueryAsync();
            var banco = new Repositorio(opcoes.ConnectionString);
            var id = await banco.SalvarClienteAsync(Exemplo);
            var ana = (await banco.ObterClienteAsync(Exemplo.Telefone))!;
            Check(id == ana.Id && ana.Pets.Count == 1, "cliente e pet vinculados");
            await RejeitaAsync(async () => { await banco.SalvarClienteAsync(Exemplo); }, "telefone duplicado");

            var segundo = Exemplo with { Nome = "Bia", Telefone = "11999990001", Pets = [new Pet(0, "Lua", "SRD", 1)] };
            await banco.SalvarClienteAsync(segundo);
            var bia = (await banco.ObterClienteAsync(segundo.Telefone))!;
            await RejeitaAsync(() => banco.AgendarAsync(ana.Id, bia.Pets[0].Id, Segunda.AddHours(9), "Banho"), "pet de outro cliente");

            async Task<bool> Reservar()
            {
                try { await banco.AgendarAsync(ana.Id, ana.Pets[0].Id, Segunda.AddHours(9), "Banho"); return true; }
                catch (InvalidOperationException) { return false; }
            }
            var disputa = await Task.WhenAll(Reservar(), Reservar());
            Check(disputa.Count(ok => ok) == 1, "concorrência reserva uma única vaga");
            var agenda = await banco.ConsultarAgendaAsync(Segunda);
            Check(agenda.Count == 1 && agenda[0].PetId == ana.Pets[0].Id, "consulta de agendamento");
            await RejeitaAsync(() => banco.CancelarAsync(agenda[0].Id, bia.Id), "cancelamento de outro cliente");

            var alterada = ana with { Nome = "Ana O'Neil", Pets = [ana.Pets[0] with { Nome = "Rex Jr" }, new Pet(0, "Mel", "SRD", 0)] };
            await banco.SalvarClienteAsync(alterada);
            var atual = (await banco.ObterClienteAsync(ana.Telefone))!;
            Check(atual.Id == ana.Id && atual.Pets[0].Id == ana.Pets[0].Id && atual.Pets.Count == 2, "atualização preserva IDs");
            Check((await banco.ConsultarAgendaAsync(Segunda)).Single().NomePet == "Rex Jr", "agendamento mantém vínculo ao editar pet");
            Check(atual.Nome == "Ana O'Neil", "parâmetros preservam apóstrofos");
            await RejeitaAsync(async () => { await banco.SalvarClienteAsync(atual with { Pets = [atual.Pets[0]] }); }, "remoção involuntária de pet");

            using var trigger = new MySqlCommand("CREATE TRIGGER falha_pet BEFORE INSERT ON Pets FOR EACH ROW SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='falha de teste'", db);
            await trigger.ExecuteNonQueryAsync();
            var falha = Exemplo with { Telefone = "11999990002" };
            try { await banco.SalvarClienteAsync(falha); throw new Exception("Falha simulada não ocorreu."); }
            catch (MySqlException ex) when (ex.SqlState == "45000") { checks++; }
            Check(await banco.ObterClienteAsync(falha.Telefone) is null, "rollback do cliente quando pet falha");
            using var removerTrigger = new MySqlCommand("DROP TRIGGER falha_pet", db);
            await removerTrigger.ExecuteNonQueryAsync();

            await banco.CancelarAsync(agenda[0].Id, ana.Id);
            Check((await banco.ConsultarAgendaAsync(Segunda)).Count == 0, "cancelamento libera vaga");
            await banco.AgendarAsync(bia.Id, bia.Pets[0].Id, Segunda.AddHours(9), "Tosa");
            Check((await banco.ConsultarAgendaAsync(Segunda)).Single().ClienteId == bia.Id, "vaga pode ser reservada novamente");
        }
        finally
        {
            using var remover = new MySqlCommand($"DROP DATABASE {nome}", admin);
            await remover.ExecuteNonQueryAsync();
        }
    }
}
