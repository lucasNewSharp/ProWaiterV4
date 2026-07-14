/* 
using NewSharp.Ferramentas.Impressoras.Termicas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Util
{
    public class ImpressoraElginI9 : ImpressoraEscPos
    {
        public ImpressoraElginI9() : base()
        {
            NomeParaExibicao = "Impressora Elgin I9";            
        }

        protected override IImpressoraTermica InstanciarImpressoraTermica()
        {
            return new ElginI9();
        }
    }
}
 */