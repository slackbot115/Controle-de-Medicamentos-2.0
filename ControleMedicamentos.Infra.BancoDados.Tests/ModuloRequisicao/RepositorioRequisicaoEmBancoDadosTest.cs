using ControleMedicamento.Infra.BancoDados.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloFuncionario;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloPaciente;
using ControleMedicamentos.Dominio.ModuloRequisicao;
using ControleMedicamentos.Infra.BancoDados.Compartilhado;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using ControleMedicamentos.Infra.BancoDados.ModuloFuncionario;
using ControleMedicamentos.Infra.BancoDados.ModuloPaciente;
using ControleMedicamentos.Infra.BancoDados.ModuloRequisicao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloRequisicao
{
    [TestClass]
    public class RepositorioRequisicaoEmBancoDadosTest
    {
        public RepositorioRequisicaoEmBancoDadosTest()
        {
            string sql =
                @"DELETE FROM TBREQUISICAO;
                  DBCC CHECKIDENT (TBREQUISICAO, RESEED, 0)

                  DELETE FROM TBMEDICAMENTO;
                  DBCC CHECKIDENT (TBMEDICAMENTO, RESEED, 0)

                  DELETE FROM TBFUNCIONARIO;
                  DBCC CHECKIDENT (TBFUNCIONARIO, RESEED, 0)

                  DELETE FROM TBPACIENTE;
                  DBCC CHECKIDENT (TBPACIENTE, RESEED, 0)
                ";

            DatabaseConfig.ExecutarSql(sql);
        }

        [TestMethod]
        public void Deve_inserir_requisicao()
        {
            #region Dependencia para criar requisicao
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
            repositorioFuncionario.Inserir(novoFuncionario);

            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorioPaciente = new RepositorioPacienteEmBancoDados();
            repositorioPaciente.Inserir(novoPaciente);

            //Necessário para o medicamento
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento); 
            #endregion

            Requisicao novaRequisicao = new Requisicao(novoMedicamento, novoPaciente, 5, novoFuncionario);
            var repositorioRequisicao = new RepositorioRequisicaoEmBancoDados();
            repositorioRequisicao.Inserir(novaRequisicao);

            Requisicao requisicaoEncontrada = repositorioRequisicao.SelecionarPorNumero(novaRequisicao.Id);

            Assert.IsNotNull(requisicaoEncontrada);
            Assert.AreEqual(novaRequisicao.Id, requisicaoEncontrada.Id);
            Assert.AreEqual(novaRequisicao.Funcionario.Id, requisicaoEncontrada.Funcionario.Id);
            Assert.AreEqual(novaRequisicao.Paciente.Id, requisicaoEncontrada.Paciente.Id);
            Assert.AreEqual(novaRequisicao.Medicamento.Id, requisicaoEncontrada.Medicamento.Id);
            Assert.AreEqual(novaRequisicao.QtdMedicamento, requisicaoEncontrada.QtdMedicamento);
            Assert.AreEqual(novaRequisicao.Data.Date, requisicaoEncontrada.Data.Date);
        }

        [TestMethod]
        public void Deve_editar_requisicao()
        {
            #region Dependencia para criar requisicao
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
            repositorioFuncionario.Inserir(novoFuncionario);

            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorioPaciente = new RepositorioPacienteEmBancoDados();
            repositorioPaciente.Inserir(novoPaciente);

            //Necessário para o medicamento
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);
            #endregion

            Requisicao novaRequisicao = new Requisicao(novoMedicamento, novoPaciente, 5, novoFuncionario);
            var repositorioRequisicao = new RepositorioRequisicaoEmBancoDados();
            repositorioRequisicao.Inserir(novaRequisicao);

            Requisicao requisicaoAtualizada = repositorioRequisicao.SelecionarPorNumero(novaRequisicao.Id);
            requisicaoAtualizada.QtdMedicamento = 7;
            repositorioRequisicao.Editar(requisicaoAtualizada);

            Requisicao requisicaoEncontrada = repositorioRequisicao.SelecionarPorNumero(novaRequisicao.Id);

            Assert.IsNotNull(requisicaoEncontrada);
            Assert.AreEqual(requisicaoAtualizada.Id, requisicaoEncontrada.Id);
            Assert.AreEqual(requisicaoAtualizada.Funcionario.Id, requisicaoEncontrada.Funcionario.Id);
            Assert.AreEqual(requisicaoAtualizada.Paciente.Id, requisicaoEncontrada.Paciente.Id);
            Assert.AreEqual(requisicaoAtualizada.Medicamento.Id, requisicaoEncontrada.Medicamento.Id);
            Assert.AreEqual(requisicaoAtualizada.QtdMedicamento, requisicaoEncontrada.QtdMedicamento);
            Assert.AreEqual(requisicaoAtualizada.Data.Date, requisicaoEncontrada.Data.Date);
        }

        [TestMethod]
        public void Deve_excluir_requisicao()
        {
            #region Dependencia para criar requisicao
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
            repositorioFuncionario.Inserir(novoFuncionario);

            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorioPaciente = new RepositorioPacienteEmBancoDados();
            repositorioPaciente.Inserir(novoPaciente);

            //Necessário para o medicamento
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);
            #endregion

            Requisicao novaRequisicao = new Requisicao(novoMedicamento, novoPaciente, 5, novoFuncionario);
            var repositorioRequisicao = new RepositorioRequisicaoEmBancoDados();
            repositorioRequisicao.Inserir(novaRequisicao);

            repositorioRequisicao.Excluir(novaRequisicao);

            Requisicao requisicaoEncontrada = repositorioRequisicao.SelecionarPorNumero(novaRequisicao.Id);
            Assert.IsNull(requisicaoEncontrada);
        }

        [TestMethod]
        public void Deve_selecionar_uma_requisicao()
        {
            #region Dependencia para criar requisicao
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
            repositorioFuncionario.Inserir(novoFuncionario);

            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorioPaciente = new RepositorioPacienteEmBancoDados();
            repositorioPaciente.Inserir(novoPaciente);

            //Necessário para o medicamento
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);
            #endregion

            Requisicao novaRequisicao = new Requisicao(novoMedicamento, novoPaciente, 5, novoFuncionario);
            var repositorioRequisicao = new RepositorioRequisicaoEmBancoDados();
            repositorioRequisicao.Inserir(novaRequisicao);

            Requisicao requisicaoEncontrada = repositorioRequisicao.SelecionarPorNumero(novaRequisicao.Id);

            Assert.IsNotNull(requisicaoEncontrada);
            Assert.AreEqual(novaRequisicao.Id, requisicaoEncontrada.Id);
            Assert.AreEqual(novaRequisicao.Funcionario.Id, requisicaoEncontrada.Funcionario.Id);
            Assert.AreEqual(novaRequisicao.Paciente.Id, requisicaoEncontrada.Paciente.Id);
            Assert.AreEqual(novaRequisicao.Medicamento.Id, requisicaoEncontrada.Medicamento.Id);
            Assert.AreEqual(novaRequisicao.QtdMedicamento, requisicaoEncontrada.QtdMedicamento);
            Assert.AreEqual(novaRequisicao.Data.Date, requisicaoEncontrada.Data.Date);
        }

        [TestMethod]
        public void Deve_selecionar_todas_requisicoes()
        {
            #region Dependencia para criar requisicao
            Funcionario novoFuncionario = new Funcionario("Nome teste", "Login teste", "Senha teste");
            var repositorioFuncionario = new RepositorioFuncionarioEmBancoDados();
            repositorioFuncionario.Inserir(novoFuncionario);

            Paciente novoPaciente = new Paciente("Nome teste", "CartaoTeste");
            var repositorioPaciente = new RepositorioPacienteEmBancoDados();
            repositorioPaciente.Inserir(novoPaciente);

            //Necessário para o medicamento
            var fornecedor = new Fornecedor("Nome teste", "Telefone teste", "Email teste", "Cidade teste", "Estado teste");
            var repositorioFornecedor = new RepositorioFornecedorEmBancoDados();
            repositorioFornecedor.Inserir(fornecedor);

            Medicamento novoMedicamento = new Medicamento("Nome teste", "Descricao teste", "Lote teste", DateTime.Now.Date, 10, fornecedor);
            var repositorioMedicamento = new RepositorioMedicamentoEmBancoDados();
            repositorioMedicamento.Inserir(novoMedicamento);
            #endregion

            var repositorioRequisicao = new RepositorioRequisicaoEmBancoDados();

            Requisicao r1 = new Requisicao(novoMedicamento, novoPaciente, 5, novoFuncionario);
            repositorioRequisicao.Inserir(r1);

            Requisicao r2 = new Requisicao(novoMedicamento, novoPaciente, 6, novoFuncionario);
            repositorioRequisicao.Inserir(r2);

            Requisicao r3 = new Requisicao(novoMedicamento, novoPaciente, 7, novoFuncionario);
            repositorioRequisicao.Inserir(r3);

            var requisicoes = repositorioRequisicao.SelecionarTodos();

            Assert.AreEqual(3, requisicoes.Count);

            Assert.AreEqual(r1.Data.Date, requisicoes[0].Data.Date);
            Assert.AreEqual(r2.Data.Date, requisicoes[1].Data.Date);
            Assert.AreEqual(r3.Data.Date, requisicoes[2].Data.Date);
        }

    }
}
