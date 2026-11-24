using MySqlConnector;

namespace agendamento_banhotosa;

internal sealed class Repositorio
{
    private readonly string conexao;

    public Repositorio(string? connectionString = null)
    {
        conexao = connectionString ??
            Environment.GetEnvironmentVariable("BANHOTOSA_CONNECTION_STRING") ?? "";
        if (string.IsNullOrWhiteSpace(conexao))
            throw new InvalidOperationException("Configure BANHOTOSA_CONNECTION_STRING antes de abrir o programa. Consulte o README.");
        var opcoes = new MySqlConnectionStringBuilder(conexao);
        if (string.IsNullOrWhiteSpace(opcoes.Database) || string.IsNullOrWhiteSpace(opcoes.UserID))
            throw new InvalidOperationException("A conexão deve informar Database e User ID.");
    }

    public async Task<Cliente?> ObterClienteAsync(string telefone)
    {
        telefone = Validacao.Telefone(telefone);
        await using var db = new MySqlConnection(conexao);
        await db.OpenAsync();
        using var comando = new MySqlCommand(
            "SELECT Id, Nome, Telefone, Endereco, Numero, Bairro, Cidade, Cep FROM Clientes WHERE Telefone=@telefone", db);
        comando.Parameters.AddWithValue("@telefone", telefone);
        Cliente cliente;
        await using (var leitor = await comando.ExecuteReaderAsync())
        {
            if (!await leitor.ReadAsync()) return null;
            cliente = new Cliente(leitor.GetInt64(0), leitor.GetString(1), leitor.GetString(2),
                leitor.GetString(3), leitor.GetString(4), leitor.GetString(5), leitor.GetString(6),
                leitor.GetString(7), []);
        }
        using var pets = new MySqlCommand("SELECT Id, Nome, Raca, Idade FROM Pets WHERE ClienteID=@id ORDER BY Id", db);
        pets.Parameters.AddWithValue("@id", cliente.Id);
        var lista = new List<Pet>();
        await using var linhas = await pets.ExecuteReaderAsync();
        while (await linhas.ReadAsync())
            lista.Add(new Pet(linhas.GetInt64(0), linhas.GetString(1), linhas.GetString(2), linhas.GetInt32(3)));
        return cliente with { Pets = lista };
    }

    public async Task<long> SalvarClienteAsync(Cliente cliente)
    {
        Validacao.Cliente(cliente);
        await using var db = new MySqlConnection(conexao);
        await db.OpenAsync();
        await using var transacao = await db.BeginTransactionAsync();
        try
        {
            long id = cliente.Id;
            if (id > 0)
            {
                using var busca = new MySqlCommand("SELECT Id FROM Clientes WHERE Id=@id AND Telefone=@telefone FOR UPDATE", db, transacao);
                busca.Parameters.AddWithValue("@id", id);
                busca.Parameters.AddWithValue("@telefone", cliente.Telefone);
                if (await busca.ExecuteScalarAsync() is null)
                    throw new InvalidOperationException("Cliente não encontrado. Consulte novamente pelo telefone.");
                using var buscaPets = new MySqlCommand("SELECT Id FROM Pets WHERE ClienteID=@id FOR UPDATE", db, transacao);
                buscaPets.Parameters.AddWithValue("@id", id);
                var ids = new HashSet<long>();
                await using (var leitor = await buscaPets.ExecuteReaderAsync())
                    while (await leitor.ReadAsync()) ids.Add(leitor.GetInt64(0));
                if (!ids.SetEquals(cliente.Pets.Where(p => p.Id > 0).Select(p => p.Id)))
                    throw new InvalidOperationException("Os pets cadastrados devem ser preservados. Reabra o cadastro para atualizar.");
            }
            else if (cliente.Pets.Any(p => p.Id != 0))
                throw new ArgumentException("Um novo cliente deve conter somente pets novos.");

            using var comando = new MySqlCommand(id == 0
                ? "INSERT INTO Clientes (Nome,Telefone,Endereco,Numero,Bairro,Cidade,Cep) VALUES (@nome,@telefone,@endereco,@numero,@bairro,@cidade,@cep)"
                : "UPDATE Clientes SET Nome=@nome,Endereco=@endereco,Numero=@numero,Bairro=@bairro,Cidade=@cidade,Cep=@cep WHERE Id=@id", db, transacao);
            comando.Parameters.AddWithValue("@id", id);
            comando.Parameters.AddWithValue("@nome", cliente.Nome.Trim());
            comando.Parameters.AddWithValue("@telefone", cliente.Telefone);
            comando.Parameters.AddWithValue("@endereco", cliente.Endereco.Trim());
            comando.Parameters.AddWithValue("@numero", cliente.Numero.Trim());
            comando.Parameters.AddWithValue("@bairro", cliente.Bairro.Trim());
            comando.Parameters.AddWithValue("@cidade", cliente.Cidade.Trim());
            comando.Parameters.AddWithValue("@cep", cliente.Cep);
            await comando.ExecuteNonQueryAsync();
            if (id == 0) id = comando.LastInsertedId;

            foreach (var pet in cliente.Pets)
            {
                using var gravarPet = new MySqlCommand(pet.Id == 0
                    ? "INSERT INTO Pets (ClienteID,Nome,Raca,Idade) VALUES (@cliente,@nome,@raca,@idade)"
                    : "UPDATE Pets SET Nome=@nome,Raca=@raca,Idade=@idade WHERE Id=@id AND ClienteID=@cliente", db, transacao);
                gravarPet.Parameters.AddWithValue("@id", pet.Id);
                gravarPet.Parameters.AddWithValue("@cliente", id);
                gravarPet.Parameters.AddWithValue("@nome", pet.Nome.Trim());
                gravarPet.Parameters.AddWithValue("@raca", pet.Raca.Trim());
                gravarPet.Parameters.AddWithValue("@idade", pet.Idade);
                await gravarPet.ExecuteNonQueryAsync();
            }
            await transacao.CommitAsync();
            return id;
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("Este telefone já está cadastrado. Use Atualizar cadastro.", ex);
        }
    }

