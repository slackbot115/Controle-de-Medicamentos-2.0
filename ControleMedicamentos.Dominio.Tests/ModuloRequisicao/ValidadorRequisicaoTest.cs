using ControleMedicamentos.Dominio.ModuloRequisicao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Dominio.Tests.ModuloRequisicao
{
    [TestClass]
    public class ValidadorRequisicaoTest
    {
        public ValidadorRequisicaoTest()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
        }

        [TestMethod]
        public void QuantidadeMedicamento_deve_ser_maior_que_0()
        {
            var r = new Requisicao();
            r.QtdMedicamento = -1;

            ValidadorRequisicao validador = new ValidadorRequisicao();

            var resultado = validador.Validate(r);

            Assert.AreEqual("Quantidade deve ser maior que 0", resultado.Errors[0].ErrorMessage);
        }
    }
}
