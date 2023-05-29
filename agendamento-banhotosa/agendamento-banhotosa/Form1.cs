using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace agendamento_banhotosa
{
    public partial class Form1 : Form
    {
        private object connection;

        public Form1()
        {
            InitializeComponent();
            int idCliente;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void CarregarNomesPets(int idCliente)
        {
            // Crie uma lista para armazenar os nomes dos pets
            List<string> nomesPets = new List<string>();

            // Conecte-se ao banco de dados
            using (SqlConnection connection = new SqlConnection("server=localhost;database=agendamento;uid=root;pwd='1234'"))
            {
                // Abra a conexão
                connection.Open();

                // Consulta os nomes dos pets correspondentes ao idCliente
                string query = "SELECT Nome FROM Pet WHERE IdCliente = @IdCliente";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCliente", idCliente);

                // Execute a consulta e leia os resultados
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nomePet = reader.GetString(0);
                        nomesPets.Add(nomePet);
                    }
                }

                // Preencha o ComboBox com os nomes dos pets
                //comboBox1.DataSource = nomesPets;
                comboBox1.Items.AddRange(nomesPets.ToArray());

                // Fecha a conexão
                connection.Close();
            }
        }

        private void agendar_Click(object sender, EventArgs e)
        {
            string telefone = txtTelefone.Text;

            // Verifica se o cliente está cadastrado
            if (VerificarClienteCadastrado(telefone))
            {
                // Obtém o idCliente com base no número de telefone
                int idCliente = ObterIdCliente(telefone, connection);

                // Verifica se o cliente está cadastrado
                if (idCliente > 0)
                {
                    // Carrega os nomes dos pets do cliente no ComboBox
                    CarregarNomesPets(idCliente);
                }


                // Obtém a data, horário e procedimento selecionados
                DateTime dataSelecionada = DateTimePicker.Value.Date;
                string horarioSelecionado = GetHorarioSelecionado(); // Implemente a lógica para obter o horário selecionado
                string procedimentoSelecionado = GetProcedimentoSelecionado(); // Implemente a lógica para obter o procedimento selecionado

                // Conecte-se ao banco de dados
                using (SqlConnection connection = new SqlConnection("server=localhost;database=agendamento;uid=root;pwd='1234'"))
                {
                    // Abre a conexão
                    connection.Open();

                    // Consulta o ID do cliente com base no telefone
                    idCliente = ObterIdCliente(telefone, connection);

                    // Insere o agendamento na tabela de agendamentos
                    string query = "INSERT INTO Agendamentos (IdCliente, NomePet, Data, Horario, Procedimento) " +
                        "VALUES (@IdCliente, @NomePet, @Data, @Horario, @Procedimento)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.Parameters.AddWithValue("@NomePet", nomePetSelecionado);
                    command.Parameters.AddWithValue("@Data", dataSelecionada);
                    command.Parameters.AddWithValue("@Horario", horarioSelecionado);
                    command.Parameters.AddWithValue("@Procedimento", procedimentoSelecionado);

                    int rowsInserted = command.ExecuteNonQuery();

                    if (rowsInserted > 0)
                    {
                        MessageBox.Show("Agendamento realizado com sucesso.", "Sucesso");
                    }
                    else
                    {
                        MessageBox.Show("Erro ao realizar o agendamento.", "Erro");
                    }

                    // Fecha a conexão
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Cliente não cadastrado. É necessário realizar o cadastro.", "Aviso");

                // Implemente a lógica para abrir a tela de cadastro do cliente ou redirecionar para ela
            }
        }

        private int ObterIdCliente(string telefone, object connection)
        {
            throw new NotImplementedException();
        }

        private bool VerificarClienteCadastrado(string telefone)
        {
            // Conecte-se ao banco de dados
            using (SqlConnection connection = new SqlConnection("server=localhost;database=agendamento;uid=root;pwd='1234'"))
            {
                // Abre a conexão
                connection.Open();

                // Consulta o banco de dados para verificar se o cliente está cadastrado
                string query = "SELECT COUNT(*) FROM Clientes WHERE Telefone = @Telefone";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Telefone", telefone);

                int count = (int)command.ExecuteScalar();

                // Retorna verdadeiro se o cliente está cadastrado, caso contrário, retorna falso
                return count > 0;
            }
        }



    }
}
