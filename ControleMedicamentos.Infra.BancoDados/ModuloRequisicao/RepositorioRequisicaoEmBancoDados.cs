using ControleMedicamento.Infra.BancoDados.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloRequisicao;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using ControleMedicamentos.Infra.BancoDados.ModuloFuncionario;
using ControleMedicamentos.Infra.BancoDados.ModuloPaciente;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.ModuloRequisicao
{
    public class RepositorioRequisicaoEmBancoDados : IRepositorioRequisicao
    {
        protected const string enderecoBanco =
            "Data Source=(LocalDb)\\MSSQLLocalDB;" +
            "Initial Catalog=ControleMedicamentosDb;" +
            "Integrated Security=True;" +
            "Pooling=False";

        #region SQL Queries
        private const string sqlInserir =
            @"INSERT INTO [TBREQUISICAO](
	            [FUNCIONARIO_ID],
	            [PACIENTE_ID],
	            [MEDICAMENTO_ID],
	            [QUANTIDADEMEDICAMENTO],
	            [DATA]
            )
            VALUES
            (
	            @FUNCIONARIO_ID,
	            @PACIENTE_ID,
	            @MEDICAMENTO_ID,
	            @QUANTIDADEMEDICAMENTO,
	            @DATA
            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBREQUISICAO]
	            SET
		            [FUNCIONARIO_ID] = @FUNCIONARIO_ID,
		            [PACIENTE_ID] = @PACIENTE_ID,
		            [MEDICAMENTO_ID] = @MEDICAMENTO_ID,
		            [QUANTIDADEMEDICAMENTO] = @QUANTIDADEMEDICAMENTO,
		            [DATA] = @DATA
	            WHERE
		            [ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBREQUISICAO]
	            WHERE
		            [ID] = @ID";

        private const string sqlSelecionarTodos =
			@"SELECT
				[ID],
				[FUNCIONARIO_ID],
				[PACIENTE_ID],
				[MEDICAMENTO_ID],
				[QUANTIDADEMEDICAMENTO],
				[DATA]
			FROM
				[TBREQUISICAO]";

		private const string sqlSelecionarPorId =
			@"SELECT
				[ID],
				[FUNCIONARIO_ID],
				[PACIENTE_ID],
				[MEDICAMENTO_ID],
				[QUANTIDADEMEDICAMENTO],
				[DATA]
			FROM
				[TBREQUISICAO]
			WHERE
				[ID] = @ID";

		#endregion

		public ValidationResult Inserir(Requisicao novoRequisicao)
		{
			var validador = new ValidadorRequisicao();

			var resultadoValidacao = validador.Validate(novoRequisicao);

			if (resultadoValidacao.IsValid == false)
				return resultadoValidacao;

			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

			ConfigurarParametrosRequisicao(novoRequisicao, comandoInsercao);

			conexaoComBanco.Open();
			var id = comandoInsercao.ExecuteScalar();
			novoRequisicao.Id = Convert.ToInt32(id);

			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public ValidationResult Editar(Requisicao requisicao)
		{
			var validador = new ValidadorRequisicao();

			var resultadoValidacao = validador.Validate(requisicao);

			if (resultadoValidacao.IsValid == false)
				return resultadoValidacao;

			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

			ConfigurarParametrosRequisicao(requisicao, comandoEdicao);

			conexaoComBanco.Open();
			comandoEdicao.ExecuteNonQuery();
			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public ValidationResult Excluir(Requisicao requisicao)
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

			comandoExclusao.Parameters.AddWithValue("ID", requisicao.Id);

			conexaoComBanco.Open();
			int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

			var resultadoValidacao = new ValidationResult();

			if (numeroRegistrosExcluidos == 0)
				resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

			conexaoComBanco.Close();

			return resultadoValidacao;
		}

		public List<Requisicao> SelecionarTodos()
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

			conexaoComBanco.Open();
			SqlDataReader leitorRequisicao = comandoSelecao.ExecuteReader();

			List<Requisicao> requisicoes = new List<Requisicao>();

			while (leitorRequisicao.Read())
			{
				Requisicao requisicao = ConverterParaRequisicao(leitorRequisicao);

				requisicoes.Add(requisicao);
			}

			conexaoComBanco.Close();

			return requisicoes;
		}

		public Requisicao SelecionarPorNumero(int id)
		{
			SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

			SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

			comandoSelecao.Parameters.AddWithValue("ID", id);

			conexaoComBanco.Open();
			SqlDataReader leitorRequisicao = comandoSelecao.ExecuteReader();

			Requisicao requisicao = null;
			if (leitorRequisicao.Read())
				requisicao = ConverterParaRequisicao(leitorRequisicao);

			conexaoComBanco.Close();

			return requisicao;
		}

		private static Requisicao ConverterParaRequisicao(SqlDataReader leitorRequisicao)
		{
			int id = Convert.ToInt32(leitorRequisicao["ID"]);
			int funcionario_id = Convert.ToInt32(leitorRequisicao["FUNCIONARIO_ID"]);
			int paciente_id = Convert.ToInt32(leitorRequisicao["PACIENTE_ID"]);
			int medicamento_id = Convert.ToInt32(leitorRequisicao["MEDICAMENTO_ID"]);
			int qtdMedicamento = Convert.ToInt32(leitorRequisicao["QUANTIDADEMEDICAMENTO"]);
			//DateTime data = Convert.ToDateTime(leitorRequisicao["DATA"]);

			RepositorioFuncionarioEmBancoDados repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
			RepositorioPacienteEmBancoDados repositorioPaciente = new RepositorioPacienteEmBancoDados();
			RepositorioMedicamentoEmBancoDados repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();

			var requisicao = new Requisicao
			{
				Id = id,
				Funcionario = repositorioFuncionario.SelecionarPorNumero(funcionario_id),
				Paciente = repositorioPaciente.SelecionarPorNumero(paciente_id),
				Medicamento = repositorioMedicamento.SelecionarPorNumero(medicamento_id),
				QtdMedicamento = qtdMedicamento,
			};

			return requisicao;
		}

		private static void ConfigurarParametrosRequisicao(Requisicao novoRequisicao, SqlCommand comando)
		{
			comando.Parameters.AddWithValue("ID", novoRequisicao.Id);
			comando.Parameters.AddWithValue("FUNCIONARIO_ID", novoRequisicao.Funcionario.Id);
			comando.Parameters.AddWithValue("PACIENTE_ID", novoRequisicao.Paciente.Id);
			comando.Parameters.AddWithValue("MEDICAMENTO_ID", novoRequisicao.Medicamento.Id);
			comando.Parameters.AddWithValue("QUANTIDADEMEDICAMENTO", novoRequisicao.QtdMedicamento);
			comando.Parameters.AddWithValue("DATA", novoRequisicao.Data);
		}

	}
}
