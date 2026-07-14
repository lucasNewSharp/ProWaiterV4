using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ItemBalcao : IEntidadeBD, IValidatableObject, IItemCodigoBarras
    {
        public const int TamMaxNome = 100;
        public const int TamMaxCodBarras = 50;
        
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(maximumLength: TamMaxNome)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Valor { get; set; }

        [Display(Name = "Perc. de desconto (%)")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal PercDesconto { get; set; }

        [StringLength(maximumLength: TamMaxCodBarras)]
        [Display(Name = "Código de Barras")]
        public string CodBarras { get; set; }

        public bool Ativo { get; set; }

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