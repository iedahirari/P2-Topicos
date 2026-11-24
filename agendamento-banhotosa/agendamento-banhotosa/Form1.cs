using MySqlConnector;

namespace agendamento_banhotosa;

public partial class Form1 : Form
{
    private Cliente? cliente;

    private sealed record Horario(DateTime Inicio, Agendamento? Marcacao, string Descricao)
    {
        public override string ToString() => Descricao;
    }

    public Form1()
    {
        InitializeComponent();
        Text = "Banho e tosa — Agendamento";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        txtTelefone.Mask = "";
        txtTelefone.MaxLength = 20;
        txtTelefone.AccessibleName = "Telefone do cliente com DDD";
        comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox1.AccessibleName = "Pet do cliente consultado";
        checkedListBox1.CheckOnClick = true;
        checkedListBox1.AccessibleName = "Horários disponíveis e agendamentos do cliente";
        DateTimePicker.Format = DateTimePickerFormat.Custom;
        DateTimePicker.CustomFormat = "dd/MM/yyyy";
        DateTimePicker.MinDate = DateTime.Today;
        label6.Text = "Data:";
        label4.Text = "Seg. a sáb., das 9h às 17h.";
        button2.Text = "Atualizar cadastro";
        button1.Click += NovoCadastro_Click;
        button2.Click += AtualizarCadastro_Click;
        txtTelefone.TextChanged += (_, _) => LimparConsulta();
        DateTimePicker.ValueChanged += (_, _) => checkedListBox1.Items.Clear();
        checkedListBox1.ItemCheck += Horario_ItemCheck;
        var ordem = new Control[] { txtTelefone, button1, button2, comboBox1,
            checkBox1, checkBox2, DateTimePicker, consultar, checkedListBox1, agendar, button3 };
        for (int i = 0; i < ordem.Length; i++) ordem[i].TabIndex = i;
    }

    private void LimparConsulta()
    {
        cliente = null;
        comboBox1.DataSource = null;
        checkedListBox1.Items.Clear();
    }

    private void Horario_ItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (e.NewValue != CheckState.Checked) return;
        var horario = (Horario)checkedListBox1.Items[e.Index];
        if (horario.Inicio <= DateTime.Now ||
            horario.Marcacao is { } marcacao && marcacao.ClienteId != cliente?.Id)
        {
            e.NewValue = CheckState.Unchecked;
            return;
        }
        for (int i = 0; i < checkedListBox1.Items.Count; i++)
            if (i != e.Index) checkedListBox1.SetItemChecked(i, false);
    }

    private async Task ExecutarAsync(Func<Task> acao)
    {
        Enabled = false;
        UseWaitCursor = true;
        try { await acao(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or MySqlException)
        {
            MessageBox.Show(this, ex is MySqlException
                ? "Não foi possível acessar o banco. Confira a conexão e o esquema descritos no README."
                : ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { Enabled = true; UseWaitCursor = false; }
    }

    private async Task ConsultarAsync()
    {
        var telefone = Validacao.Telefone(txtTelefone.Text);
        LimparConsulta();
        var banco = new Repositorio();
        cliente = await banco.ObterClienteAsync(telefone)
            ?? throw new InvalidOperationException("Cliente não cadastrado. Clique em Fazer cadastro.");
        comboBox1.DataSource = cliente.Pets.ToArray();
        var agenda = await banco.ConsultarAgendaAsync(DateTimePicker.Value);
        foreach (var inicio in Validacao.Horarios(DateTimePicker.Value))
        {
            var marcacao = agenda.SingleOrDefault(a => a.Inicio == inicio);
            var descricao = marcacao is null ? "Disponível"
                : marcacao.ClienteId == cliente.Id ? $"{marcacao.NomePet} — {marcacao.Servico} (agendado)" : "Ocupado";
            if (inicio <= DateTime.Now) descricao = "Encerrado";
            checkedListBox1.Items.Add(new Horario(inicio, marcacao, $"{inicio:HH:mm} — {descricao}"));
        }
        if (checkedListBox1.Items.Count == 0)
            MessageBox.Show(this, "Não há atendimento aos domingos.", "Horários");
    }

    private async void button1_Click(object? sender, EventArgs e) =>
        await ExecutarAsync(ConsultarAsync);

    private void NovoCadastro_Click(object? sender, EventArgs e)
    {
        using var tela = new Cadastro();
        tela.PreencherTelefone(txtTelefone.Text);
        if (tela.ShowDialog(this) == DialogResult.OK)
        {
            txtTelefone.Text = tela.TelefoneSalvo;
            LimparConsulta();
        }
    }

    private async void AtualizarCadastro_Click(object? sender, EventArgs e)
    {
        await ExecutarAsync(async () =>
        {
            var atual = await new Repositorio().ObterClienteAsync(txtTelefone.Text)
                ?? throw new InvalidOperationException("Cliente não encontrado pelo telefone.");
            using var tela = new Cadastro(atual);
            if (tela.ShowDialog(this) == DialogResult.OK) LimparConsulta();
        });
    }

    private async void agendar_Click(object? sender, EventArgs e)
    {
        await ExecutarAsync(async () =>
        {
            if (cliente is null || comboBox1.SelectedItem is not Pet pet ||
                checkedListBox1.CheckedItems.Count != 1)
                throw new ArgumentException("Consulte o telefone e a data, selecione um pet e marque um horário.");
            var horario = checkedListBox1.CheckedItems[0] as Horario
                ?? throw new ArgumentException("Selecione um horário.");
            if (horario.Marcacao is not null)
                throw new ArgumentException("Este horário já está ocupado.");
            var servico = (checkBox1.Checked, checkBox2.Checked) switch
            {
                (true, true) => "Banho e tosa",
                (true, false) => "Banho",
                (false, true) => "Tosa",
                _ => ""
            };
            await new Repositorio().AgendarAsync(cliente.Id, pet.Id, horario.Inicio, servico);
            MessageBox.Show(this, "Agendamento realizado com sucesso.", "Agendamento");
            await ConsultarAsync();
        });
    }

    private async void button3_Click(object? sender, EventArgs e)
    {
        await ExecutarAsync(async () =>
        {
            if (cliente is null || checkedListBox1.CheckedItems.Count != 1)
                throw new ArgumentException("Consulte o cliente e marque o agendamento que deseja cancelar.");
            var horario = checkedListBox1.CheckedItems[0] as Horario
                ?? throw new ArgumentException("Selecione um horário.");
            if (horario.Marcacao is not { } marcacao || marcacao.ClienteId != cliente.Id)
                throw new ArgumentException("Selecione um agendamento deste cliente.");
            if (MessageBox.Show(this, $"Cancelar o agendamento de {marcacao.NomePet} em {marcacao.Inicio:dd/MM/yyyy HH:mm}?",
                "Confirmar cancelamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await new Repositorio().CancelarAsync(marcacao.Id, cliente.Id);
            await ConsultarAsync();
        });
    }
}
