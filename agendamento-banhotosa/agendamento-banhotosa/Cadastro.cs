using System.Globalization;
using MySqlConnector;

namespace agendamento_banhotosa;

public partial class Cadastro : Form
{
    internal string TelefoneSalvo { get; private set; } = "";
    private readonly long clienteId;
    private readonly long[] petIds = new long[4];
    private readonly Control[][] camposPets;
    private readonly TextBox[] nomes;
    private readonly TextBox[] racas;
    private readonly TextBox[] idades;

    public Cadastro() : this(null) { }

    internal Cadastro(Cliente? cliente)
    {
        InitializeComponent();
        Text = cliente is null ? "Cadastrar cliente e pets" : "Atualizar cliente e pets";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        cadastrar.Text = "Salvar";
        txtTelefone.Mask = "";
        txtTelefone.MaxLength = 20;
        txtTelefone.AccessibleName = "Telefone com DDD";
        txtCEP.Mask = "00000-000";
        txtNome.MaxLength = 120;
        txtEndereco.MaxLength = 200;
        txtNumero.MaxLength = 20;
        txtBairro.MaxLength = txtCidade.MaxLength = 80;
        nomes = [txtPetNome1, txtPetNome2, txtPetNome3, txtPetNome4];
        racas = [txtPetRaca1, txtPetRaca2, txtPetRaca3, txtPetRaca4];
        idades = [txtPetIdade1, txtPetIdade2, txtPetIdade3, txtPetIdade4];
        camposPets = [
            [label13, label12, label11, txtPetNome2, txtPetRaca2, txtPetIdade2],
            [label16, label15, label14, txtPetNome3, txtPetRaca3, txtPetIdade3],
            [label19, label18, label17, txtPetNome4, txtPetRaca4, txtPetIdade4]
        ];
        for (int i = 0; i < 4; i++)
        {
            nomes[i].MaxLength = racas[i].MaxLength = 80;
            idades[i].MaxLength = 5;
            nomes[i].AccessibleName = $"Nome do pet {i + 1}";
            racas[i].AccessibleName = $"Raça do pet {i + 1}";
            idades[i].AccessibleName = $"Idade em anos do pet {i + 1}";
        }
        var ordem = new Control[] { txtNome, txtTelefone, txtEndereco, txtNumero, txtBairro,
            txtCidade, txtCEP, txtPetNome1, txtPetRaca1, txtPetIdade1, checkBox1,
            txtPetNome2, txtPetRaca2, txtPetIdade2, checkBox3, txtPetNome3,
            txtPetRaca3, txtPetIdade3, checkBox2, txtPetNome4, txtPetRaca4, txtPetIdade4, cadastrar };
        for (int i = 0; i < ordem.Length; i++) ordem[i].TabIndex = i;
        if (cliente is not null)
        {
            if (cliente.Pets.Count > 4)
                throw new InvalidOperationException("Este cliente possui mais de quatro pets. O formulário não pode editar este Cadastro.");
            clienteId = cliente.Id;
            txtNome.Text = cliente.Nome;
            txtTelefone.Text = cliente.Telefone;
            txtTelefone.ReadOnly = true;
            txtEndereco.Text = cliente.Endereco;
            txtNumero.Text = cliente.Numero;
            txtBairro.Text = cliente.Bairro;
            txtCidade.Text = cliente.Cidade;
            txtCEP.Text = cliente.Cep;
            checkBox1.Checked = cliente.Pets.Count >= 2;
            checkBox3.Checked = cliente.Pets.Count >= 3;
            checkBox2.Checked = cliente.Pets.Count >= 4;
            checkBox1.Enabled = cliente.Pets.Count < 2;
            checkBox3.Enabled = cliente.Pets.Count < 3;
            checkBox2.Enabled = cliente.Pets.Count < 4;
            for (int i = 0; i < cliente.Pets.Count; i++)
            {
                var pet = cliente.Pets[i];
                petIds[i] = pet.Id;
                nomes[i].Text = pet.Nome;
                racas[i].Text = pet.Raca;
                idades[i].Text = pet.Idade.ToString(CultureInfo.InvariantCulture);
            }
        }
        AtualizarCamposPets();
    }

    internal void PreencherTelefone(string telefone) => txtTelefone.Text = telefone;

    private void AtualizarCamposPets()
    {
        if (camposPets is null) return;
        if (!checkBox1.Checked) checkBox3.Checked = false;
        if (!checkBox3.Checked) checkBox2.Checked = false;
        checkBox3.Visible = checkBox1.Checked;
        checkBox2.Visible = checkBox3.Checked;
        bool[] visiveis = [checkBox1.Checked, checkBox3.Checked, checkBox2.Checked];
        for (int i = 0; i < camposPets.Length; i++)
            foreach (var campo in camposPets[i]) campo.Visible = visiveis[i];
    }

    private void checkBox1_CheckedChanged(object? sender, EventArgs e) => AtualizarCamposPets();
    private void checkBox3_CheckedChanged(object? sender, EventArgs e) => AtualizarCamposPets();
    private void checkBox2_CheckedChanged(object? sender, EventArgs e) => AtualizarCamposPets();

    private async void cadastrar_Click(object? sender, EventArgs e)
    {
        Enabled = false;
        UseWaitCursor = true;
        try
        {
            int quantidade = checkBox2.Checked ? 4 : checkBox3.Checked ? 3 : checkBox1.Checked ? 2 : 1;
            var pets = new List<Pet>();
            for (int i = 0; i < quantidade; i++)
            {
                if (!int.TryParse(idades[i].Text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int idade))
                    throw new ArgumentException($"Informe a idade do pet {i + 1} em anos inteiros.");
                pets.Add(new Pet(petIds[i], nomes[i].Text.Trim(), racas[i].Text.Trim(), idade));
            }
            var cliente = new Cliente(clienteId, txtNome.Text.Trim(), Validacao.Telefone(txtTelefone.Text),
                txtEndereco.Text.Trim(), txtNumero.Text.Trim(), txtBairro.Text.Trim(),
                txtCidade.Text.Trim(), Validacao.Digitos(txtCEP.Text), pets);
            await new Repositorio().SalvarClienteAsync(cliente);
            TelefoneSalvo = cliente.Telefone;
            MessageBox.Show(this, "Cliente e pets salvos com sucesso.", "Cadastro");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or MySqlException)
        {
            MessageBox.Show(this, ex is MySqlException
                ? "Não foi possível salvar. Nenhum Cadastro parcial foi gravado. Confira a conexão e o esquema no README."
                : ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { Enabled = true; UseWaitCursor = false; }
    }
}
