namespace agendamento_banhotosa;

internal sealed record Pet(long Id, string Nome, string Raca, int Idade)
{
    public override string ToString() => Nome;
}

internal sealed record Cliente(long Id, string Nome, string Telefone, string Endereco,
    string Numero, string Bairro, string Cidade, string Cep, IReadOnlyList<Pet> Pets);

internal sealed record Agendamento(long Id, long ClienteId, long PetId, string NomePet,
    DateTime Inicio, string Servico);

internal static class Validacao
{
    public static string Digitos(string valor) => new(valor.Where(c => c is >= '0' and <= '9').ToArray());

    public static string Telefone(string valor)
    {
        if (valor.Any(c => !char.IsAsciiDigit(c) && !" ()-".Contains(c)))
            throw new ArgumentException("Informe um telefone válido com DDD.");
        var telefone = Digitos(valor);
        if (telefone.Length is not (10 or 11))
            throw new ArgumentException("Informe o telefone com DDD, com 10 ou 11 dígitos.");
        return telefone;
    }

    public static string Texto(string valor, string campo, int limite)
    {
        valor = valor.Trim();
        if (string.IsNullOrWhiteSpace(valor) || valor.Length > limite || valor.Any(char.IsControl))
            throw new ArgumentException($"{campo}: informe de 1 a {limite} caracteres.");
        return valor;
    }

    public static void Cliente(Cliente cliente)
    {
        if (cliente.Id < 0) throw new ArgumentException("Cliente inválido.");
        Texto(cliente.Nome, "Nome", 120);
        if (Telefone(cliente.Telefone) != cliente.Telefone)
            throw new ArgumentException("O telefone deve conter somente os dígitos.");
        Texto(cliente.Endereco, "Endereço", 200);
        Texto(cliente.Numero, "Número", 20);
        Texto(cliente.Bairro, "Bairro", 80);
        Texto(cliente.Cidade, "Cidade", 80);
        if (cliente.Cep.Length != 8 || cliente.Cep.Any(c => !char.IsAsciiDigit(c)))
            throw new ArgumentException("Informe um CEP com 8 dígitos.");
        if (cliente.Pets.Count is < 1 or > 4)
            throw new ArgumentException("Cadastre de um a quatro pets.");
        foreach (var pet in cliente.Pets)
        {
            Texto(pet.Nome, "Nome do pet", 80);
            Texto(pet.Raca, "Raça", 80);
            if (pet.Id < 0 || pet.Idade is < 0 or > 32767)
                throw new ArgumentException("Informe uma idade inteira não negativa para o pet.");
        }
        var ids = cliente.Pets.Where(p => p.Id > 0).Select(p => p.Id).ToArray();
        if (ids.Distinct().Count() != ids.Length)
            throw new ArgumentException("Um pet não pode ser atualizado duas vezes.");
    }

    public static IEnumerable<DateTime> Horarios(DateTime dia)
    {
        if (dia.DayOfWeek == DayOfWeek.Sunday) yield break;
        for (int hora = 9; hora < 17; hora++) yield return dia.Date.AddHours(hora);
    }

    public static void Agendamento(DateTime inicio, string servico, DateTime agora)
    {
        if (inicio <= agora || !Horarios(inicio).Contains(inicio))
            throw new ArgumentException("Escolha um horário futuro, de segunda a sábado, das 9h às 16h.");
        if (servico is not ("Banho" or "Tosa" or "Banho e tosa"))
            throw new ArgumentException("Selecione banho, tosa ou ambos.");
    }
}
