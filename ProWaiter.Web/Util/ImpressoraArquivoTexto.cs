/* 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProWaiter.Web.Models.Entidades;
using System.IO;
using System.Text;
using NewSharp.Ferramentas.Impressoras.Termicas;

namespace ProWaiter.Web.Util
{
    public class ImpressoraArquivoTexto : ITipoImpressoraProWaiter
    {
        public string PastaBaseArquivos { get; set; }
        private bool UtilizaComanda { get; set; }
        private bool ImprimirNomeGarcomTiket { get; set; }
        public ImpressoraArquivoTexto(string pastaArquivos)
        {
            if (!Directory.Exists(pastaArquivos))
                Directory.CreateDirectory(pastaArquivos);

            PastaBaseArquivos = pastaArquivos;
            Configuracoes config = Configuracoes.ObterInstancia();
            UtilizaComanda = config.UtilizaComanda;
            ImprimirNomeGarcomTiket = config.ImprimirNomeGarcomTicket;
        }

        public string NomeParaExibicao { get { return "Arquivo Texto"; } }

        public Impressora ImpressoraConectada { get; private set; }

        public bool ConectarImpressora(Impressora impressora)
        {
            ImpressoraConectada = impressora;
            return ImpressoraConectada != null;
        }

        public bool DeconectarImpressora()
        {
            ImpressoraConectada = null;
            return true;
        }


        public bool CortarPapel() { return true; }

        public eStatusImpressora ObterStatusImpressora() { return eStatusImpressora.Normal; }

        public bool ImprimirBebidas(PedidoInterno pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido, mesa, false, localInterno);
                ImprimirBebidas(bebidas, sb, textoRodape);

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }

        public bool ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, string textoRodape = null)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido);
                ImprimirBebidas(bebidas, sb, textoRodape);

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }

        #region Helpers

        private string GerarNomeArquivo(Pedido pedido)
        {
            return String.Format("{0}Pedido{1:00000}_{2}_Impr{3}.txt", PastaBaseArquivos, pedido.Codigo, DateTime.Now.ToString("yyyyMMdd_HHmmss"), ImpressoraConectada.Local);
        }

        private StringBuilder GerarCabecalho(PedidoInterno pedido, Mesa mesa, bool podeTerMesaNula, LocalInterno localInterno)
        {
            var sb = new StringBuilder("Pedido Interno ").Append(pedido.Codigo).Append("\r\n");

            if (!UtilizaComanda)
            {
                if (podeTerMesaNula && mesa == null)
                    sb.Append("Local: Mesa indefinida\r\n");
                else
                    sb.Append("Local: " + mesa.Descricao + "\r\n");
            }
            else
            {
                if (localInterno != null)
                    sb.Append($"Local: {localInterno.Nome}");

                if (podeTerMesaNula && mesa == null)
                    sb.Append("Comanda: Comanda indefinida\r\n");
                else
                    sb.Append("Comanda: " + mesa.Descricao + "\r\n");

            }
            return sb;
        }

        private StringBuilder GerarCabecalho(Pedido pedido)
        {
            if (pedido is PedidoExterno)
                return new StringBuilder("Pedido Para Entrega ").Append(pedido.Codigo).Append("\r\n")
                                        .Append("Cliente ").Append(((PedidoExterno)pedido).Cliente.Nome).Append("\r\n");
            return new StringBuilder("PEDIDO PARA LEVAR ").Append(pedido.Codigo).Append("\r\n");
        }

        private void ImprimirBebidas(IEnumerable<BebidaDoPedido> bebidas, StringBuilder sb, string textoRodape = null)
        {
            foreach (BebidaDoPedido b in bebidas)
            {
                sb.Append("- ").Append(b.Bebida.Nome);
                if (!String.IsNullOrEmpty(b.Observacoes))
                    sb.Append(" (").Append(b.Observacoes).Append(")");

                sb.Append("\r\n");

                if (!String.IsNullOrWhiteSpace(textoRodape))
                    sb.Append(textoRodape).Append("\r\n");
            }
        }

        private void ImprimirRefeicoes(IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, StringBuilder sb, string textoRodape = null)
        {
            foreach (RefeicaoDoPedido r in refeicoesDoPedido)
            {
                sb.Append("- ").Append(r.RefeicaoDoCardapio.Refeicao.Nome).Append("\r\n");
                foreach (var c in r.ComponentesRefeicaoPedido)
                    sb.Append("\t").Append(c.ComponenteRefeicao.Nome).Append("\r\n");
                if (!String.IsNullOrEmpty(r.Observacoes))
                    sb.Append("OBS.: ").Append(r.Observacoes);
                sb.Append("\r\n");

                if (!String.IsNullOrWhiteSpace(textoRodape))
                    sb.Append(textoRodape).Append("\r\n");
            }
        }

        #endregion

        public bool ImprimirRefeicoes(PedidoInterno pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido, mesa, false, localInterno);
                ImprimirRefeicoes(refeicoesDoPedido, sb, textoRodape);

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }

        public bool ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, string textoRodape = null)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido);
                ImprimirRefeicoes(refeicoesDoPedido, sb, textoRodape);

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }

        public bool ImprimirTodoPedido(PedidoInterno pedido, Mesa mesa, decimal? valorRecebido)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido, mesa, true, null);
                sb.Append("*** REFEIÇÕES ***\r\n");
                ImprimirRefeicoes(pedido.RefeicoesDoPedido, sb);
                sb.Append("**** BEBIDAS ****\r\n");
                ImprimirBebidas(pedido.BebidasDoPedido, sb);

                if (valorRecebido.HasValue)
                    sb.Append("Valor Recebido: R$ " + valorRecebido.Value.ToString("{0:C}"));

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }

        public bool ImprimirTodoPedido(Pedido pedido, decimal? valorRecebido)
        {
            try
            {
                string arquivo = GerarNomeArquivo(pedido);

                StringBuilder sb = GerarCabecalho(pedido);
                sb.Append("*** REFEIÇÕES ***\r\n");
                ImprimirRefeicoes(pedido.RefeicoesDoPedido, sb);
                sb.Append("**** BEBIDAS ****\r\n");
                ImprimirBebidas(pedido.BebidasDoPedido, sb);

                if (valorRecebido.HasValue)
                    sb.Append("Valor Recebido: " + valorRecebido.Value.ToString("{0:C}"));

                File.AppendAllText(arquivo, sb.ToString());
                return true;
            }
            catch { return false; }
        }
    }
}
 */