using ControleMedicamento.Infra.BancoDados.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using ControleMedicamentos.Infra.BancoDados.Compartilhado;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ControleMedicamento.Infra.BancoDados.Tests.ModuloMedicamento
{
    [TestClass]
    public class RepositorioMedicamentoEmBancoDadosTest
    {
        public RepositorioMedicamentoEmBancoDadosTest()
        {
            string sql =
                @"DELETE FROM TBREQUISICAO;
                DBCC CHECKIDENT (TBREQUISICAO, RESEED, 0)

                DELETE FROM TBMEDICAMENTO;
                DBCC CHECKIDENT (TBMEDICAMENTO, RESEED, 0)

                DELETE FROM TBFORNECEDOR;
                DBCC CHECKIDENT (TBFORNECEDOR, RESEED, 0)";

            DatabaseConfig.ExecutarSql(sql);
        }

        [TestMethod]
        public void Deve_inserir_medicamento()
        {
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();            
            repositorioMedicamento.Inserir(novoMedicamento);

            Medicamento medicamentoEncontrado = repositorioMedicamento.SelecionarPorNumero(novoMedicamento.Id);

            Assert.IsNotNull(medicamentoEncontrado);
            Assert.AreEqual(novoMedicamento.Id, medicamentoEncontrado.Id);
            Assert.AreEqual(novoMedicamento.Nome, medicamentoEncontrado.Nome);
            Assert.AreEqual(novoMedicamento.Descricao, medicamentoEncontrado.Descricao);
            Assert.AreEqual(novoMedicamento.Lote, medicamentoEncontrado.Lote);
            Assert.AreEqual(novoMedicamento.Validade, medicamentoEncontrado.Validade);
            Assert.AreEqual(novoMedicamento.Fornecedor.Id, medicamentoEncontrado.Fornecedor.Id);
        }

        [TestMethod]
        public void Deve_editar_medicamento()
        {
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);

            Medicamento medicamentoAtualizado = repositorioMedicamento.SelecionarPorNumero(novoMedicamento.Id);
            medicamentoAtualizado.Nome = "Nome alterado";
            medicamentoAtualizado.Descricao = "Descricao alterada";
            medicamentoAtualizado.Lote = "Lote alterado";
            medicamentoAtualizado.Validade = DateTime.Now.AddDays(10).Date;
            medicamentoAtualizado.QuantidadeDisponivel = 20;
            medicamentoAtualizado.Fornecedor = repositorioFornecedor.SelecionarPorNumero(1);

            repositorioMedicamento.Editar(medicamentoAtualizado);

            Medicamento medicamentoEncontrado = repositorioMedicamento.SelecionarPorNumero(novoMedicamento.Id);

            Assert.IsNotNull(medicamentoEncontrado);
            Assert.AreEqual(medicamentoAtualizado.Id, medicamentoEncontrado.Id);
            Assert.AreEqual(medicamentoAtualizado.Nome, medicamentoEncontrado.Nome);
            Assert.AreEqual(medicamentoAtualizado.Descricao, medicamentoEncontrado.Descricao);
            Assert.AreEqual(medicamentoAtualizado.Lote, medicamentoEncontrado.Lote);
            Assert.AreEqual(medicamentoAtualizado.Validade, medicamentoEncontrado.Validade);
            Assert.AreEqual(medicamentoAtualizado.Fornecedor.Id, medicamentoEncontrado.Fornecedor.Id);
        }

        [TestMethod]
        public void Deve_excluir_medicamento()
        {
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);

            repositorioMedicamento.Excluir(novoMedicamento);

            Medicamento medicamentoEncontrado = repositorioMedicamento.SelecionarPorNumero(novoMedicamento.Id);
            Assert.IsNull(medicamentoEncontrado);
        }

        [TestMethod]
        public void Deve_selecionar_um_medicamento()
        {
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);

            Medicamento medicamentoEncontrado = repositorioMedicamento.SelecionarPorNumero(novoMedicamento.Id);

            Assert.IsNotNull(medicamentoEncontrado);
            Assert.AreEqual(novoMedicamento.Id, medicamentoEncontrado.Id);
            Assert.AreEqual(novoMedicamento.Nome, medicamentoEncontrado.Nome);
            Assert.AreEqual(novoMedicamento.Descricao, medicamentoEncontrado.Descricao);
            Assert.AreEqual(novoMedicamento.Lote, medicamentoEncontrado.Lote);
            Assert.AreEqual(novoMedicamento.Validade, medicamentoEncontrado.Validade);
            Assert.AreEqual(novoMedicamento.Fornecedor.Id, medicamentoEncontrado.Fornecedor.Id);
        }

        [TestMethod]
        public void Deve_selecionar_todos_medicamentos()
        {
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            Medicamento m1 = new Medicamento("Nome teste 1", "Descricao teste 1", "Lote teste 1", DateTime.Now.Date, 10, fornecedor);
            repositorioMedicamento.Inserir(m1);

            Medicamento m2 = new Medicamento("Nome teste 2", "Descricao teste 2", "Lote teste 2", DateTime.Now.Date, 10, fornecedor);
            repositorioMedicamento.Inserir(m2);

            Medicamento m3 = new Medicamento("Nome teste 3", "Descricao teste 3", "Lote teste 3", DateTime.Now.Date, 10, fornecedor);
            repositorioMedicamento.Inserir(m3);

            var medicamentos = repositorioMedicamento.SelecionarTodos();

            Assert.AreEqual(3, medicamentos.Count);

            Assert.AreEqual("Nome teste 1", medicamentos[0].Nome);
            Assert.AreEqual("Nome teste 2", medicamentos[1].Nome);
            Assert.AreEqual("Nome teste 3", medicamentos[2].Nome);
        }

    }
}
