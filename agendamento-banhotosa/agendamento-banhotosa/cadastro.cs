using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace agendamento_banhotosa
{

    public partial class cadastro : Form
    {
        private SqlConnection connection;
        public cadastro()
        {
            InitializeComponent();


            // conexão com o banco de dados
            string connectionString = "server=localhost;database=agendamento;uid=root;pwd='1234'";
            connection = new SqlConnection(connectionString);
        }

        private void cadastrar_Click(object sender, EventArgs e)
        {
            string nome = txtNome.Text;
            string telefone = txtTelefone.Text;
            string endereco = txtEndereco.Text;
            string numero = txtNumero.Text;
            string bairro = txtBairro.Text;
            string cidade = txtCidade.Text;
            string cep = txtCEP.Text;

            try
            {
                // Abre a conexão
                connection.Open();

                // Verifica se o cliente já está cadastrado com base no número de telefone
                string queryVerificarCliente = "SELECT COUNT(*) FROM Clientes WHERE Telefone = @TELEFONE";
                SqlCommand commandVerificarCliente = new SqlCommand(queryVerificarCliente, connection);
                commandVerificarCliente.Parameters.AddWithValue("@TELEFONE", telefone);

                int count = (int)commandVerificarCliente.ExecuteScalar();

                if (count > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Cliente já cadastrado. Deseja atualizar os dados?", "Confirmação", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // Obtém os novos dados do cliente
                        string queryAtualizarCliente = "UPDATE Clientes SET Nome = @NOME, Endereco = @ENDERECO, Numero = @NUMERO, Bairro = @BAIRRO, Cidade = @CIDADE, CEP = @CEP WHERE Telefone = @TELEFONE";
                        SqlCommand commandAtualizarCliente = new SqlCommand(queryAtualizarCliente, connection);
                        commandAtualizarCliente.Parameters.AddWithValue("@NONE", nome);
                        commandAtualizarCliente.Parameters.AddWithValue("@ENDERECO", endereco);
                        commandAtualizarCliente.Parameters.AddWithValue("@NUMERO", numero);
                        commandAtualizarCliente.Parameters.AddWithValue("@BAIRRO", bairro);
                        commandAtualizarCliente.Parameters.AddWithValue("@CIDADE", cidade);
                        commandAtualizarCliente.Parameters.AddWithValue("@CEP", cep);
                        commandAtualizarCliente.Parameters.AddWithValue("@TELEFONE", telefone);

                        int rowsUpdated = commandAtualizarCliente.ExecuteNonQuery();

                        MessageBox.Show("Dados do cliente atualizados com sucesso.", "Sucesso");
                    }
                }
                else
                {
                    // Insere os dados do novo cliente no banco de dados
                    string queryInserirCliente = "INSERT INTO Clientes (NOME, TELEFONE, ENDERECO, NUMERO, BAIRRO, CIDADE, CEP) " +
                                                 "VALUES (@NOME, @TELEFONE, @ENDERECO, @NUMERO, @BAIRRO, @CIDADE, @CEP)";
                    SqlCommand commandInserirCliente = new SqlCommand(queryInserirCliente, connection);
                    commandInserirCliente.Parameters.AddWithValue("@NOME", nome);
                    commandInserirCliente.Parameters.AddWithValue("@TELEFONE", telefone);
                    commandInserirCliente.Parameters.AddWithValue("@ENDERECO", endereco);
                    commandInserirCliente.Parameters.AddWithValue("@NUMERO", numero);
                    commandInserirCliente.Parameters.AddWithValue("@BAIRRO", bairro);
                    commandInserirCliente.Parameters.AddWithValue("@CIDADE", cidade);
                    commandInserirCliente.Parameters.AddWithValue("@CEP", cep);

                    int rowsInserted = commandInserirCliente.ExecuteNonQuery();

                    MessageBox.Show("Cliente cadastrado com sucesso.", "Sucesso");
                }


                string petNome1 = "";
                string petRaca1 = "";
                string petIdade1 = "";
                string petNome2 = "";
                string petRaca2 = "";
                string petIdade2 = "";
                string petNome3 = "";
                string petRaca3 = "";
                string petIdade3 = "";
                string petNome4 = "";
                string petRaca4 = "";
                string petIdade4 = "";

                petNome1 = txtPetNome1.Text;
                petRaca1 = txtPetRaca1.Text;
                petIdade1 = (txtPetIdade1.Text);
                petNome2 = txtPetNome2.Text;
                petRaca2 = txtPetRaca2.Text;
                petIdade2 = (txtPetIdade2.Text);
                petNome3 = txtPetNome3.Text;
                petRaca3 = txtPetRaca3.Text;
                petIdade3 = (txtPetIdade3.Text);
                petNome4 = txtPetNome4.Text;
                petRaca4 = txtPetRaca4.Text;
                petIdade4 = (txtPetIdade4.Text);


                string queryInserirPet = "INSERT INTO pets (NOME_1, RACA_1, IDADE_1,NOME_2, RACA_2, IDADE_2,NOME_3, RACA_3, IDADE_3,NOME_4, RACA_4, IDADE_4, ClienteID) " +
                                             "VALUES (@Nome_1, @Raca_1, @Idade_1, @Nome_2, @Raca_2, @Idade_2, @Nome_3, @Raca_3, @Idade_3, @Nome_4, @Raca_4, @Idade_4 ,SCOPE_IDENTITY())";
                SqlCommand commandInserirPet = new SqlCommand(queryInserirPet, connection);

                commandInserirPet.Parameters.AddWithValue("@NOME_1", petNome1);
                commandInserirPet.Parameters.AddWithValue("@RACA_1", petRaca1);
                commandInserirPet.Parameters.AddWithValue("@IDADE_1", petIdade1);
                commandInserirPet.Parameters.AddWithValue("@NOME_2", petNome2);
                commandInserirPet.Parameters.AddWithValue("@RACA_2", petRaca2);
                commandInserirPet.Parameters.AddWithValue("@IDADE_2", petIdade2);
                commandInserirPet.Parameters.AddWithValue("@NOME_3", petNome3);
                commandInserirPet.Parameters.AddWithValue("@RACA_3", petRaca3);
                commandInserirPet.Parameters.AddWithValue("@IDADE_3", petIdade3);
                commandInserirPet.Parameters.AddWithValue("@NOME_4", petNome4);
                commandInserirPet.Parameters.AddWithValue("@RACA_4", petRaca4);
                commandInserirPet.Parameters.AddWithValue("@IDADE_4", petIdade4);


                int rowsInsertedPet = commandInserirPet.ExecuteNonQuery();




            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar cliente: " + ex.Message, "Erro");
            }
            finally
            {
                // Fecha a conexão com o banco de dados
                connection.Close();
            }
        }

        private void textBoxPet2Raca_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label13.Visible = true;
                label12.Visible = true;
                label11.Visible = true;
                txtPetNome2.Visible = true;
                txtPetRaca2.Visible = true;
                txtPetIdade2.Visible = true;
                checkBox3.Visible = true;
            }
            else
            {

                label13.Visible = false;
                label12.Visible = false;
                label11.Visible = false;
                txtPetNome2.Visible = false;
                txtPetRaca2.Visible = false;
                txtPetIdade2.Visible = false;
                checkBox3.Visible = false;
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label16.Visible = true;
                label15.Visible = true;
                label14.Visible = true;
                txtPetNome3.Visible = true;
                txtPetRaca3.Visible = true;
                txtPetIdade3.Visible = true;
                checkBox2.Visible = true;
            }
            else
            {

                label16.Visible = false;
                label15.Visible = false;
                label14.Visible = false;
                txtPetNome3.Visible = false;
                txtPetRaca3.Visible = false;
                txtPetIdade3.Visible = false;
                checkBox2.Visible = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label19.Visible = true;
                label17.Visible = true;
                label18.Visible = true;
                txtPetNome4.Visible = true;
                txtPetRaca4.Visible = true;
                txtPetIdade4.Visible = true;

            }
            else
            {

                label19.Visible = false;
                label17.Visible = false;
                label18.Visible = false;
                txtPetNome4.Visible = false;
                txtPetRaca4.Visible = false;
                txtPetIdade4.Visible = false;

            }
        }

        private void cadastro_Load(object sender, EventArgs e)
        {

        }
    }
}
