using ControleMedicamentos.Dominio.ModuloPaciente;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Dominio.Tests.ModuloPaciente
{
    [TestClass]
    public class ValidadorPacienteTest
    {
        public ValidadorPacienteTest()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
        }

        [TestMethod]
        public void Nome_do_paciente_deve_ser_obrigatorio()
        {
            var p = new Paciente();
            p.Nome = null;

            ValidadorPaciente validador = new ValidadorPaciente();

            var resultado = validador.Validate(p);

            Assert.AreEqual("'Nome' não pode ser nulo.", resultado.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void CartaoSUS_do_paciente_deve_ser_obrigatorio()
        {
            var p = new Paciente();
            p.Nome = "Nome teste";
            p.CartaoSUS = null;

            ValidadorPaciente validador = new ValidadorPaciente();

            var resultado = validador.Validate(p);

            Assert.AreEqual("'Cartao SUS' não pode ser nulo.", resultado.Errors[0].ErrorMessage);
        }

    }
}
