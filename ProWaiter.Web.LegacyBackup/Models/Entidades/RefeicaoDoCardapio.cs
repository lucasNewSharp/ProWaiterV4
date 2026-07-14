using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public class RefeicaoDoCardapio : IEntidadeBD, IValidatableObject, IItemCodigoBarras
    {
        public const int TamMaxCodBarras = 100;

        public short CodRefeicao { get; set; }
        public virtual Refeicao Refeicao { get; set; }
        public string CodTamanho { get; set; }
        public virtual TamanhoRefeicao TamanhoRefeicao { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public byte CodImpressora { get; set; }
        public virtual Impressora Impressora { get; set; }
        [Display(Name = "Perc. de desconto (%)")]
        public decimal PercDesconto { get; set; }

        [Display(Name = "Código de barras")]
        public string CodBarras { get; set; }

        [Display(Name = "De composição")]
        public bool DeComposicao { get; set; }

        [Display(Name = "Componentes para composição")]
        public virtual ICollection<ComponenteComposicaoRefeicaoCardapio> ComponentesComposicaoRefeicao { get; set; }


        public string Nome { get => Refeicao.Nome; }

        public RefeicaoDoCardapio() { ComponentesComposicaoRefeicao = new List<ComponenteComposicaoRefeicaoCardapio>();  }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (Valor < 0)
                retorno.Add(new ValidationResult(this.ObterMensagemErro($"{CodRefeicao} - {CodTamanho}", nameof(Valor), Valor)));
            if (PercDesconto > 100 || PercDesconto < 0)
                retorno.Add(new ValidationResult($"Não é possível setar o percentual de desconto com valor acima de 100%"));
            if (!string.IsNullOrWhiteSpace(CodBarras) && CodBarras.Length > TamMaxCodBarras)
                retorno.Add(new ValidationResult(this.ObterMensagemErro($"{CodRefeicao} - {CodTamanho}", nameof(CodBarras), CodBarras)));

            return retorno;
        }

    }
}
