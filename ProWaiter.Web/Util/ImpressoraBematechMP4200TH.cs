/* 
using NewSharp.Ferramentas.Impressoras.Termicas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Util
{
    public class ImpressoraBematechMP4200TH : ImpressoraEscPos
    {
        public ImpressoraBematechMP4200TH():base()
        {
            NomeParaExibicao = "Impressora Bematech MP-4200 TH";            
        }

        protected override IImpressoraTermica InstanciarImpressoraTermica()
        {
            return new BematechMP4200TH();
        }
    }
}
 */