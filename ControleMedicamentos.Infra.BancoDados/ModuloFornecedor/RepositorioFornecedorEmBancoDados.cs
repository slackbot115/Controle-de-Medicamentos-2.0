using ControleMedicamentos.Dominio.ModuloFornecedor;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.ModuloFornecedor
{
    public class RepositorioFornecedorEmBancoDados : IRepositorioFornecedor
    {
        protected const string enderecoBanco =
            "Data Source=(LocalDb)\\MSSQLLocalDB;" +
            "Initial Catalog=ControleMedicamentosDb;" +
            "Integrated Security=True;" +
            "Pooling=False";

        #region SQL Queries
        private const string sqlInserir =
            @"INSERT INTO [TBFORNECEDOR](
	            [NOME],
	            [TELEFONE],
	            [EMAIL],
	            [CIDADE],
	            [ESTADO]
            )
            VALUES
            (
	            @NOME,
	            @TELEFONE,
	            @EMAIL,
	            @CIDADE,
	            @ESTADO
            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
			@"UPDATE [TBFORNECEDOR]
	            SET
		            [NOME] = @NOME,
		            [TELEFONE] = @TELEFONE,
		            [EMAIL] = @EMAIL,
		            [CIDADE] = @CIDADE,
		            [ESTADO] = @ESTADO
	            WHERE
		            [ID] = @ID";

		private const string sqlContarMedicamentosDoFornecedor =
			@"SELECT 
				count(*)
			FROM 
				[TBMEDICAMENTO] MEDICAMENTO INNER JOIN 
				[TBFORNECEDOR] FORNECEDOR
			ON 
				MEDICAMENTO.FORNECEDOR_ID = FORNECEDOR.ID
			WHERE
				MEDICAMENTO.FORNECEDOR_ID = @ID;";

        private const string sqlExcluir =
            @"DELETE FROM [TBFORNECEDOR]
	            WHERE
		            [ID] = @ID";

        private const string sqlSelecionarTodos =
			@"SELECT
				[ID],
				[NOME],
				[TELEFONE],
				[EMAIL],
				[CIDADE],
				[ESTADO]
			FROM
				[TBFORNECEDOR]";

		private const string sqlSelecionarPorId =
			@"SELECT
				[ID],
				[NOME],
				[TELEFONE],
				[EMAIL],
				[CIDADE],
				[ESTADO]
			FROM
				[TBFORNECEDOR]
			WHERE
				[ID] = @ID";

		#endregion

		public ValidationResult Inserir(Fornecedor novoFornecedor)
		{
			var validador = new ValidadorFornecedor();

			var resultadoValidacao = validador.Validate(novoFornecedor);

			if (resultadoValidacao.IsValid == false)
				return resultadoValidacao;

			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

			ConfigurarParametrosFornecedor(novoFornecedor, comandoInsercao);

			conexaoComBanco.Open();
			var id = comandoInsercao.ExecuteScalar();
			novoFornecedor.Id = Convert.ToInt32(id);

			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public ValidationResult Editar(Fornecedor fornecedor)
		{
			var validador = new ValidadorFornecedor();

			var resultadoValidacao = validador.Validate(fornecedor);

			if (resultadoValidacao.IsValid == false)
				return resultadoValidacao;

			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

			ConfigurarParametrosFornecedor(fornecedor, comandoEdicao);

			conexaoComBanco.Open();
			comandoEdicao.ExecuteNonQuery();
			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public ValidationResult Excluir(Fornecedor fornecedor)
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoVerificacao = new SqlCommand(sqlContarMedicamentosDoFornecedor, conexaoComBanco);
			comandoVerificacao.Parameters.AddWithValue("ID", fornecedor.Id);

			var resultadoValidacao = new ValidationResult();

			conexaoComBanco.Open();
			int numeroMedicamentosDoFornecedor = comandoVerificacao.ExecuteNonQuery();
			if (numeroMedicamentosDoFornecedor != 0)
            {
				resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro por haver medicamentos atrelados"));
				return resultadoValidacao;
            }

			SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

			comandoExclusao.Parameters.AddWithValue("ID", fornecedor.Id);

			int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

			if (numeroRegistrosExcluidos == 0)
				resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public List<Fornecedor> SelecionarTodos()
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

			conexaoComBanco.Open();
			SqlDataReader leitorFornecedor = comandoSelecao.ExecuteReader();

			List<Fornecedor> fornecedores = new List<Fornecedor>();

			while (leitorFornecedor.Read())
			{
				Fornecedor contato = ConverterParaFornecedor(leitorFornecedor);

				fornecedores.Add(contato);
			}

			conexaoComBanco.Close();

			return fornecedores;
		}

		public Fornecedor SelecionarPorNumero(int id)
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

			comandoSelecao.Parameters.AddWithValue("ID", id);

			conexaoComBanco.Open();
			SqlDataReader leitorContato = comandoSelecao.ExecuteReader();

			Fornecedor paciente = null;
			if (leitorContato.Read())
				paciente = ConverterParaFornecedor(leitorContato);

			conexaoComBanco.Close();

			return paciente;
		}

		private static Fornecedor ConverterParaFornecedor(SqlDataReader leitorFornecedor)
		{
			int id = Convert.ToInt32(leitorFornecedor["ID"]);
			string nome = Convert.ToString(leitorFornecedor["NOME"]);
			string telefone = Convert.ToString(leitorFornecedor["TELEFONE"]);
			string email = Convert.ToString(leitorFornecedor["EMAIL"]);
			string cidade = Convert.ToString(leitorFornecedor["CIDADE"]);
			string estado = Convert.ToString(leitorFornecedor["ESTADO"]);

			var fornecedor = new Fornecedor
			{
				Id = id,
				Nome = nome,
				Telefone = telefone,
				Email = email,
				Cidade = cidade,
				Estado = estado
			};

			return fornecedor;
		}

		private static void ConfigurarParametrosFornecedor(Fornecedor novoFornecedor, SqlCommand comando)
		{
			comando.Parameters.AddWithValue("ID", novoFornecedor.Id);
			comando.Parameters.AddWithValue("NOME", novoFornecedor.Nome);
			comando.Parameters.AddWithValue("TELEFONE", novoFornecedor.Telefone);
			comando.Parameters.AddWithValue("EMAIL", novoFornecedor.Email);
			comando.Parameters.AddWithValue("CIDADE", novoFornecedor.Cidade);
			comando.Parameters.AddWithValue("ESTADO", novoFornecedor.Estado);
		}

	}
}
