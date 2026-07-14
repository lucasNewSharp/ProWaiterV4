using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProWaiter.Web.Models.Entidades;
using NewSharp.Ferramentas.Impressoras.Termicas;
using System.Text;

namespace ProWaiter.Web.Util
{
    public abstract class ImpressoraEscPos : ITipoImpressoraProWaiter
    {
        private const int numLinhasFinalImpressao = 6;
        public string NomeParaExibicao { get; protected set; }

        protected IImpressoraTermica ImprEscPos = null;

        public Impressora ImpressoraConectada { get; private set; }

        private readonly Configuracoes _config;

        public ImpressoraEscPos()
        {
            _config = Configuracoes.ObterInstancia();
        }

        public bool ConectarImpressora(Impressora impressora)
        {
            ImpressoraConectada = impressora ?? throw new ArgumentNullException("impressora");
            ImprEscPos = InstanciarImpressoraTermica();
            ImprEscPos.ConectarImpressora(impressora.Ip, impressora.Porta);
            return impressora != null;
        }

        protected abstract IImpressoraTermica InstanciarImpressoraTermica();

        public bool DeconectarImpressora()
        {
            if (ImprEscPos != null)
                ImprEscPos.DesconectarImpressora();
            ImpressoraConectada = null;
            return true;
        }

        private void ValidarImpressoraConectada()
        {
            if (ImprEscPos == null)
                throw new ApplicationException("A ImpressoraEscPos do ProWaiter não foi conectada!");
            if (ImpressoraConectada == null)
                throw new ApplicationException("A ImpressoraEscPos do ProWaiter não foi conectada");
        }

        public bool CortarPapel()
        {
            return ImprEscPos.CortarPapel();
        }

        public eStatusImpressora ObterStatusImpressora()
        {
            return ImprEscPos.ObterStatusImpressora();
        }

        private RetornoImpressoraTermica GerarCabecalho(RetornoImpressoraTermica impr, PedidoInterno pedido, Mesa mesa, LocalInterno localInterno, bool podeTerMesaNula)
        {
            impr
                .SelecionarFonte(eFonte.AlturaDupla)
                .SelecionarAlinhamento(eAlinhamento.Esquerda)
                .ImprimirTexto("Pedido Interno\r\n")
                .SelecionarFonte(eFonte.ModoDestaque | eFonte.AlturaDupla | eFonte.LarguraDupla);

            if (!_config.UtilizaComanda)
            {
                if (podeTerMesaNula && mesa == null)
                    impr.ImprimirTexto("Local: Mesa indefinida\r\n");
                else
                    impr.ImprimirTexto("Local: " + mesa.Descricao + "\r\n");
            }
            else
            {
                if (localInterno != null)
                    impr.ImprimirTexto($"Local: {localInterno.Nome}\r\n");

                if (podeTerMesaNula && mesa == null)
                    impr.ImprimirTexto("Comanda: Comanda indefinida\r\n");
                else
                    impr.ImprimirTexto("Comanda: " + mesa.Descricao + "\r\n");
            }

            return impr;
        }

        private RetornoImpressoraTermica GerarCabecalho(RetornoImpressoraTermica impr, Pedido pedido)
        {
            if (pedido is PedidoExterno pedExterno)
            {
                string sequencial = string.Empty;
                if (_config.ImprimirSequencialFechamentoPedidoEntrega)
                {
                    sequencial = " - " + pedExterno.SequencialNoDia.ToString("0000");
                }

                Cliente cliente = pedExterno.Cliente;
                impr
                    .SelecionarFonte(eFonte.AlturaDupla)
                    .SelecionarAlinhamento(eAlinhamento.Esquerda)
                    .ImprimirTexto("Pedido Para Entrega" + sequencial + "\r\n")
                    .ImprimirTexto("Cliente: " + cliente.Nome + "\r\n")
                    .SelecionarFonte(eFonte.Padrao);

                if (!string.IsNullOrEmpty(cliente.Telefone1))
                    impr.ImprimirTexto("Telefone 1: " + cliente.Telefone1 + "\r\n");

                if (!string.IsNullOrEmpty(cliente.Telefone2))
                    impr.ImprimirTexto("Telefone 2: " + cliente.Telefone2 + "\r\n");

                impr
                    .ImprimirTexto("Endereço: " + pedExterno.EnderecoCliente?.Endereco + "\r\n")
                    .ImprimirTexto("Bairro: " + pedExterno.EnderecoCliente?.Bairro + "\t " + pedExterno.EnderecoCliente?.Cidade.ToString() + "\r\n");

                if (!string.IsNullOrWhiteSpace(pedido.Observacoes))
                    impr.ImprimirTexto("Observações: " + pedido.Observacoes + "\r\n");

                impr.SelecionarFonte(eFonte.AlturaDupla);
            }
            else if (pedido is PedidoParaLevar)
            {
                impr
                .SelecionarFonte(eFonte.AlturaDupla)
                .SelecionarAlinhamento(eAlinhamento.Esquerda)
                .ImprimirTexto("PEDIDO PARA LEVAR\r\n")
                .SelecionarFonte(eFonte.AlturaDupla);
            }
            else
                throw new ArgumentException("O pedido deve ser externo ou para levar na chamada do método GerarCabecalho!", "pedido");
            return impr;
        }

