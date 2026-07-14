using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eTamanhoRefeicao { Codigo, Nome }

    public class TamanhoRefeicao : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 20;
        public const int TamCodigo = 1;

        public TamanhoRefeicao() { }

        public string Codigo { get; set; }
        [Display(Name = "Tamanho")]
        public string Nome { get; set; }

        public byte Posicao { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();
            
            if (string.IsNullOrEmpty(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eTamanhoRefeicao.Nome.ToString(), Nome)));

            return retorno;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
