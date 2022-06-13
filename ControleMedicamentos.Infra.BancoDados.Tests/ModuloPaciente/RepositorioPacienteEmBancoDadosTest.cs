using ControleMedicamentos.Dominio.ModuloPaciente;
using ControleMedicamentos.Infra.BancoDados.Compartilhado;
using ControleMedicamentos.Infra.BancoDados.ModuloPaciente;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloPaciente
{
    [TestClass]
    public class RepositorioPacienteEmBancoDadosTest
    {
        public RepositorioPacienteEmBancoDadosTest()
        {
            string sql =
                @"DELETE FROM TBREQUISICAO;
                  DBCC CHECKIDENT (TBREQUISICAO, RESEED, 0)

                  DELETE FROM TBPACIENTE;
                  DBCC CHECKIDENT (TBPACIENTE, RESEED, 0)";

            DatabaseConfig.ExecutarSql(sql);
        }

        [TestMethod]
        public void Deve_inserir_paciente()
        {
            //arrange
            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");

            var repositorio = new RepositorioPacienteEmBancoDados();

            //action
            repositorio.Inserir(novoPaciente);

            //assert
            Paciente pacienteEncontrado = repositorio.SelecionarPorNumero(novoPaciente.Id);

            Assert.IsNotNull(pacienteEncontrado);
            Assert.AreEqual(novoPaciente.Id, pacienteEncontrado.Id);
            Assert.AreEqual(novoPaciente.Nome, pacienteEncontrado.Nome);
            Assert.AreEqual(novoPaciente.CartaoSUS, pacienteEncontrado.CartaoSUS);
        }

        [TestMethod]
        public void Deve_editar_paciente()
        {
            //arrange
            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorio = new RepositorioPacienteEmBancoDados();
            repositorio.Inserir(novoPaciente);

            Paciente pacienteAtualizado = repositorio.SelecionarPorNumero(novoPaciente.Id);
            pacienteAtualizado.Nome = "Nome teste 2";
            pacienteAtualizado.CartaoSUS = "CartaoTeste2";

            //action
            repositorio.Editar(pacienteAtualizado);

            //assert
            Paciente pacienteEncontrado = repositorio.SelecionarPorNumero(novoPaciente.Id);

            Assert.IsNotNull(pacienteEncontrado);
            Assert.AreEqual(pacienteAtualizado.Id, pacienteEncontrado.Id);
            Assert.AreEqual(pacienteAtualizado.Nome, pacienteEncontrado.Nome);
            Assert.AreEqual(pacienteAtualizado.CartaoSUS, pacienteEncontrado.CartaoSUS);
        }

        [TestMethod]
        public void Deve_excluir_paciente()
        {
            //arrange
            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");

            var repositorio = new RepositorioPacienteEmBancoDados();

            repositorio.Inserir(novoPaciente);

            //action
            repositorio.Excluir(novoPaciente);

            //assert
            Paciente tarefaEncontrada = repositorio.SelecionarPorNumero(novoPaciente.Id);

            Assert.IsNull(tarefaEncontrada);
        }

        [TestMethod]
        public void Deve_selecionar_um_paciente()
        {
            //arrange
            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");

            var repositorio = new RepositorioPacienteEmBancoDados();

            repositorio.Inserir(novoPaciente);

            //action
            Paciente pacienteEncontrado = repositorio.SelecionarPorNumero(novoPaciente.Id);

            //assert
            Assert.IsNotNull(pacienteEncontrado);
            Assert.AreEqual(novoPaciente.Id, pacienteEncontrado.Id);
            Assert.AreEqual(novoPaciente.Nome, pacienteEncontrado.Nome);
            Assert.AreEqual(novoPaciente.CartaoSUS, pacienteEncontrado.CartaoSUS);
        }

        [TestMethod]
        public void Deve_selecionar_todos_pacientes()
        {
            var repositorio = new RepositorioPacienteEmBancoDados();

            Paciente p1 = new Paciente("Nome teste", "CartaoTeste");
            repositorio.Inserir(p1);

            Paciente p2 = new Paciente("Nome teste 2", "CartaoTeste2");
            repositorio.Inserir(p2);

            Paciente p3 = new Paciente("Nome teste 3", "CartaoTeste3");
            repositorio.Inserir(p3);

            //action
            var pacientes = repositorio.SelecionarTodos();

            //assert
            Assert.AreEqual(3, pacientes.Count);

            Assert.AreEqual("Nome teste", pacientes[0].Nome);
            Assert.AreEqual("Nome teste 2", pacientes[1].Nome);
            Assert.AreEqual("Nome teste 3", pacientes[2].Nome);
        }

    }
}
