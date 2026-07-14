using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models
{
    [Serializable]
    public class ComponenteRefeicaoPedidoViewModel
    {
        public int CodRefeicaoPedido { get; set; }
        public short CodComponente { get; set; }
        public byte Quantidade { get; set; }

        public ComponenteRefeicaoPedidoViewModel() { }
    }
}