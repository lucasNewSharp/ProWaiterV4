using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.GestoresBD
{
    public class ProWaiterContextBDProvider : ContextoBDProvider
    {
        protected override ContextoBD InstanciarNovoContextoBD()
        {
            return new ProWaiterContext();
        }
    }
}
