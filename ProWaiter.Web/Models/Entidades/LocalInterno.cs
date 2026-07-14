using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eLocalInterno { Codigo, Nome }

    public class LocalInterno : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 20;

        public LocalInterno() { }

        [Display(Name = "Código")]
        public short Codigo { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eLocalInterno.Nome.ToString(), Nome)));

            return retorno;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}