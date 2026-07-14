using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eComponenteRefeicao { Codigo, Nome }

    public class ComponenteRefeicao : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 100;

        public ComponenteRefeicao() { }
        public ComponenteRefeicao(string nome)
        {
            this.Nome = nome.Trim();
        }

        public short Codigo { get; set; }
        public string Nome { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eComponenteRefeicao.Nome.ToString(), Nome)));

            return retorno;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
