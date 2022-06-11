using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Infra.BancoDados.Compartilhado;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloFornecedor
{
    [TestClass]
    public class RepositorioFornecedorEmBancoDadosTest
    {
        public RepositorioFornecedorEmBancoDadosTest()
        {
            string sql =
                @"DELETE FROM TBFORNECEDOR;
                  DBCC CHECKIDENT (TBFORNECEDOR, RESEED, 0)";

            DatabaseConfig.ExecutarSql(sql);
        }

        [TestMethod]
        public void Deve_inserir_fornecedor()
        {
            Fornecedor novoFornecedor = new Fornecedor(
                "Nome teste", 
                "Telefone teste",
                "Email teste",
                "Cidade teste",
                "Estado teste"
                );

            var repositorio = new RepositorioFornecedorEmBancoDados();

            repositorio.Inserir(novoFornecedor);

            Fornecedor fornecedorEncontrado = repositorio.SelecionarPorNumero(novoFornecedor.Id);

            Assert.IsNotNull(fornecedorEncontrado);
            Assert.AreEqual(novoFornecedor.Id, fornecedorEncontrado.Id);
            Assert.AreEqual(novoFornecedor.Nome, fornecedorEncontrado.Nome);
            Assert.AreEqual(novoFornecedor.Telefone, fornecedorEncontrado.Telefone);
            Assert.AreEqual(novoFornecedor.Email, fornecedorEncontrado.Email);
            Assert.AreEqual(novoFornecedor.Cidade, fornecedorEncontrado.Cidade);
            Assert.AreEqual(novoFornecedor.Estado, fornecedorEncontrado.Estado);
        }

        [TestMethod]
        public void Deve_editar_fornecedor()
        {
            Fornecedor novoFornecedor = new Fornecedor(
                "Nome teste",
                "Telefone teste",
                "Email teste",
                "Cidade teste",
                "Estado teste"
                );

            var repositorio = new RepositorioFornecedorEmBancoDados();

            repositorio.Inserir(novoFornecedor);

            Fornecedor fornecedorAtualizado = repositorio.SelecionarPorNumero(novoFornecedor.Id);
            fornecedorAtualizado.Nome = "Nome alterado";
            fornecedorAtualizado.Telefone = "Telefone alterado";
            fornecedorAtualizado.Email = "Email alterado";
            fornecedorAtualizado.Cidade = "Cidade alterada";
            fornecedorAtualizado.Estado = "Estado alterado";

            repositorio.Editar(fornecedorAtualizado);

            Fornecedor fornecedorEncontrado = repositorio.SelecionarPorNumero(novoFornecedor.Id);

            Assert.IsNotNull(fornecedorEncontrado);
            Assert.AreEqual(fornecedorAtualizado.Id, fornecedorEncontrado.Id);
            Assert.AreEqual(fornecedorAtualizado.Nome, fornecedorEncontrado.Nome);
            Assert.AreEqual(fornecedorAtualizado.Telefone, fornecedorEncontrado.Telefone);
            Assert.AreEqual(fornecedorAtualizado.Email, fornecedorEncontrado.Email);
            Assert.AreEqual(fornecedorAtualizado.Cidade, fornecedorEncontrado.Cidade);
            Assert.AreEqual(fornecedorAtualizado.Estado, fornecedorEncontrado.Estado);
        }

        [TestMethod]
        public void Deve_excluir_fornecedor()
        {
            Fornecedor novoFornecedor = new Fornecedor(
                "Nome teste",
                "Telefone teste",
                "Email teste",
                "Cidade teste",
                "Estado teste"
                );

            var repositorio = new RepositorioFornecedorEmBancoDados();

            repositorio.Inserir(novoFornecedor);

            repositorio.Excluir(novoFornecedor);

            Fornecedor fornecedorEncontrado = repositorio.SelecionarPorNumero(novoFornecedor.Id);

            Assert.IsNull(fornecedorEncontrado);
        }

        [TestMethod]
        public void Deve_selecionar_um_fornecedor()
        {
            Fornecedor novoFornecedor = new Fornecedor(
                "Nome teste",
                "Telefone teste",
                "Email teste",
                "Cidade teste",
                "Estado teste"
                );

            var repositorio = new RepositorioFornecedorEmBancoDados();

            repositorio.Inserir(novoFornecedor);

            Fornecedor fornecedorEncontrado = repositorio.SelecionarPorNumero(novoFornecedor.Id);

            Assert.IsNotNull(fornecedorEncontrado);
            Assert.AreEqual(novoFornecedor.Id, fornecedorEncontrado.Id);
            Assert.AreEqual(novoFornecedor.Nome, fornecedorEncontrado.Nome);
            Assert.AreEqual(novoFornecedor.Telefone, fornecedorEncontrado.Telefone);
            Assert.AreEqual(novoFornecedor.Email, fornecedorEncontrado.Email);
            Assert.AreEqual(novoFornecedor.Cidade, fornecedorEncontrado.Cidade);
            Assert.AreEqual(novoFornecedor.Estado, fornecedorEncontrado.Estado);
        }

        [TestMethod]
        public void Deve_selecionar_todos_fornecedores()
        {
            var repositorio = new RepositorioFornecedorEmBancoDados();

            Fornecedor f1 = new Fornecedor(
                "Nome teste 1",
                "Telefone teste 1",
                "Email teste 1",
                "Cidade teste 1",
                "Estado teste 1"
                );
            repositorio.Inserir(f1);

            Fornecedor f2 = new Fornecedor(
                "Nome teste 2",
                "Telefone teste 2",
                "Email teste 2",
                "Cidade teste 2",
                "Estado teste 2"
                );
            repositorio.Inserir(f2);

            Fornecedor f3 = new Fornecedor(
                "Nome teste 3",
                "Telefone teste 3",
                "Email teste 3",
                "Cidade teste 3",
                "Estado teste 3"
                );
            repositorio.Inserir(f3);

            var fornecedores = repositorio.SelecionarTodos();

            Assert.AreEqual(3, fornecedores.Count);

            Assert.AreEqual("Nome teste 1", fornecedores[0].Nome);
            Assert.AreEqual("Nome teste 2", fornecedores[1].Nome);
            Assert.AreEqual("Nome teste 3", fornecedores[2].Nome);
        }

    }
}
