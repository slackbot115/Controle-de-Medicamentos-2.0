using ControleMedicamento.Infra.BancoDados.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloFuncionario;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloPaciente;
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
				REQUISICAO.[ID],
				REQUISICAO.[FUNCIONARIO_ID],
				REQUISICAO.[PACIENTE_ID],
				REQUISICAO.[MEDICAMENTO_ID],
				REQUISICAO.[QUANTIDADEMEDICAMENTO],
				REQUISICAO.[DATA],
				FUNCIONARIO.[NOME] NOME_FUNCIONARIO,
				FUNCIONARIO.[LOGIN],
				FUNCIONARIO.[SENHA],
				PACIENTE.[NOME] NOME_PACIENTE,
				PACIENTE.[CARTAOSUS],
				MEDICAMENTO.[NOME] NOME_MEDICAMENTO,
				MEDICAMENTO.[DESCRICAO],
				MEDICAMENTO.[LOTE],
				MEDICAMENTO.[VALIDADE],
				MEDICAMENTO.[QUANTIDADEDISPONIVEL],
				MEDICAMENTO.[FORNECEDOR_ID],
				FORNECEDOR.[NOME] NOME_FORNECEDOR,
				FORNECEDOR.[TELEFONE],
				FORNECEDOR.[EMAIL],
				FORNECEDOR.[CIDADE],
				FORNECEDOR.[ESTADO]
			FROM
				[TBREQUISICAO] AS REQUISICAO 
					LEFT JOIN
						[TBFUNCIONARIO] AS FUNCIONARIO
					ON
						REQUISICAO.FUNCIONARIO_ID = FUNCIONARIO.ID
					LEFT JOIN
						[TBPACIENTE] AS PACIENTE
					ON
						REQUISICAO.PACIENTE_ID = PACIENTE.ID
					LEFT JOIN
						[TBMEDICAMENTO] AS MEDICAMENTO
					ON
						REQUISICAO.MEDICAMENTO_ID = MEDICAMENTO.ID
					LEFT JOIN
						[TBFORNECEDOR] AS FORNECEDOR
					ON
						MEDICAMENTO.FORNECEDOR_ID = FORNECEDOR.ID";

		private const string sqlSelecionarPorId =
			@"SELECT
				REQUISICAO.[ID],
				REQUISICAO.[FUNCIONARIO_ID],
				REQUISICAO.[PACIENTE_ID],
				REQUISICAO.[MEDICAMENTO_ID],
				REQUISICAO.[QUANTIDADEMEDICAMENTO],
				REQUISICAO.[DATA],
				FUNCIONARIO.[NOME] NOME_FUNCIONARIO,
				FUNCIONARIO.[LOGIN],
				FUNCIONARIO.[SENHA],
				PACIENTE.[NOME] NOME_PACIENTE,
				PACIENTE.[CARTAOSUS],
				MEDICAMENTO.[NOME] NOME_MEDICAMENTO,
				MEDICAMENTO.[DESCRICAO],
				MEDICAMENTO.[LOTE],
				MEDICAMENTO.[VALIDADE],
				MEDICAMENTO.[QUANTIDADEDISPONIVEL],
				MEDICAMENTO.[FORNECEDOR_ID],
				FORNECEDOR.[NOME] NOME_FORNECEDOR,
				FORNECEDOR.[TELEFONE],
				FORNECEDOR.[EMAIL],
				FORNECEDOR.[CIDADE],
				FORNECEDOR.[ESTADO]
			FROM
				[TBREQUISICAO] AS REQUISICAO 
					LEFT JOIN
						[TBFUNCIONARIO] AS FUNCIONARIO
					ON
						REQUISICAO.FUNCIONARIO_ID = FUNCIONARIO.ID
					LEFT JOIN
						[TBPACIENTE] AS PACIENTE
					ON
						REQUISICAO.PACIENTE_ID = PACIENTE.ID
					LEFT JOIN
						[TBMEDICAMENTO] AS MEDICAMENTO
					ON
						REQUISICAO.MEDICAMENTO_ID = MEDICAMENTO.ID
					LEFT JOIN
						[TBFORNECEDOR] AS FORNECEDOR
					ON
						MEDICAMENTO.FORNECEDOR_ID = FORNECEDOR.ID
			WHERE
				REQUISICAO.[ID] = @ID";

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
			int qtdMedicamento = Convert.ToInt32(leitorRequisicao["QUANTIDADEMEDICAMENTO"]);
			//DateTime data = Convert.ToDateTime(leitorRequisicao["DATA"]);

			int funcionario_id = Convert.ToInt32(leitorRequisicao["FUNCIONARIO_ID"]);
			string nome_funcionario = Convert.ToString(leitorRequisicao["NOME_FUNCIONARIO"]);
			string login = Convert.ToString(leitorRequisicao["LOGIN"]);
			string senha = Convert.ToString(leitorRequisicao["SENHA"]);

			int paciente_id = Convert.ToInt32(leitorRequisicao["PACIENTE_ID"]);
			string nome_paciente = Convert.ToString(leitorRequisicao["NOME_PACIENTE"]);
			string cartaosus = Convert.ToString(leitorRequisicao["CARTAOSUS"]);

			int medicamento_id = Convert.ToInt32(leitorRequisicao["MEDICAMENTO_ID"]);
			string nome_medicamento = Convert.ToString(leitorRequisicao["NOME_MEDICAMENTO"]);
			string descricao = Convert.ToString(leitorRequisicao["DESCRICAO"]);
			string lote = Convert.ToString(leitorRequisicao["LOTE"]);
			DateTime validade = Convert.ToDateTime(leitorRequisicao["VALIDADE"]);
			int quantidadedisponivel = Convert.ToInt32(leitorRequisicao["QUANTIDADEDISPONIVEL"]);
			
			int fornecedor_id = Convert.ToInt32(leitorRequisicao["FORNECEDOR_ID"]);
			string nome_fornecedor = Convert.ToString(leitorRequisicao["NOME_FORNECEDOR"]);
			string telefone = Convert.ToString(leitorRequisicao["TELEFONE"]);
			string email = Convert.ToString(leitorRequisicao["EMAIL"]);
			string cidade = Convert.ToString(leitorRequisicao["CIDADE"]);
			string estado = Convert.ToString(leitorRequisicao["ESTADO"]);


			var requisicao = new Requisicao
			{
				Id = id,
				Funcionario = new Funcionario()
                {
					Id = funcionario_id,
					Nome = nome_funcionario,
					Login = login,
					Senha = senha,
                },
				Paciente = new Paciente()
                {
					Id = paciente_id,
					Nome = nome_paciente,
					CartaoSUS = cartaosus,
                },
				Medicamento = new Medicamento()
                {
					Id = medicamento_id,
					Descricao = descricao,
					Lote = lote,
					Validade = validade,
					QuantidadeDisponivel = quantidadedisponivel,
					Fornecedor = new Fornecedor()
                    {
						Id = fornecedor_id,
						Nome = nome_fornecedor,
						Telefone = telefone,
						Email = email,
						Cidade = cidade,
						Estado = estado,
                    }
                },
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
