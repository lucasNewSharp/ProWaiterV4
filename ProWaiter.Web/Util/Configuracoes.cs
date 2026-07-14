using Newtonsoft.Json;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.Gestores;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ProWaiter.Web.Util
{
    /*
     * 1º Criar propriedade para a configuração
     * 2º Carregar a propriedade no método "RecarregarConfiguracoes()"
     * 3º Adicionar na View e controler das configurações
     * */

    class VersaoAPP
    {
        public VersaoAPP() { }

        public string VersionCode { get; set; }
    }

    public class Configuracoes
    {

        #region Singleton

        private static Configuracoes _configuracoes = null;
        private Configuracoes()
        {
            RecarregarConfiguracoes();
        }

        public static Configuracoes ObterInstancia()
        {
            if (_configuracoes == null)
                _configuracoes = new Configuracoes();
            return _configuracoes;
        }

        #endregion

        private const string CodVersao = "Versao";
        public string Versao { get; private set; }

        private const string CodValidador = "Validador";
        public string Validador { get; private set; }

        private const string CodArquivoRestaurante = "ArquivoRestaurante";
        public string ArquivoRestaurante { get; private set; }

        private const string CodTiposImpressoras = "TiposImpressoras";
        public string TiposImpressoras { get; private set; }

        private const string CodPastaArquivosImpressao = "PastaArquivosImpressao";
        public string PastaArquivosImpressao { get; private set; }

        public const string CodCodCidadePadrao = "CodCidadePadrao";
        public int CodCidadePadrao { get; private set; }

        public const string CodUtilizaComanda = "UtilizaComanda";
        public string StringTipoLocalEntrega { get; private set; }
        public bool UtilizaComanda { get; set; }

        public const string CodRequerObservacaoAoAbrirPedidoInterno = "RequerObservacaoAoAbrirPedidoInterno";
        public bool RequerObservacaoAoAbrirPedidoInterno { get; private set; }

        public const string CodImprimirNomeGarcomTiket = "ImprimirNomeGarcomTicket";
        public bool ImprimirNomeGarcomTicket { get; private set; }

        public const string CodImprimirLanchesPedidoExterno = "ImprimirLanchesPedidoExterno";
        public bool ImprimirLanchesPedidoExterno { get; private set; }

        public string VersaoAPP { get; private set; }
        public const string CodTextoFinalCupomFechamento = "TextoFinalCupomFechamento";
        public string TextoFinalCupomFechamento { get; private set; }

        public const string CodImprimirTextoCupomFechamentoInterno = "ImprimirTextoCupomFechamentoInterno";
        public bool ImprimirTextoCupomFechamentoInterno { get; private set; }

        public const string CodImprimirTextoCupomFechamentoTeleEntrega = "ImprimirTextoCupomFechamentoTeleEntrega";
        public bool ImprimirTextoCupomFechamentoTeleEntrega { get; private set; }

        public const string CodImprimirCopiaFechamentoImpressoraEntrega = "ImprimirCopiaFechamentoImpressoraEntrega";
        public bool ImprimirCopiaFechamentoImpressoraEntrega { get; private set; }

        public const string CodImprimirSequencialFechamentoPedidoEntrega = "ImprimirSequencialFechamentoPedidoEntrega";
        public bool ImprimirSequencialFechamentoPedidoEntrega { get; private set; }

        public const string CodImprimirHorarioGrandePedidoEntrega = "ImprimirHorarioGrandePedidoEntrega";
        public bool ImprimirHorarioGrandePedidoEntrega { get; private set; }

        public const string CodExibirAdicionaisMolhosPedidoEntrega = "ExibirAdicionaisMolhosPedidoEntrega";
        public bool ExibirAdicionaisMolhosPedidoEntrega { get; private set; }

        public const string CodPortaCOMIntegracaoICBox = "PortaCOMIntegracaoICBox";
        public string PortaCOMIntegracaoICBox { get; private set; }

        public void RecarregarConfiguracoes()
        {            
            using(StreamReader sr = File.OpenText(HostingEnvironment.MapPath("~/ProWaiterAPK/Versao.json")))
            {
                JsonSerializer serializer = new JsonSerializer();
                VersaoAPP verApp = (VersaoAPP)serializer.Deserialize(sr, typeof(VersaoAPP));
                VersaoAPP = verApp.VersionCode;
            }

            GestoresEntidades gEnt = new GestoresEntidades(new ProWaiterContextBDProvider());
            gEnt.ContextoBDProvider.IniciarContextoBD();
            try
            {
                List<Configuracao> configs = gEnt.gConfiguracoes.ObterEntidades().ToList();
                foreach(var conf in configs)
                {
                    switch (conf.Codigo)
                    {
                        case CodVersao:
                            Versao = conf.Valor;
                            break;
                        case CodValidador:
                            Validador = conf.Valor;
                            break;
                        case CodArquivoRestaurante:
                            ArquivoRestaurante = conf.Valor;
                            break;
                        case CodTiposImpressoras:
                            TiposImpressoras = conf.Valor;
                            break;
                        case CodPastaArquivosImpressao:
                            PastaArquivosImpressao = conf.Valor;
                            break;
                        case CodCodCidadePadrao:
                            CodCidadePadrao = int.Parse(conf.Valor);
                            break;
                        case CodUtilizaComanda:
                            UtilizaComanda = bool.Parse(conf.Valor);
                            StringTipoLocalEntrega = UtilizaComanda ? "Comandas" : "Mesas";
                            break;
                        case CodRequerObservacaoAoAbrirPedidoInterno:
                            RequerObservacaoAoAbrirPedidoInterno = bool.Parse(conf.Valor);
                            break;
                        case CodImprimirNomeGarcomTiket:
                            ImprimirNomeGarcomTicket = bool.Parse(conf.Valor);
                            break;
                        case CodImprimirLanchesPedidoExterno:
                            ImprimirLanchesPedidoExterno = bool.Parse(conf.Valor);
                            break;
                        case CodTextoFinalCupomFechamento:
                            TextoFinalCupomFechamento = conf.Valor;
                            break;
                        case CodImprimirTextoCupomFechamentoInterno:
                            ImprimirTextoCupomFechamentoInterno = bool.Parse(conf.Valor);
                            break;
                        case CodImprimirTextoCupomFechamentoTeleEntrega:
                            ImprimirTextoCupomFechamentoTeleEntrega = bool.Parse(conf.Valor);
                            break;
                        case CodImprimirCopiaFechamentoImpressoraEntrega:
                            ImprimirCopiaFechamentoImpressoraEntrega = bool.Parse(conf.Valor);
                            break;
                        case CodImprimirSequencialFechamentoPedidoEntrega:
                            ImprimirSequencialFechamentoPedidoEntrega = bool.Parse(conf.Valor);
                            break;
                        case CodExibirAdicionaisMolhosPedidoEntrega:
                            ExibirAdicionaisMolhosPedidoEntrega = bool.Parse(conf.Valor);
                            break;
                        case CodPortaCOMIntegracaoICBox:
                            PortaCOMIntegracaoICBox = conf.Valor;
                            break;
                        case CodImprimirHorarioGrandePedidoEntrega:
                            ImprimirHorarioGrandePedidoEntrega = bool.Parse(conf.Valor);
                            break;
                    }
                }
            }
            finally
            {
                gEnt.ContextoBDProvider.FinalizarContextoBD();
            }
        }
    
        public string ObterUltimoTelefoneDetectado(ProWaiterContext db)
        {            
            Configuracao config = db.Configuracoes.Where(c => c.Codigo == "UltimoTelefoneDetectado").SingleOrDefault();

            if (config == null)
                return string.Empty;

            return config.Valor;
        }
    
    }
}