using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProWaiter.Web.Models.Entidades
{
    public class Bebida : IEntidadeBD, IValidatableObject, IItemCodigoBarras
    {
        public const int TamMaxNome = 100;
        public const int TamMaxCodBarras = 50;

        public Bebida() { }        

        public short Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }

        public bool Ativo { get; set; }

        public short CodTipo { get; set; }

        public virtual TipoBebida Tipo { get; set; }

        public byte CodImpressora { get; set; }
        public virtual Impressora Impressora { get; set; }

        [Display(Name = "Código de barras")]
        public string CodBarras { get; set; }

        [Display(Name = "Perc. de desconto (%)")]
        public decimal PercDesconto { get; set; }

        public override string ToString()
        {
            return Nome;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(Nome), Nome)));
            if (Valor < 0)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(Valor), Valor)));
            if (PercDesconto > 100 || PercDesconto < 0)
                validacoes.Add(new ValidationResult($"Não é possível setar o percentual de desconto com valor acima de 100%"));
            if (!string.IsNullOrWhiteSpace(CodBarras) && CodBarras.Length > TamMaxCodBarras)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(CodBarras), CodBarras)));

            return validacoes;
        }
    }
}
