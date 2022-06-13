using ControleMedicamentos.Dominio.ModuloFornecedor;
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
	            MEDICAMENTO.[ID],
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
	            [TBMEDICAMENTO] AS MEDICAMENTO LEFT JOIN
	            [TBFORNECEDOR] FORNECEDOR
            ON
	            MEDICAMENTO.FORNECEDOR_ID = FORNECEDOR.ID";

        private const string sqlSelecionarPorId =
            @"SELECT 
	            MEDICAMENTO.[ID],
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
	            [TBMEDICAMENTO] AS MEDICAMENTO LEFT JOIN
	            [TBFORNECEDOR] FORNECEDOR
            ON
	            MEDICAMENTO.FORNECEDOR_ID = FORNECEDOR.ID
            WHERE
	            MEDICAMENTO.[ID] = @ID;";

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

            List<Medicamento> medicamentos = new List<Medicamento>();

            while (leitorMedicamento.Read())
            {
                Medicamento medicamento = ConverterParaMedicamento(leitorMedicamento);

                medicamentos.Add(medicamento);
            }

            conexaoComBanco.Close();

            return medicamentos;
        }

        public Medicamento SelecionarPorNumero(int id)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", id);

            conexaoComBanco.Open();
            SqlDataReader leitorMedicamento = comandoSelecao.ExecuteReader();

            Medicamento medicamento = null;
            if (leitorMedicamento.Read())
                medicamento = ConverterParaMedicamento(leitorMedicamento);

            conexaoComBanco.Close();

            return medicamento;
        }

        private static Medicamento ConverterParaMedicamento(SqlDataReader leitorMedicamento)
        {
            int id = Convert.ToInt32(leitorMedicamento["ID"]);
            string nome_medicamento = Convert.ToString(leitorMedicamento["NOME_MEDICAMENTO"]);
            string descricao = Convert.ToString(leitorMedicamento["DESCRICAO"]);
            string lote = Convert.ToString(leitorMedicamento["LOTE"]);
            DateTime validade = Convert.ToDateTime(leitorMedicamento["VALIDADE"]);
            int qtdDisponivel = Convert.ToInt32(leitorMedicamento["QUANTIDADEDISPONIVEL"]);
            
            int fornecedor_id = Convert.ToInt32(leitorMedicamento["FORNECEDOR_ID"]);
            string nome_fornecedor = Convert.ToString(leitorMedicamento["NOME_FORNECEDOR"]);
            string telefone = Convert.ToString(leitorMedicamento["TELEFONE"]);
            string email = Convert.ToString(leitorMedicamento["EMAIL"]);
            string cidade = Convert.ToString(leitorMedicamento["CIDADE"]);
            string estado = Convert.ToString(leitorMedicamento["ESTADO"]);

            RepositorioFornecedorEmBancoDados repositorioFornecedor = new RepositorioFornecedorEmBancoDados();

            var medicamento = new Medicamento
            {
                Id = id,
                Nome = nome_medicamento,
                Descricao = descricao,
                Lote = lote,
                Validade = validade,
                QuantidadeDisponivel = qtdDisponivel,
                Fornecedor = new Fornecedor() {
                    Id = fornecedor_id,
                    Nome = nome_fornecedor,
                    Telefone = telefone,
                    Email = email,
                    Cidade = cidade,
                    Estado = estado,
                }
            };

            return medicamento;
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
