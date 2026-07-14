using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum ePedidoParaLevar { CodPedido }
    public class PedidoParaLevar: Pedido
    {
        protected PedidoParaLevar() : base() { }
        public PedidoParaLevar(DateTime dataInicio, string nomeUsuario, decimal acrescimos, decimal descontos, string observacoes)
                : base(dataInicio, nomeUsuario, acrescimos, descontos, observacoes)
            { }
        public override string ToString()
        {
            return string.Format("Pedido para levar {0} de {1}", Codigo, DataInicio);
        }
    }

}