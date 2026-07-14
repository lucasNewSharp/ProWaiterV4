using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eCidade { Codigo, Nome, CodUF }

    public class Cidade : IValidatableObject, IEntidadeBD
    {
        public static int TamMaxNome = 32;

        public Cidade() { }
        public int Codigo { get; private set; }
        [Display(Name = "Cidade")]
        public string Nome { get; set; }
        public string CodUF { get; private set; }
        public virtual UF UF { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Nome, CodUF);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eCidade.Nome.ToString(), Nome)));

            if (UF == null)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), "UF", null)));

            return validacoes;
        }
    }
}