        private RetornoImpressoraTermica ImprimirBebidas(RetornoImpressoraTermica impr, IEnumerable<BebidaDoPedido> bebidas, eTipoImpressao tipoImpressao)
        {
            IEnumerable<BebidaDoPedido> bebidasOrdenadasPorGarcom = bebidas.OrderBy(b => b.NomeUsuario);

            string ultimoGarcom = null;
            foreach (BebidaDoPedido beb in bebidasOrdenadasPorGarcom)
            {
                switch (tipoImpressao)
                {
                    case eTipoImpressao.impressaoFechamentoComDetalhes:
                        impr
                            .SelecionarFonte()
                            .ImprimirTexto("- " + beb.Bebida.Nome, beb.Valor.ToString("C"), eFonte.FonteA, '.');
                        impr.ImprimirTexto("\r\n");

                        if (!String.IsNullOrEmpty(beb.Observacoes))
                            impr.ImprimirTexto("\tOBS.: " + beb.Observacoes + "\r\n");
                        break;
                    case eTipoImpressao.impressaoFechamentoSemDetalhes:
                        impr
                            .SelecionarFonte()
                            .ImprimirTexto("- " + beb.Bebida.Nome, beb.Valor.ToString("C"), eFonte.FonteA, '.');
                        impr.ImprimirTexto("\r\n");
                        break;
                    case eTipoImpressao.impressaoNaoFechamento:
                        if (_config.ImprimirNomeGarcomTicket && !string.IsNullOrWhiteSpace(beb.NomeUsuario) && ultimoGarcom != beb.NomeUsuario)
                            impr
                                .SelecionarFonte(eFonte.AlturaDupla)
                                .ImprimirTexto(beb.NomeUsuario)
                                .ImprimirLinhasEmBranco();

                        impr
                            .SelecionarFonte(eFonte.AlturaDupla)
                            .ImprimirTexto("- " + beb.Bebida.Nome);
                        impr.ImprimirTexto("\r\n");

                        if (!String.IsNullOrEmpty(beb.Observacoes))
                            impr.ImprimirTexto("\tOBS.: " + beb.Observacoes + "\r\n");

                        break;
                }
                ultimoGarcom = beb.NomeUsuario;
            }

            return impr;
        }

