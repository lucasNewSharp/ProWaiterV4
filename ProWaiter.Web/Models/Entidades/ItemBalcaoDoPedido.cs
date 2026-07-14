using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ItemBalcaoDoPedido : IEntidadeBD, IValidatableObject
    {
        public ItemBalcaoDoPedido() { }

        public ItemBalcaoDoPedido(Pedido pedido, ItemBalcao itemBalcao)
        {
            if (pedido == null)
                throw new ArgumentNullException("pedido");
            CodPedido = pedido.Codigo;
            ItemBalcao = itemBalcao ?? throw new ArgumentNullException("itemBalcao");
            AplicarDesconto();
        }

        public int Codigo { get; set; }
        public int CodPedido { get; set; }

        public int CodItemBalcao { get; set; }
        public virtual ItemBalcao ItemBalcao { get; set; }

        public decimal Valor { get; set; }
        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }
        public DateTime DataHora { get; set; }
        public decimal PercDesconto { get; set; }

        public void AplicarDesconto()
        {
            PercDesconto = ItemBalcao.PercDesconto;
            Valor = ItemBalcao.PercDesconto > 0 ? (ItemBalcao.Valor * ((100 - ItemBalcao.PercDesconto) / 100)) : ItemBalcao.Valor;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}