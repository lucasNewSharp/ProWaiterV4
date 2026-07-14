using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public class PedidoExterno : Pedido
    {
        public PedidoExterno() : base() { }
        
        public int CodCliente { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Display(Name = "Valor entrega: R$ ")]
        public decimal ValorEntrega { get; set; }
        
        public short SequencialNoDia { get; set; }

        public bool? Catchup { get; set; }
        public bool? Mostarda { get; set; }
        public bool? Maionese { get; set; }

        public int? CodEnderecoEntrega { get; set; }
        public virtual EnderecoCliente EnderecoCliente { get; set; }

        public override string ToString()
        {
            return string.Format("Pedido externo {0} do cliente de código {1}", Codigo, CodCliente);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = (List<ValidationResult>)base.Validate(validationContext);
            return retorno;
        }
    }
}
