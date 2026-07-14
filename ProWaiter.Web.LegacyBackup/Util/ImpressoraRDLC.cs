using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProWaiter.Web.Models.Entidades;
using System.Configuration;
using NewSharp.Ferramentas.Impressoras.Termicas;

namespace ProWaiter.Web.Util
{
    public class ImpressoraRDLC : ITipoImpressoraProWaiter
    {
        private ImpressoraArquivoTexto ImprArqTexto = null;

        private bool UtilizaComanda { get; set; }

        public string NomeParaExibicao { get { return "Impressora de Relatório do Windows"; } }

        public Impressora ImpressoraConectada { get; private set; }

        public bool CortarPapel() { return true; }

        public eStatusImpressora ObterStatusImpressora()
        {
            return ImprArqTexto.ObterStatusImpressora();
        }

        public ImpressoraRDLC()
        {
            //TODO: Implementar o ImpressoraRDL
            ImprArqTexto = new ImpressoraArquivoTexto(Configuracoes.ObterInstancia().PastaArquivosImpressao);
            UtilizaComanda = Configuracoes.ObterInstancia().UtilizaComanda;
        }

        public bool ConectarImpressora(Impressora impressora)
        {
            ImprArqTexto.ConectarImpressora(impressora);
            ImpressoraConectada = impressora;
            return ImpressoraConectada != null;
        }

        public bool DeconectarImpressora()
        {
            ImprArqTexto.DeconectarImpressora();
            ImpressoraConectada = null;
            return true;
        }

        public bool ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, string textoRodape = null)
        {
            //TODO: Implementar o ImpressoraRDL
            return ImprArqTexto.ImprimirBebidas(pedido, bebidas, textoRodape);
        }

        public bool ImprimirBebidas(PedidoInterno pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            //TODO: Implementar o ImpressoraRDLC
            return ImprArqTexto.ImprimirBebidas(pedido, bebidas, mesa, localInterno, textoRodape);
        }

        public bool ImprimirRefeicoes(PedidoInterno pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            //TODO: Implementar o ImpressoraRDLC
            return ImprArqTexto.ImprimirRefeicoes(pedido, refeicoesDoPedido, mesa, localInterno, textoRodape);
        }

        public bool ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, string textoRodape = null)
        {
            //TODO: Implementar o ImpressoraRDLC
            return ImprArqTexto.ImprimirRefeicoes(pedido, refeicoesDoPedido, textoRodape);
        }

        public bool ImprimirTodoPedido(PedidoInterno pedido, Mesa mesa, decimal? valorRecebido)
        {
            //TODO: Implementar o ImpressoraRDLC
            return ImprArqTexto.ImprimirTodoPedido(pedido, mesa, valorRecebido);
        }

        public bool ImprimirTodoPedido(Pedido pedido, decimal? valorRecebido)
        {
            //TODO: Implementar o ImpressoraRDLC
            return ImprArqTexto.ImprimirTodoPedido(pedido, valorRecebido);
        }
    }
}