    public async Task<IReadOnlyList<Agendamento>> ConsultarAgendaAsync(DateTime dia)
    {
        await using var db = new MySqlConnection(conexao);
        await db.OpenAsync();
        using var comando = new MySqlCommand(
            "SELECT a.Id,a.ClienteID,a.PetID,p.Nome,a.Inicio,a.Servico FROM Agendamentos a JOIN Pets p ON p.Id=a.PetID WHERE a.Inicio>=@dia AND a.Inicio<@fim ORDER BY a.Inicio", db);
        comando.Parameters.AddWithValue("@dia", dia.Date);
        comando.Parameters.AddWithValue("@fim", dia.Date.AddDays(1));
        var resultado = new List<Agendamento>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
            resultado.Add(new Agendamento(leitor.GetInt64(0), leitor.GetInt64(1),
                leitor.GetInt64(2), leitor.GetString(3), leitor.GetDateTime(4), leitor.GetString(5)));
        return resultado;
    }

    public async Task AgendarAsync(long clienteId, long petId, DateTime inicio, string servico)
    {
        Validacao.Agendamento(inicio, servico, DateTime.Now);
        await using var db = new MySqlConnection(conexao);
        await db.OpenAsync();
        using var comando = new MySqlCommand(
            "INSERT INTO Agendamentos (ClienteID,PetID,Inicio,Servico) SELECT @cliente,@pet,@inicio,@servico FROM Pets WHERE Id=@pet AND ClienteID=@cliente", db);
        comando.Parameters.AddWithValue("@cliente", clienteId);
        comando.Parameters.AddWithValue("@pet", petId);
        comando.Parameters.AddWithValue("@inicio", inicio);
        comando.Parameters.AddWithValue("@servico", servico);
        try
        {
            if (await comando.ExecuteNonQueryAsync() != 1)
                throw new InvalidOperationException("O pet não pertence ao cliente consultado.");
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new InvalidOperationException("O horário acabou de ser reservado. Consulte novamente.", ex);
        }
    }

    public async Task CancelarAsync(long agendamentoId, long clienteId)
    {
        await using var db = new MySqlConnection(conexao);
        await db.OpenAsync();
        using var comando = new MySqlCommand(
            "DELETE FROM Agendamentos WHERE Id=@id AND ClienteID=@cliente AND Inicio>@agora", db);
        comando.Parameters.AddWithValue("@id", agendamentoId);
        comando.Parameters.AddWithValue("@cliente", clienteId);
        comando.Parameters.AddWithValue("@agora", DateTime.Now);
        if (await comando.ExecuteNonQueryAsync() != 1)
            throw new InvalidOperationException("Agendamento futuro não encontrado para este cliente.");
    }
}