        private enum eTipoImpressao { impressaoFechamentoComDetalhes, impressaoFechamentoSemDetalhes, impressaoNaoFechamento }
        private RetornoImpressoraTermica ImprimirRefeicoes(RetornoImpressoraTermica impr, IEnumerable<RefeicaoDoPedido> refeicoes, eTipoImpressao tipoImpressao)
        {
            IEnumerable<RefeicaoDoPedido> reficoesOrdenadasPorGarcom = refeicoes.OrderBy(r => r.NomeUsuario).ThenBy(r => r.Tamanho.Nome).ThenBy(r => r.RefeicaoDoCardapio.Refeicao.Nome);

            string ultimoGarcom = null;
            foreach (RefeicaoDoPedido refDoPed in reficoesOrdenadasPorGarcom)
            {
                switch (tipoImpressao)
                {
                    case eTipoImpressao.impressaoFechamentoComDetalhes:
                        impr
                           .SelecionarFonte()
                           .ImprimirTexto("- " + refDoPed.RefeicaoDoCardapio.Refeicao.Nome + " (" + refDoPed.Tamanho.Nome + ")" + (refDoPed.Acrescimo != 0 ? "*" : ""), (refDoPed.Valor + refDoPed.Acrescimo).ToString("C"), eFonte.FonteA, '.');
                        impr.ImprimirTexto("\r\n");
                        ImprimirDetalhes(impr, refDoPed);
                        break;
                    case eTipoImpressao.impressaoFechamentoSemDetalhes:
                        impr
                           .SelecionarFonte()
                           .ImprimirTexto("- " + refDoPed.RefeicaoDoCardapio.Refeicao.Nome + " (" + refDoPed.Tamanho.Nome + ")" + (refDoPed.Acrescimo != 0 ? "*" : ""), (refDoPed.Valor + refDoPed.Acrescimo).ToString("C"), eFonte.FonteA, '.');
                        break;
                    case eTipoImpressao.impressaoNaoFechamento:
                        if (_config.ImprimirNomeGarcomTicket && !string.IsNullOrWhiteSpace(refDoPed.NomeUsuario) && ultimoGarcom != refDoPed.NomeUsuario)
                            impr.SelecionarFonte(eFonte.AlturaDupla)
                                .ImprimirTexto(refDoPed.NomeUsuario)
                                .ImprimirLinhasEmBranco();

                        impr
                            .SelecionarFonte(eFonte.AlturaDupla | eFonte.ModoDestaque | eFonte.LarguraDupla)
                            .ImprimirTexto("- " + refDoPed.RefeicaoDoCardapio.Refeicao.Nome)
                            .ImprimirTexto(" (" + refDoPed.Tamanho.Nome + ")")
                            .SelecionarFonte(eFonte.AlturaDupla);
                        impr.ImprimirTexto("\r\n");
                        impr.SelecionarFonte(eFonte.AlturaDupla | eFonte.ModoDestaque | eFonte.LarguraDupla);

                        ImprimirDetalhes(impr, refDoPed);
                        impr.SelecionarFonte(eFonte.Padrao)
                            .ImprimirTexto("\r\n");
                        break;
                }
                ultimoGarcom = refDoPed.NomeUsuario;
            }
            return impr;
        }

        private static void ImprimirDetalhes(RetornoImpressoraTermica impr, RefeicaoDoPedido refDoPed)
        {
            if (refDoPed.RefeicaoDoCardapio.DeComposicao)
            {
                int quantidadeProporcional = 0;
                foreach (ComponenteRefeicaoPedido comp in refDoPed.ComponentesRefeicaoPedido)
                {
                    ComponenteComposicaoRefeicaoCardapio componenteComposicao = refDoPed.RefeicaoDoCardapio.ComponentesComposicaoRefeicao.Where(c => c.CodComponente == comp.CodComponente && c.CodTamanho == refDoPed.CodTamanho).Single();
                    if (componenteComposicao.CalculoProporcional)
                        quantidadeProporcional += comp.Quantidade;
                }

                foreach (ComponenteRefeicaoPedido comp in refDoPed.ComponentesRefeicaoPedido)
                {
                    string texto = "\t" + comp.ComponenteRefeicao.Nome;

                    ComponenteComposicaoRefeicaoCardapio componenteComposicao = refDoPed.RefeicaoDoCardapio.ComponentesComposicaoRefeicao.Where(c => c.CodComponente == comp.CodComponente && c.CodTamanho == refDoPed.CodTamanho).Single();
                    if (componenteComposicao.CalculoProporcional)
                    {
                        if (componenteComposicao.CodUnidade == UnidadeComponenteComposicao.CodPartes)
                            texto += "(" + comp.Quantidade + "/" + quantidadeProporcional + ")";
                    }
                    else if (!string.IsNullOrWhiteSpace(componenteComposicao.CodUnidade))
                    {
                        texto += "(" + comp.Quantidade + " " + componenteComposicao.CodUnidade + ")";
                    }
                    texto += "\r\n";
                    impr.ImprimirTexto(texto);
                }
            }
            else
            {
                foreach (ComponenteRefeicao comp in refDoPed.RefeicaoDoCardapio.Refeicao.ComponentesRefeicao.Except(refDoPed.ComponentesRefeicaoPedido.Select(c => c.ComponenteRefeicao)))
                    impr.ImprimirTexto("\tSEM " + comp.Nome + "\r\n");
            }
            if (!string.IsNullOrEmpty(refDoPed.Observacoes))
                impr.ImprimirTexto("\tOBS.: " + refDoPed.Observacoes + "\r\n");
        }

        public bool ImprimirBebidas(PedidoInterno pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                GerarCabecalho(impr, pedido, mesa, localInterno, false);
                ImprimirBebidas(impr, bebidas, eTipoImpressao.impressaoNaoFechamento);

                return ImprimirRodape(impr, textoRodape)
                    .SelecionarFonte()
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                    .Retorno;
            }
            catch { return false; }
        }

