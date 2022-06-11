using ControleMedicamentos.Dominio.ModuloFuncionario;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Dominio.Tests.ModuloFuncionario
{
    [TestClass]
    public class ValidadorFuncionarioTest
    {
        public ValidadorFuncionarioTest()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
        }

        [TestMethod]
        public void Nome_do_funcionario_deve_ser_obrigatorio()
        {
            var f = new Funcionario();
            f.Nome = null;

            ValidadorFuncionario validador = new ValidadorFuncionario();

            var resultado = validador.Validate(f);

            Assert.AreEqual("'Nome' não pode ser nulo.", resultado.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Login_do_funcionario_deve_ser_obrigatorio()
        {
            var f = new Funcionario();
            f.Nome = "Nome teste";
            f.Login = null;

            ValidadorFuncionario validador = new ValidadorFuncionario();

            var resultado = validador.Validate(f);

            Assert.AreEqual("'Login' não pode ser nulo.", resultado.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Senha_do_funcionario_deve_ser_obrigatorio()
        {
            var f = new Funcionario();
            f.Nome = "Nome teste";
            f.Login = "Login teste";
            f.Senha = null;

            ValidadorFuncionario validador = new ValidadorFuncionario();

            var resultado = validador.Validate(f);

            Assert.AreEqual("'Senha' não pode ser nulo.", resultado.Errors[0].ErrorMessage);
        }

    }
}
