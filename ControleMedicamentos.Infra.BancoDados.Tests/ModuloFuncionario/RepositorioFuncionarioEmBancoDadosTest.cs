using ControleMedicamentos.Dominio.ModuloFuncionario;
using ControleMedicamentos.Infra.BancoDados.Compartilhado;
using ControleMedicamentos.Infra.BancoDados.ModuloFuncionario;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloFuncionario
{
    [TestClass]
    public class RepositorioFuncionarioEmBancoDadosTest
    {
        public RepositorioFuncionarioEmBancoDadosTest()
        {
            string sql =
                @"DELETE FROM TBFUNCIONARIO;
                  DBCC CHECKIDENT (TBFUNCIONARIO, RESEED, 0)";

            DatabaseConfig.ExecutarSql(sql);
        }

        [TestMethod]
        public void Deve_inserir_funcionario()
        {
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");

            var repositorio = new RepositorioFuncionarioEmBancoDados();

            repositorio.Inserir(novoFuncionario);

            Funcionario funcionarioEncontrado = repositorio.SelecionarPorNumero(novoFuncionario.Id);

            Assert.IsNotNull(funcionarioEncontrado);
            Assert.AreEqual(novoFuncionario.Id, funcionarioEncontrado.Id);
            Assert.AreEqual(novoFuncionario.Nome, funcionarioEncontrado.Nome);
            Assert.AreEqual(novoFuncionario.Login, funcionarioEncontrado.Login);
            Assert.AreEqual(novoFuncionario.Senha, funcionarioEncontrado.Senha);
        }

        [TestMethod]
        public void Deve_editar_funcionario()
        {
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorio = new RepositorioFuncionarioEmBancoDados();
            repositorio.Inserir(novoFuncionario);

            Funcionario funcionarioAtualizado = repositorio.SelecionarPorNumero(novoFuncionario.Id);
            funcionarioAtualizado.Nome = "Nome alterado";
            funcionarioAtualizado.Login = "Login alterado";
            funcionarioAtualizado.Senha = "Senha alterada";

            repositorio.Editar(funcionarioAtualizado);

            Funcionario funcionarioEncontrado = repositorio.SelecionarPorNumero(novoFuncionario.Id);

            Assert.IsNotNull(funcionarioEncontrado);
            Assert.AreEqual(funcionarioAtualizado.Id, funcionarioEncontrado.Id);
            Assert.AreEqual(funcionarioAtualizado.Nome, funcionarioEncontrado.Nome);
            Assert.AreEqual(funcionarioAtualizado.Login, funcionarioEncontrado.Login);
            Assert.AreEqual(funcionarioAtualizado.Senha, funcionarioEncontrado.Senha);
        }

        [TestMethod]
        public void Deve_excluir_funcionario()
        {
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorio = new RepositorioFuncionarioEmBancoDados();
            repositorio.Inserir(novoFuncionario);

            repositorio.Excluir(novoFuncionario);

            Funcionario funcionarioEncontrado = repositorio.SelecionarPorNumero(novoFuncionario.Id);
            Assert.IsNull(funcionarioEncontrado);
        }

        [TestMethod]
        public void Deve_selecionar_um_funcionario()
        {
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorio = new RepositorioFuncionarioEmBancoDados();
            repositorio.Inserir(novoFuncionario);

            Funcionario funcionarioEncontrado = repositorio.SelecionarPorNumero(novoFuncionario.Id);

            Assert.IsNotNull(funcionarioEncontrado);
            Assert.AreEqual(novoFuncionario.Id, funcionarioEncontrado.Id);
            Assert.AreEqual(novoFuncionario.Nome, funcionarioEncontrado.Nome);
            Assert.AreEqual(novoFuncionario.Login, funcionarioEncontrado.Login);
            Assert.AreEqual(novoFuncionario.Senha, funcionarioEncontrado.Senha);
        }

        [TestMethod]
        public void Deve_selecionar_todos_funcionarios()
        {
            var repositorio = new RepositorioFuncionarioEmBancoDados();

            Funcionario f1 = new Funcionario("Nome teste 1", "Login teste 1", "Senha teste 1");
            repositorio.Inserir(f1);

            Funcionario f2 = new Funcionario("Nome teste 2", "Login teste 2", "Senha teste 2");
            repositorio.Inserir(f2);

            Funcionario f3 = new Funcionario("Nome teste 3", "Login teste 3", "Senha teste 3");
            repositorio.Inserir(f3);

            var funcionarios = repositorio.SelecionarTodos();

            Assert.AreEqual(3, funcionarios.Count);

            Assert.AreEqual("Nome teste 1", funcionarios[0].Nome);
            Assert.AreEqual("Nome teste 2", funcionarios[1].Nome);
            Assert.AreEqual("Nome teste 3", funcionarios[2].Nome);
        }

    }
}
