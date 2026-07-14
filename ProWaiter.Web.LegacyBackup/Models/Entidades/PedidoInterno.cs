using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum ePedidoInterno { CodPedido, CodLocalInterno }
    public class PedidoInterno : Pedido
    {
        public short? CodLocalInterno { get; set; }
        public virtual LocalInterno LocalInterno { get; set; }

        public short? CodMesa { get; set; }
        public virtual Mesa Mesa { get; set; }

        protected PedidoInterno() : base() { }
        public PedidoInterno(DateTime dataInicio, string nomeUsuario, decimal acrescimos, decimal descontos, string observacoes)
            : base(dataInicio, nomeUsuario, acrescimos, descontos, observacoes)
        {

        }

        public override string ToString()
        {
            return String.Format("Pedido interno {0} de {1}", Codigo, DataInicio);
        }
    }
}
