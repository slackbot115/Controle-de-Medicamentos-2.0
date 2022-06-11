using ControleMedicamentos.Dominio.ModuloFuncionario;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.ModuloFuncionario
{
    public class RepositorioFuncionarioEmBancoDados : IRepositorioFuncionario
    {
        protected const string enderecoBanco =
            "Data Source=(LocalDb)\\MSSQLLocalDB;" +
            "Initial Catalog=ControleMedicamentosDb;" +
            "Integrated Security=True;" +
            "Pooling=False";

        #region SQL Queries
        private const string sqlInserir =
            @"INSERT INTO [TBFUNCIONARIO](
	            [NOME],
	            [LOGIN],
	            [SENHA]
            )
            VALUES
            (
	            @NOME,
	            @LOGIN,
	            @SENHA
            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBFUNCIONARIO]
	            SET
		            [NOME] = @NOME,
		            [LOGIN] = @LOGIN,
		            [SENHA] = @SENHA
	            WHERE
		            [ID] = @ID
            ";

        private const string sqlExcluir =
            @"DELETE FROM [TBFUNCIONARIO]
	            WHERE
		            [ID] = @ID";

        private const string sqlSelecionarTodos =
            @"SELECT
	            [ID],
	            [NOME],
	            [LOGIN],
	            [SENHA]
            FROM
	            [TBFUNCIONARIO]";

        private const string sqlSelecionarPorId =
            @"SELECT
	            [ID],
	            [NOME],
	            [LOGIN],
	            [SENHA]
            FROM
	            [TBFUNCIONARIO]
            WHERE
	            [ID] = @ID";

        #endregion

        public ValidationResult Inserir(Funcionario novoFuncionario)
        {
            var validador = new ValidadorFuncionario();

            var resultadoValidacao = validador.Validate(novoFuncionario);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosFuncionario(novoFuncionario, comandoInsercao);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            novoFuncionario.Id = Convert.ToInt32(id);

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Funcionario funcionario)
        {
            var validador = new ValidadorFuncionario();

            var resultadoValidacao = validador.Validate(funcionario);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            ConfigurarParametrosFuncionario(funcionario, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Funcionario funcionario)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", funcionario.Id);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Funcionario> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorFuncionario = comandoSelecao.ExecuteReader();

            List<Funcionario> funcionarios = new List<Funcionario>();

            while (leitorFuncionario.Read())
            {
                Funcionario contato = ConverterParaFuncionario(leitorFuncionario);

                funcionarios.Add(contato);
            }

            conexaoComBanco.Close();

            return funcionarios;
        }

        public Funcionario SelecionarPorNumero(int id)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", id);

            conexaoComBanco.Open();
            SqlDataReader leitorContato = comandoSelecao.ExecuteReader();

            Funcionario paciente = null;
            if (leitorContato.Read())
                paciente = ConverterParaFuncionario(leitorContato);

            conexaoComBanco.Close();

            return paciente;
        }

        private static Funcionario ConverterParaFuncionario(SqlDataReader leitorFuncionario)
        {
            int id = Convert.ToInt32(leitorFuncionario["ID"]);
            string nome = Convert.ToString(leitorFuncionario["NOME"]);
            string login = Convert.ToString(leitorFuncionario["LOGIN"]);
            string senha = Convert.ToString(leitorFuncionario["SENHA"]);

            var funcionario = new Funcionario
            {
                Id = id,
                Nome = nome,
                Login = login,
                Senha = senha
            };

            return funcionario;
        }

        private static void ConfigurarParametrosFuncionario(Funcionario novoFuncionario, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ID", novoFuncionario.Id);
            comando.Parameters.AddWithValue("NOME", novoFuncionario.Nome);
            comando.Parameters.AddWithValue("LOGIN", novoFuncionario.Login);
            comando.Parameters.AddWithValue("SENHA", novoFuncionario.Senha);
        }

    }
}
