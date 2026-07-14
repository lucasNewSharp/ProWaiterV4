using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public class RefeicaoDoPedido : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxObservacoes = int.MaxValue;

        public int Codigo { get; set; }
        public int CodPedido { get; set; }

        public short CodRefeicao { get; set; }
        public string CodTamanho { get; set; }
        public virtual TamanhoRefeicao Tamanho { get; set; }
        public virtual RefeicaoDoCardapio RefeicaoDoCardapio { get; set; }

        public virtual ICollection<ComponenteRefeicaoPedido> ComponentesRefeicaoPedido { get; set; }
        public bool Enviado { get; set; }

        public string Observacoes { get; set; }
        public decimal Valor { get; set; }
        public decimal Acrescimo { get; set; }

        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }
        public DateTime? DataHora { get; set; }
        public decimal PercDesconto { get; set; }

        public RefeicaoDoPedido()
        {
            ComponentesRefeicaoPedido = new List<ComponenteRefeicaoPedido>();
        }

        public RefeicaoDoPedido(Pedido pedido, RefeicaoDoCardapio refeicaoDoCardapio, List<ComponenteRefeicaoPedido> componentes) : this()
        {
            if (pedido == null)
                throw new ArgumentNullException("pedido");
            CodPedido = pedido.Codigo;
            RefeicaoDoCardapio = refeicaoDoCardapio ?? throw new ArgumentNullException("refeicaoDoCardapio");
            ComponentesRefeicaoPedido = componentes;
            PercDesconto = refeicaoDoCardapio.PercDesconto;            

            foreach (ComponenteRefeicaoPedido c in ComponentesRefeicaoPedido)
                c.RefeicaoDoPedido = this;

            RecalcularValorRefeicao();
        }

        public void RecalcularValorRefeicao()
        {
            PercDesconto = RefeicaoDoCardapio.PercDesconto;
            if (!RefeicaoDoCardapio.DeComposicao)
            {
                Valor = RefeicaoDoCardapio.Valor * ObterFatorDesconto();
                return;
            }

            Valor = 0;            

            int qtdProporcional = 0;
            foreach (var comp in ComponentesRefeicaoPedido)
            {
                var compComposicao = RefeicaoDoCardapio.ComponentesComposicaoRefeicao.Where(c => c.CodRefeicao == RefeicaoDoCardapio.CodRefeicao && c.CodTamanho == RefeicaoDoCardapio.CodTamanho && c.CodComponente == comp.CodComponente).Single();
                if (compComposicao.CalculoProporcional)
                    qtdProporcional += comp.Quantidade;                    
            }

            foreach (var comp in ComponentesRefeicaoPedido)
            {
                var compComposicao = RefeicaoDoCardapio.ComponentesComposicaoRefeicao.Where(c => c.CodRefeicao == RefeicaoDoCardapio.CodRefeicao && c.CodTamanho == RefeicaoDoCardapio.CodTamanho && c.CodComponente == comp.CodComponente).Single();
                if (compComposicao.CalculoProporcional)
                    Valor += (comp.Quantidade * compComposicao.Valor) / qtdProporcional;                    
                else
                    Valor += compComposicao.Valor * comp.Quantidade;
            }

            Valor *= ObterFatorDesconto();
        }

        private decimal ObterFatorDesconto()
        {
            return PercDesconto == 0 ? 1 : ((100 - PercDesconto) / 100);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (Valor < 0)
                validacoes.Add(new ValidationResult("O valor do pedido (" + Codigo.ToString() + " devem ser maior ou igual a zero!"));

            return validacoes;
        }
    }
}
