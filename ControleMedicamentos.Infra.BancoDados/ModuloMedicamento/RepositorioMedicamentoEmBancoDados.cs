using ControleMedicamentos.Dominio.ModuloMedicamento;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ControleMedicamento.Infra.BancoDados.ModuloMedicamento
{
    public class RepositorioMedicamentoEmBancoDados : IRepositorioMedicamento
    {
        protected const string enderecoBanco =
            "Data Source=(LocalDb)\\MSSQLLocalDB;" +
            "Initial Catalog=ControleMedicamentosDb;" +
            "Integrated Security=True;" +
            "Pooling=False";

        #region SQL Queries
        private const string sqlInserir =
            @"INSERT INTO [TBMEDICAMENTO](
	            [NOME],
	            [DESCRICAO],
	            [LOTE],
	            [VALIDADE],
	            [QUANTIDADEDISPONIVEL],
	            [FORNECEDOR_ID]
            )
            VALUES
            (
	            @NOME,
	            @DESCRICAO,
	            @LOTE,
	            @VALIDADE,
	            @QUANTIDADEDISPONIVEL,
	            @FORNECEDOR_ID
            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBMEDICAMENTO]
			SET
				[NOME] = @NOME,
				[DESCRICAO] = @DESCRICAO,
				[LOTE] = @LOTE,
				[VALIDADE] = @VALIDADE,
				[QUANTIDADEDISPONIVEL] = @QUANTIDADEDISPONIVEL,
				[FORNECEDOR_ID] = @FORNECEDOR_ID
			WHERE
				[ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBMEDICAMENTO]
				WHERE 
					[ID] = @ID";

        private const string sqlSelecionarTodos =
            @"SELECT
				[ID],
				[NOME],
				[DESCRICAO],
				[LOTE],
				[VALIDADE],
				[QUANTIDADEDISPONIVEL],
				[FORNECEDOR_ID]
			FROM
				[TBMEDICAMENTO]";

        private const string sqlSelecionarPorId =
            @"SELECT
				[ID],
				[NOME],
				[DESCRICAO],
				[LOTE],
				[VALIDADE],
				[QUANTIDADEDISPONIVEL],
				[FORNECEDOR_ID]
			FROM
				[TBMEDICAMENTO]
			WHERE
				[ID] = @ID";

        #endregion

        public ValidationResult Inserir(Medicamento novoMedicamento)
        {
            var validador = new ValidadorMedicamento();

            var resultadoValidacao = validador.Validate(novoMedicamento);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosMedicamento(novoMedicamento, comandoInsercao);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            novoMedicamento.Id = Convert.ToInt32(id);

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Medicamento medicamento)
        {
            var validador = new ValidadorMedicamento();

            var resultadoValidacao = validador.Validate(medicamento);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            ConfigurarParametrosMedicamento(medicamento, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Medicamento medicamento)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("ID", medicamento.Id);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Medicamento> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorMedicamento = comandoSelecao.ExecuteReader();

            List<Medicamento> funcionarios = new List<Medicamento>();

            while (leitorMedicamento.Read())
            {
                Medicamento medicamento = ConverterParaMedicamento(leitorMedicamento);

                funcionarios.Add(medicamento);
            }

            conexaoComBanco.Close();

            return funcionarios;
        }

        public Medicamento SelecionarPorNumero(int id)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", id);

            conexaoComBanco.Open();
            SqlDataReader leitorMedicamento = comandoSelecao.ExecuteReader();

            Medicamento paciente = null;
            if (leitorMedicamento.Read())
                paciente = ConverterParaMedicamento(leitorMedicamento);

            conexaoComBanco.Close();

            return paciente;
        }

        private static Medicamento ConverterParaMedicamento(SqlDataReader leitorMedicamento)
        {
            int id = Convert.ToInt32(leitorMedicamento["ID"]);
            string nome = Convert.ToString(leitorMedicamento["NOME"]);
            string descricao = Convert.ToString(leitorMedicamento["DESCRICAO"]);
            string lote = Convert.ToString(leitorMedicamento["LOTE"]);
            DateTime validade = Convert.ToDateTime(leitorMedicamento["VALIDADE"]);
            int qtdDisponivel = Convert.ToInt32(leitorMedicamento["QUANTIDADEDISPONIVEL"]);
            int fornecedor_id = Convert.ToInt32(leitorMedicamento["FORNECEDOR_ID"]);

            RepositorioFornecedorEmBancoDados repositorioFornecedor = new RepositorioFornecedorEmBancoDados();

            var funcionario = new Medicamento
            {
                Id = id,
                Nome = nome,
                Descricao = descricao,
                Lote = lote,
                Validade = validade,
                QuantidadeDisponivel = qtdDisponivel,
                Fornecedor = repositorioFornecedor.SelecionarPorNumero(fornecedor_id)
            };

            return funcionario;
        }

        private static void ConfigurarParametrosMedicamento(Medicamento novoMedicamento, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ID", novoMedicamento.Id);
            comando.Parameters.AddWithValue("NOME", novoMedicamento.Nome);
            comando.Parameters.AddWithValue("DESCRICAO", novoMedicamento.Descricao);
            comando.Parameters.AddWithValue("LOTE", novoMedicamento.Lote);
            comando.Parameters.AddWithValue("VALIDADE", novoMedicamento.Validade);
            comando.Parameters.AddWithValue("QUANTIDADEDISPONIVEL", novoMedicamento.QuantidadeDisponivel);
            comando.Parameters.AddWithValue("FORNECEDOR_ID", novoMedicamento.Fornecedor.Id);
        }

    }
}