        public bool ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, string textoRodape = null)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                GerarCabecalho(impr, pedido);
                ImprimirBebidas(impr, bebidas, eTipoImpressao.impressaoNaoFechamento);

                return ImprimirRodape(impr, textoRodape)
                    .SelecionarFonte()
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                    .Retorno;
            }
            catch { return false; }
        }

        public bool ImprimirRefeicoes(PedidoInterno pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, string textoRodape = null)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                GerarCabecalho(impr, pedido, mesa, localInterno, false);
                ImprimirRefeicoes(impr, refeicoesDoPedido, eTipoImpressao.impressaoNaoFechamento);

                return ImprimirRodape(impr, textoRodape)
                    .SelecionarFonte()
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                    .Retorno;
            }
            catch { return false; }
        }

        public bool ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, string textoRodape = null)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                GerarCabecalho(impr, pedido);
                ImprimirRefeicoes(impr, refeicoesDoPedido, eTipoImpressao.impressaoNaoFechamento);

                return ImprimirRodape(impr, textoRodape)
                    .SelecionarFonte()
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                    .Retorno;
            }
            catch { return false; }
        }

        private bool ImprimirAdicionaisMolho(PedidoExterno pedidoExterno)
        {
            if (!_config.ExibirAdicionaisMolhosPedidoEntrega)
                return true;

            //Se não existe nada não imprimimos
            if ((!pedidoExterno.Maionese.HasValue || !pedidoExterno.Maionese.Value) &&
                (!pedidoExterno.Mostarda.HasValue || !pedidoExterno.Mostarda.Value) &&
                (!pedidoExterno.Catchup.HasValue || !pedidoExterno.Catchup.Value))
                return true;

            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                ImprimirLinhaSeparadora(impr, eFonte.Padrao);
                impr.SelecionarFonte()
                    .ImprimirTexto("**** MOLHOS ****\r\n");

                if (pedidoExterno.Maionese.HasValue && pedidoExterno.Maionese.Value)
                    impr.ImprimirTexto("- Maionese\r\n");
                if (pedidoExterno.Catchup.HasValue && pedidoExterno.Catchup.Value)
                    impr.ImprimirTexto("- Catchup\r\n");
                if (pedidoExterno.Mostarda.HasValue && pedidoExterno.Mostarda.Value)
                    impr.ImprimirTexto("- Mostarda\r\n");

                return impr.Retorno;

            }
            catch { return false; }
        }

        private RetornoImpressoraTermica ImprimirRodape(RetornoImpressoraTermica impr, string textoRodape)
        {
            if (!string.IsNullOrEmpty(textoRodape))
                impr
                    .SelecionarFonte(eFonte.FonteA)
                    .SelecionarAlinhamento(eAlinhamento.Direita)
                    .ImprimirTexto(textoRodape)
                    .SelecionarAlinhamento();
            return impr;
        }

        public bool ImprimirTodoPedido(PedidoInterno pedido, Mesa mesa, decimal? valorRecebido)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                ImprimirLinhaSeparadora(impr, eFonte.Padrao);
                GerarCabecalho(impr, pedido, mesa, null, true);
                return ImprimirRestanteTodoPedido(impr, pedido, eTipoImpressao.impressaoFechamentoSemDetalhes, valorRecebido);
            }
            catch { return false; }
        }

        public bool ImprimirTodoPedido(Pedido pedido, decimal? valorRecebido)
        {
            ValidarImpressoraConectada();
            try
            {
                RetornoImpressoraTermica impr = new RetornoImpressoraTermica(ImprEscPos, ImpressoraConectada.Nome, ImpressoraConectada.BuzinaAtivada);
                ImprimirLinhaSeparadora(impr, eFonte.Padrao);
                GerarCabecalho(impr, pedido);

                eTipoImpressao tipoImpressao = eTipoImpressao.impressaoFechamentoSemDetalhes;
                if (pedido is PedidoExterno pedidoExterno)
                {
                    tipoImpressao = eTipoImpressao.impressaoFechamentoComDetalhes;
                    ImprimirAdicionaisMolho(pedidoExterno);
                }

                return ImprimirRestanteTodoPedido(impr, pedido, tipoImpressao, valorRecebido);
            }
            catch { return false; }
        }

        private RetornoImpressoraTermica ImprimirLinhaSeparadora(RetornoImpressoraTermica retornoImprTerminca, eFonte fonte)
        {
            StringBuilder sb = new StringBuilder();
            int numCaracteres = retornoImprTerminca.ImpressoraTermica.ObterNumCaracteresDaLinha(fonte);
            for (int i = 0; i < numCaracteres; i++)
                sb.Append("-");
            sb.Append("\r\n");

            return retornoImprTerminca
                .SelecionarFonte(fonte)
                .ImprimirTexto(sb.ToString());
        }

        private bool ImprimirRestanteTodoPedido(RetornoImpressoraTermica impr, Pedido pedido, eTipoImpressao tipoImpressao, decimal? valorRecebido)
        {
            ImprimirLinhaSeparadora(impr, eFonte.Padrao)
                .ImprimirTexto("*** REFEIÇÕES ***\r\n");

            ImprimirRefeicoes(impr, pedido.RefeicoesDoPedido, tipoImpressao);

            impr
                .SelecionarFonte()
                .ImprimirTexto("**** BEBIDAS ****\r\n");

            ImprimirBebidas(impr, pedido.BebidasDoPedido, tipoImpressao);

            if (pedido.ItensBalcaoDoPedido.Count() > 0)
            {
                impr.SelecionarFonte()
                    .SelecionarAlinhamento()
                    .ImprimirTexto("*** DIVERSOS ****\r\n");

                foreach (ItemBalcaoDoPedido item in pedido.ItensBalcaoDoPedido)
                {
                    impr.SelecionarFonte()
                            .ImprimirTexto("- " + item.ItemBalcao.Nome, item.Valor.ToString("C"), eFonte.FonteA, '.');
                    impr.ImprimirTexto("\r\n");
                }
            }
            impr.SelecionarAlinhamento(eAlinhamento.Direita)
                .ImprimirLinhasEmBranco();

            ImprimirLinhaSeparadora(impr, eFonte.Padrao)
                .ImprimirTexto(String.Format("  ACRÉSCIMO {0:C}\r\n", pedido.Acrescimos))
                .ImprimirTexto(String.Format("   DESCONTO {0:C}\r\n", pedido.Descontos));

            if (valorRecebido.HasValue)
                impr
                    .ImprimirTexto(String.Format("VALOR RECEBIDO {0:C}\r\n", valorRecebido.Value))
                    .ImprimirTexto(String.Format("TROCO {0:C}\r\n", valorRecebido.Value - pedido.ValorTotal));

            if (pedido is PedidoExterno)
                impr.ImprimirTexto(string.Format("VALOR ENTREGA {0:C}\r\n", ((PedidoExterno)pedido).ValorEntrega));

            impr
                .SelecionarFonte(eFonte.AlturaDupla | eFonte.ModoDestaque)
                .ImprimirTexto(String.Format("VALOR TOTAL {0:C}\r\n", pedido.ValorTotal))
                .SelecionarFonte();

            Configuracoes config = Configuracoes.ObterInstancia();
            if (!string.IsNullOrWhiteSpace(config.TextoFinalCupomFechamento))
            {
                if ((pedido is PedidoInterno && config.ImprimirTextoCupomFechamentoInterno) ||
                    (pedido is PedidoExterno && config.ImprimirTextoCupomFechamentoTeleEntrega))
                {
                    impr.SelecionarAlinhamento(eAlinhamento.Esquerda)
                        .ImprimirTexto(Environment.NewLine + config.TextoFinalCupomFechamento + Environment.NewLine + Environment.NewLine)
                        .SelecionarAlinhamento(eAlinhamento.Direita);
                }
            }

            if (pedido is PedidoExterno && Configuracoes.ObterInstancia().ImprimirHorarioGrandePedidoEntrega)
            {
                return
                   impr.SelecionarFonte(eFonte.FonteC | eFonte.ModoDestaque | eFonte.AlturaDupla | eFonte.LarguraDupla)
                   .SelecionarAlinhamento(eAlinhamento.Centralizado)
                   .ImprimirTexto(DateTime.Now.ToString("HH:mm:ss"))
                   .SelecionarAlinhamento()
                   .SelecionarFonte()
                    .SelecionarAlinhamento(eAlinhamento.Esquerda)
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                .Retorno;
            }
            else
            {
                return ImprimirRodape(impr, DateTime.Now.ToString())
                    .SelecionarFonte()
                    .SelecionarAlinhamento(eAlinhamento.Esquerda)
                    .ImprimirLinhasEmBranco(numLinhasFinalImpressao)
                    .AtivarAltoFalante()
                    .Retorno;
            }
        }
    }
}