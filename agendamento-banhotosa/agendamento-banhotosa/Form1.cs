using System.Data.SqlClient;

namespace agendamento_banhotosa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void agendar_Click(object sender, EventArgs e)
        {
            string telefone = txtTelefone.Text;
            using (SqlConnection connection = new SqlConnection("server=localhost;database=agendamento;uid=root;pwd='1234'"))
            {
                // Abra a conexão
                connection.Open();

                // Consulta o cliente com base no telefone
                string query = "SELECT COUNT(*) FROM cliente WHERE Telefone = @TELEFONE";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TELEFONE", telefone);

                int count = (int)command.ExecuteScalar();

                // Verifica se o cliente está cadastrado ou não
                if (count > 0)
                {
                    // Cliente cadastrado
                    // Realize o agendamento no banco de dados

                    // Obtém a data, horário e procedimento selecionados
                    DateTime dataSelecionada = dateTimePickerData.Value.Date;
                    string horarioSelecionado = GetHorarioSelecionado(); // Implemente a lógica para obter o horário selecionado
                    string procedimentoSelecionado = GetProcedimentoSelecionado(); // Implemente a lógica para obter o procedimento selecionado

                    // Insere o agendamento na tabela de agendamentos
                    string insertAgendamentoQuery = "INSERT INTO Agendamentos (Telefone, Data, Horario, Procedimento) " +
                        "VALUES (@Telefone, @Data, @Horario, @Procedimento)";
                    SqlCommand insertAgendamentoCommand = new SqlCommand(insertAgendamentoQuery, connection);
                    insertAgendamentoCommand.Parameters.AddWithValue("@Telefone", telefone);
                    insertAgendamentoCommand.Parameters.AddWithValue("@Data", dataSelecionada);
                    insertAgendamentoCommand.Parameters.AddWithValue("@Horario", horarioSelecionado);
                    insertAgendamentoCommand.Parameters.AddWithValue("@Procedimento", procedimentoSelecionado);

                    int rowsInserted = insertAgendamentoCommand.ExecuteNonQuery();

                    if (rowsInserted > 0)
                    {
                        MessageBox.Show("Agendamento realizado com sucesso.", "Sucesso");
                    }
                    else
                    {
                        MessageBox.Show("Erro ao realizar o agendamento.", "Erro");
                    }
                }
                else
                {
                    // Cliente não cadastrado
                    MessageBox.Show("Cliente não cadastrado. É necessário realizar o cadastro.", "Aviso");

                    // Implemente a lógica para abrir a tela de cadastro do cliente ou redirecionar para ela
                }

                // Fecha a conexão
                connection.Close();
            }
        }
    }
}