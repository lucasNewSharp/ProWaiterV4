using NewSharp.Ferramentas.Impressoras.Termicas;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;

namespace ProWaiter.Web.Util
{
    public interface ITipoImpressoraProWaiter
    {
        string NomeParaExibicao { get; }

        Impressora ImpressoraConectada { get; }
        bool ConectarImpressora(Impressora impressora);
        bool DeconectarImpressora();

        eStatusImpressora ObterStatusImpressora();
        bool CortarPapel();
        bool ImprimirBebidas(PedidoInterno pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno, string textoRodape = null);
        bool ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, string textoRodape = null);
        bool ImprimirRefeicoes(PedidoInterno pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, string textoRodape = null);
        bool ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, string textoRodape = null);

        bool ImprimirTodoPedido(PedidoInterno pedido, Mesa mesa, decimal? valorRecebido);
        bool ImprimirTodoPedido(Pedido pedido, decimal? valorRecebido);
    }

    public class GestorImpressoes
    {
        #region Retornos

        public class RetornoImpressaoRefeicoes
        {
            public List<RefeicaoDoPedido> RefeicoesEnviadas { get; set; }
            public List<RefeicaoDoPedido> RefeicoesNaoEnviadas { get; set; }
            public List<Exception> Erros { get; set; }
            public string ImpressorasComProblema { get; set; }

            public RetornoImpressaoRefeicoes()
            {
                this.RefeicoesEnviadas = new List<RefeicaoDoPedido>();
                this.RefeicoesNaoEnviadas = new List<RefeicaoDoPedido>();
                this.Erros = new List<Exception>();
            }
        }

        public class RetornoImpressaoBebidas
        {
            public List<BebidaDoPedido> BebidasEnviadas { get; set; }
            public List<BebidaDoPedido> BebidasNaoEnviadas { get; set; }
            public List<Exception> Erros { get; set; }
            public string ImpressorasComProblema { get; set; }

            public RetornoImpressaoBebidas()
            {
                this.BebidasEnviadas = new List<BebidaDoPedido>();
                this.BebidasNaoEnviadas = new List<BebidaDoPedido>();
                this.Erros = new List<Exception>();
            }
        }

        #endregion

        private Dictionary<Type, ITipoImpressoraProWaiter> TiposImpressoras { get; set; }

        #region Singleton

        private static GestorImpressoes _instancia = new GestorImpressoes();
        public static GestorImpressoes Instancia { get { return _instancia; } }

        private GestorImpressoes()
        {
            string[] nomesTipos = Configuracoes.ObterInstancia().TiposImpressoras.Split(';');
            TiposImpressoras = new Dictionary<Type, Util.ITipoImpressoraProWaiter>();
            foreach (string nomeTipo in nomesTipos)
            {
                Type tipo = Type.GetType(nomeTipo);
                ITipoImpressoraProWaiter tipoImpressora = null;
                if (tipo == typeof(ImpressoraArquivoTexto))
                {
                    string pastaArquivosImpressao = Configuracoes.ObterInstancia().PastaArquivosImpressao;
                    tipoImpressora = (ITipoImpressoraProWaiter)Activator.CreateInstance(tipo, new object[] { pastaArquivosImpressao });
                }
                else
                    tipoImpressora = (ITipoImpressoraProWaiter)Activator.CreateInstance(tipo);

                TiposImpressoras.Add(tipo, tipoImpressora);
            }
        }

        #endregion

        /// <summary>
        /// Obtem os tipos de impressoras em um objeto anônimo contendo "Nome" e "Valor"
        /// </summary>
        /// <returns>Objeto anônimo contendo "Nome" e "Valor"</returns>
        public object[] ObterTiposImpressoras()
        {
            return TiposImpressoras.Values.Select(t => new { Nome = t.NomeParaExibicao, Valor = t.GetType().FullName }).Distinct().OrderBy(t => t.Nome).ToArray();
        }

        public string[] ObterNomesImpressorasInstaladas()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Printer");
            var results = searcher.Get();

            IList<string> printers = new List<string>();

            foreach (var printer in results)
            {
                if ((bool)printer["Local"])
                {
                    printers.Add((string)printer["Name"]);
                }
            }

            return printers.ToArray();
        }

        public string ObterNomeParaExibicao(Type tipoImpressora)
        {
            if (TiposImpressoras.ContainsKey(tipoImpressora))
                return TiposImpressoras[tipoImpressora].NomeParaExibicao;
            return null;
        }

        #region ImprimirRefeicoes

        public RetornoImpressaoRefeicoes ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno)
        {
            return ImprimirRefeicoes(pedido, refeicoesDoPedido, mesa, localInterno, false);
        }

        private RetornoImpressaoRefeicoes CriarRetornoRefeicoesComErro(IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Exception ex)
        {
            return new RetornoImpressaoRefeicoes()
            {
                RefeicoesNaoEnviadas = refeicoesDoPedido != null ? refeicoesDoPedido.ToList() : new List<RefeicaoDoPedido>(),
                Erros = new List<Exception>(new Exception[] { ex })
            };
        }

        private RetornoImpressaoRefeicoes ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, bool reimpressao)
        {
            if (pedido is PedidoInterno)
            {
                if (mesa == null)
                    return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ApplicationException("O Pedido interno deve ter uma mesa ao gerar uma impressao!"));
                return ImprimirRefeicoes((PedidoInterno)pedido, refeicoesDoPedido, mesa, localInterno, reimpressao);
            }
            else
            {
                if (mesa != null)
                    return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ApplicationException("O Pedido para entrega não deve ter uma mesa ao gerar uma impressao!"));
                return ImprimirRefeicoes(pedido, refeicoesDoPedido, reimpressao);
            }
        }

        private RetornoImpressaoRefeicoes ImprimirRefeicoes(PedidoInterno pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, Mesa mesa, LocalInterno localInterno, bool reimpressao)
        {
            if (pedido == null) return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ArgumentNullException("pedido"));
            if (refeicoesDoPedido == null) return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ArgumentNullException("refeicoesDoPedido"));
            if (mesa == null) return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ArgumentNullException("mesa"));

            RetornoImpressaoRefeicoes retorno = new RetornoImpressaoRefeicoes();
            var gruposImpressoras = refeicoesDoPedido.GroupBy(r => r.RefeicaoDoCardapio.Impressora);
            string rodape = GerarRodape(reimpressao, gruposImpressoras.Count());

            int numImp = 1;
            foreach (var grupoImpressora in gruposImpressoras)
            {
                Impressora impressora = grupoImpressora.Key;
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impressora.TipoImpressao];
                lock (tipoImpressora)
                {
                    IEnumerable<RefeicaoDoPedido> refeicoes = grupoImpressora.AsEnumerable();
                    try
                    {
                        tipoImpressora.ConectarImpressora(impressora);
                        if (
                            !TestarImpressora(tipoImpressora)
                            || !tipoImpressora.ImprimirRefeicoes(pedido, refeicoes, mesa, localInterno, string.Format(rodape, numImp++))
                            || !tipoImpressora.CortarPapel()
                            )
                        {
                            retorno.RefeicoesNaoEnviadas.AddRange(refeicoes);
                            retorno.ImpressorasComProblema += impressora.Local + "\n";
                            retorno.Erros.Add(new ApplicationException("Não foi possível imprimir as refeições da mesa " + mesa.Descricao + "(pedido " + pedido.Codigo + ")!"));
                        }
                        else
                            retorno.RefeicoesEnviadas.AddRange(refeicoes);
                    }
                    catch (Exception ex)
                    {
                        retorno.RefeicoesNaoEnviadas.AddRange(refeicoes);
                        retorno.ImpressorasComProblema += impressora.Local + "\n";
                        retorno.Erros.Add(ex);
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }
            return retorno;
        }

        private RetornoImpressaoRefeicoes ImprimirRefeicoes(Pedido pedido, IEnumerable<RefeicaoDoPedido> refeicoesDoPedido, bool reimpressao)
        {
            if (pedido == null) return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ArgumentNullException("pedido"));
            if (refeicoesDoPedido == null) return CriarRetornoRefeicoesComErro(refeicoesDoPedido, new ArgumentNullException("refeicoesDoPedido"));

            RetornoImpressaoRefeicoes retorno = new RetornoImpressaoRefeicoes();
            var gruposImpressoras = refeicoesDoPedido.GroupBy(r => r.RefeicaoDoCardapio.Impressora);
            string rodape = GerarRodape(reimpressao, gruposImpressoras.Count());

            int numImp = 1;
            foreach (var grupoImpressora in gruposImpressoras)
            {
                Impressora impressora = grupoImpressora.Key;
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impressora.TipoImpressao];
                lock (tipoImpressora)
                {
                    IEnumerable<RefeicaoDoPedido> refeicoes = grupoImpressora.AsEnumerable();
                    try
                    {
                        tipoImpressora.ConectarImpressora(impressora);
                        if (
                            !TestarImpressora(tipoImpressora)
                            || !tipoImpressora.ImprimirRefeicoes(pedido, refeicoes, string.Format(rodape, numImp++))
                            || !tipoImpressora.CortarPapel()
                            )
                        {
                            retorno.RefeicoesNaoEnviadas.AddRange(refeicoes);
                            retorno.ImpressorasComProblema += impressora.Local + "\n";
                            retorno.Erros.Add(new ApplicationException("Não foi possível imprimir as refeições do pedido " + pedido.Codigo + "!"));
                        }
                        else
                            retorno.RefeicoesEnviadas.AddRange(refeicoes);
                    }
                    catch (Exception ex)
                    {
                        retorno.RefeicoesNaoEnviadas.AddRange(refeicoes);
                        retorno.ImpressorasComProblema += impressora.Local + "\n";
                        retorno.Erros.Add(ex);
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }
            return retorno;
        }
        #endregion

        #region ImprimirBebidas
        public RetornoImpressaoBebidas ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno)
        {
            if (pedido is PedidoExterno && !Configuracoes.ObterInstancia().ImprimirLanchesPedidoExterno)
                return null;

            return ImprimirBebidas(pedido, bebidas, mesa, localInterno, false);
        }

        private RetornoImpressaoBebidas CriarRetornoBebibasComErro(IEnumerable<BebidaDoPedido> bebidas, Exception ex)
        {
            return new RetornoImpressaoBebidas()
            {
                BebidasNaoEnviadas = bebidas != null ? bebidas.ToList() : new List<BebidaDoPedido>(),
                Erros = new List<Exception>(new Exception[] { ex })
            };
        }

        private RetornoImpressaoBebidas ImprimirBebidas(Pedido pedido, IEnumerable<BebidaDoPedido> bebidas, Mesa mesa, LocalInterno localInterno, bool reimpressao)
        {
            if (pedido is PedidoInterno)
            {
                if (mesa == null)
                    return CriarRetornoBebibasComErro(bebidas, new ApplicationException("O Pedido interno deve ter uma mesa ao gerar uma impressao!"));
                return ImprimirBebidas((PedidoInterno)pedido, bebidas, mesa, localInterno, reimpressao);
            }
            else if (pedido is PedidoExterno)
            {
                if (mesa != null)
                    return CriarRetornoBebibasComErro(bebidas, new ApplicationException("O Pedido externo não deve ter uma mesa ou comanda ao gerar uma impressao!"));
                if (localInterno != null)
                    return CriarRetornoBebibasComErro(bebidas, new ApplicationException("O Pedido externo não deve ter um local interno ao gerar uma impressao!"));
                return ImprimirBebidas((PedidoExterno)pedido, bebidas, reimpressao);
            }
            else
            {
                return ImprimirBebidas((PedidoParaLevar)pedido, bebidas, reimpressao);
            }
        }

        private RetornoImpressaoBebidas ImprimirBebidas(PedidoExterno pedido, IEnumerable<BebidaDoPedido> bebidasDoPedido, bool reimpressao)
        {
            if (pedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("pedido"));
            if (bebidasDoPedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("bebidasDoPedido"));

            RetornoImpressaoBebidas retorno = new RetornoImpressaoBebidas();
            var gruposImpressoras = bebidasDoPedido.GroupBy(b => b.Bebida.Impressora);
            string rodape = GerarRodape(reimpressao, gruposImpressoras.Count());

            int numImp = 1;
            foreach (var grupoImpressora in gruposImpressoras)
            {
                Impressora impressora = grupoImpressora.Key;
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impressora.TipoImpressao];
                lock (tipoImpressora)
                {
                    IEnumerable<BebidaDoPedido> bebidas = grupoImpressora.AsEnumerable();
                    try
                    {
                        tipoImpressora.ConectarImpressora(impressora);
                        if (
                            !TestarImpressora(tipoImpressora)
                            || !tipoImpressora.ImprimirBebidas(pedido, bebidas, string.Format(rodape, numImp++))
                            || !tipoImpressora.CortarPapel())
                        {
                            retorno.BebidasNaoEnviadas.AddRange(bebidas);
                            retorno.ImpressorasComProblema += impressora.Local + "\n";
                            retorno.Erros.Add(new ApplicationException("Não foi possível imprimir as bebidas do pedido " + pedido.Codigo + "!"));
                        }
                        else
                            retorno.BebidasEnviadas.AddRange(bebidas);
                    }
                    catch (Exception ex)
                    {
                        retorno.BebidasNaoEnviadas.AddRange(bebidas);
                        retorno.ImpressorasComProblema += impressora.Local + "\n";
                        retorno.Erros.Add(ex);
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }
            return retorno;
        }

        private RetornoImpressaoBebidas ImprimirBebidas(PedidoParaLevar pedido, IEnumerable<BebidaDoPedido> bebidasDoPedido, bool reimpressao)
        {
            if (pedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("pedido"));
            if (bebidasDoPedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("bebidasDoPedido"));

            RetornoImpressaoBebidas retorno = new RetornoImpressaoBebidas();
            var gruposImpressoras = bebidasDoPedido.GroupBy(b => b.Bebida.Impressora);
            string rodape = GerarRodape(reimpressao, gruposImpressoras.Count());

            int numImp = 1;
            foreach (var grupoImpressora in gruposImpressoras)
            {
                Impressora impressora = grupoImpressora.Key;
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impressora.TipoImpressao];
                lock (tipoImpressora)
                {
                    IEnumerable<BebidaDoPedido> bebidas = grupoImpressora.AsEnumerable();
                    try
                    {
                        tipoImpressora.ConectarImpressora(impressora);
                        if (
                            !TestarImpressora(tipoImpressora)
                            || !tipoImpressora.ImprimirBebidas(pedido, bebidas, string.Format(rodape, numImp++))
                            || !tipoImpressora.CortarPapel())
                        {
                            retorno.BebidasNaoEnviadas.AddRange(bebidas);
                            retorno.ImpressorasComProblema += impressora.Local + "\n";
                            retorno.Erros.Add(new ApplicationException("Não foi possível imprimir as bebidas do pedido " + pedido.Codigo + "!"));
                        }
                        else
                            retorno.BebidasEnviadas.AddRange(bebidas);
                    }
                    catch (Exception ex)
                    {
                        retorno.BebidasNaoEnviadas.AddRange(bebidas);
                        retorno.ImpressorasComProblema += impressora.Local + "\n";
                        retorno.Erros.Add(ex);
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }
            return retorno;
        }

        private RetornoImpressaoBebidas ImprimirBebidas(PedidoInterno pedido, IEnumerable<BebidaDoPedido> bebidasDoPedido, Mesa mesa, LocalInterno localInterno, bool reimpressao)
        {
            if (pedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("pedido"));
            if (bebidasDoPedido == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("bebidasDoPedido"));
            if (mesa == null) return CriarRetornoBebibasComErro(bebidasDoPedido, new ArgumentNullException("mesa"));

            RetornoImpressaoBebidas retorno = new RetornoImpressaoBebidas();
            var gruposImpressoras = bebidasDoPedido.GroupBy(b => b.Bebida.Impressora);
            string rodape = GerarRodape(reimpressao, gruposImpressoras.Count());

            int numImp = 1;
            foreach (var grupoImpressora in gruposImpressoras)
            {
                Impressora impressora = grupoImpressora.Key;
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impressora.TipoImpressao];
                lock (tipoImpressora)
                {
                    IEnumerable<BebidaDoPedido> bebidas = grupoImpressora.AsEnumerable();
                    try
                    {
                        tipoImpressora.ConectarImpressora(impressora);
                        if (
                            !TestarImpressora(tipoImpressora)
                            || !tipoImpressora.ImprimirBebidas(pedido, bebidas, mesa, localInterno, string.Format(rodape, numImp++))
                            || !tipoImpressora.CortarPapel()
                            )
                        {
                            retorno.BebidasNaoEnviadas.AddRange(bebidas);
                            retorno.Erros.Add(new ApplicationException("Não foi possível imprimir as bebidas da mesa " + mesa.Descricao + "(pedido " + pedido.Codigo + ")!"));
                        }
                        else
                            retorno.BebidasEnviadas.AddRange(bebidas);
                    }
                    catch (Exception ex)
                    {
                        retorno.BebidasNaoEnviadas.AddRange(bebidas);
                        retorno.ImpressorasComProblema += impressora.Local + "\n";
                        retorno.Erros.Add(ex);
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }
            return retorno;
        }

        #endregion

        private string GerarRodape(bool reimpressao, int numImpressoras)
        {
            string rodape = DateTime.Now.ToString() + " - Impressão {0} de " + numImpressoras;
            if (reimpressao) rodape += "\r\n************* REIMPRESSÃO *************";
            return rodape;
        }

        public RetornoImpressaoBebidas ReimprimirBebidas(Pedido pedido, Mesa mesa, LocalInterno localInterno)
        {
            if (pedido is PedidoExterno && !Configuracoes.ObterInstancia().ImprimirLanchesPedidoExterno)
                return null;

            return ImprimirBebidas(pedido, pedido.BebidasDoPedido.Where(b => b.Enviado), mesa, localInterno, true);
        }

        public RetornoImpressaoRefeicoes ReimprimirRefeicoes(Pedido pedido, Mesa mesa, LocalInterno localInterno)
        {
            if (pedido is PedidoExterno && !Configuracoes.ObterInstancia().ImprimirLanchesPedidoExterno)
                return null;

            return ImprimirRefeicoes(pedido, pedido.RefeicoesDoPedido.Where(r => r.Enviado), mesa, localInterno, true);
        }

        public bool ImprimirFechamentoPedido(Pedido pedido, decimal? valorRecebido, bool imprimirCopiaFechamentoImprEntrega)
        {
            if (pedido == null) throw new ArgumentNullException("pedido");

            List<Impressora> impressoras = new List<Impressora>
            {
                ObterImpressoraDoCaixa()
            };

            //Se for pedido externo verificamos se é necessário imprimir uma cópia do ticket de fechamento na impressora da entrega
            if (pedido is PedidoExterno && imprimirCopiaFechamentoImprEntrega)
            {
                Impressora imprEntrega = ObterImpressoraDeEntrega();
                if (imprEntrega != null)
                    impressoras.Add(imprEntrega);
            }

            bool imprimiu = false;
            foreach (var impr in impressoras)
            {
                ITipoImpressoraProWaiter tipoImpressora = TiposImpressoras[impr.TipoImpressao];
                lock (tipoImpressora)
                {
                    try
                    {
                        tipoImpressora.ConectarImpressora(impr);
                        if (pedido is PedidoInterno)
                        {
                            PedidoInterno pedInterno = (PedidoInterno)pedido;
                            imprimiu = tipoImpressora.ImprimirTodoPedido((PedidoInterno)pedido, pedInterno.Mesa, valorRecebido) && tipoImpressora.CortarPapel();
                            if (!imprimiu)
                                break;
                        }
                        else if (pedido is PedidoExterno)
                        {                            
                            imprimiu = tipoImpressora.ImprimirTodoPedido((PedidoExterno)pedido, valorRecebido) && tipoImpressora.CortarPapel();
                            if (!imprimiu)
                                break;
                        }
                        else
                        {
                            imprimiu = tipoImpressora.ImprimirTodoPedido((PedidoParaLevar)pedido, valorRecebido) && tipoImpressora.CortarPapel();
                            if (!imprimiu)
                                break;
                        }
                    }
                    finally { try { tipoImpressora.DeconectarImpressora(); } catch { } }
                }
            }

            return imprimiu;
        }

        private Impressora ObterImpressoraDoCaixa()
        {
            using (ProWaiterContext db = new ProWaiterContext())
            {
                Impressora impDoCaixa = db.Impressoras.FirstOrDefault(i => i.EhDoCaixa);
                if (impDoCaixa == null)
                    throw new ApplicationException("Não foi encontrada uma impressora cadastrada no caixa!");
                return impDoCaixa;
            }
        }

        private Impressora ObterImpressoraDeEntrega()
        {
            using (ProWaiterContext db = new ProWaiterContext())
            {
                Impressora impDeEntrega = db.Impressoras.FirstOrDefault(i => i.EhDeEntrega);
                if (impDeEntrega == null)
                {
                    impDeEntrega = db.Impressoras.FirstOrDefault(i => i.EhDoCaixa);
                    if (impDeEntrega == null)
                        throw new ApplicationException("Não foi encontrada uma impressora cadastrada na entrega e nem no caixa!");
                }
                return impDeEntrega;
            }
        }

        public bool TestarImpressora(ITipoImpressoraProWaiter tipoImpressora)
        {
            switch (tipoImpressora.ObterStatusImpressora())
            {
                case eStatusImpressora.Normal: return true;
                case eStatusImpressora.TampaAberta:
                    throw new ApplicationException("A impressora " + tipoImpressora.ImpressoraConectada.Nome + " está com a tampa aberta!");
                case eStatusImpressora.SemPapel:
                    throw new ApplicationException("A impressora " + tipoImpressora.ImpressoraConectada.Nome + " está sem papel!");
                case eStatusImpressora.Desconhecido:
                    throw new ApplicationException("A impressora " + tipoImpressora.ImpressoraConectada.Nome + " está com um erro desconhecido!");
            }
            return false;
        }
    }
}