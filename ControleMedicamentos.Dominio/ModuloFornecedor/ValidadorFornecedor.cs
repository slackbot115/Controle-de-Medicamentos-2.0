using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Dominio.ModuloFornecedor
{
    public class ValidadorFornecedor : AbstractValidator<Fornecedor>
    {
        public ValidadorFornecedor()
        {
            RuleFor(x => x.Nome)
                .NotNull().NotEmpty();

            RuleFor(x => x.Telefone)
                .NotNull().NotEmpty();

            RuleFor(x => x.Email)
                .NotNull().NotEmpty();

            RuleFor(x => x.Cidade)
                .NotNull().NotEmpty();

            RuleFor(x => x.Estado)
                .NotNull().NotEmpty();
        }
    }
}
