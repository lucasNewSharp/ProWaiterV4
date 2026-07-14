using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProWaiter.Web.Models
{
    public class ConfiguracoesViewModel
    {
        public SelectList ListaEstados { get; set; }
        public SelectList ListaCidade { get; set; }

        public ConfiguracoesViewModel()
        {
            Configuracoes config = Configuracoes.ObterInstancia();
            UtilizaComanda = config.UtilizaComanda;
            RequerObservacaoAoAbrirPedidoInterno = config.RequerObservacaoAoAbrirPedidoInterno;
            ImprimirNomeGarcomTicket = config.ImprimirNomeGarcomTicket;
            ImprimirLanchesPedidoExterno = config.ImprimirLanchesPedidoExterno;
            CodCidade = config.CodCidadePadrao;
            TextoFinalCupomFechamento = config.TextoFinalCupomFechamento;
            ImprimirTextoCupomFechamentoInterno = config.ImprimirTextoCupomFechamentoInterno;
            ImprimirTextoCupomFechamentoTeleEntrega = config.ImprimirTextoCupomFechamentoTeleEntrega;
            ImprimirCopiaFechamentoImpressoraEntrega = config.ImprimirCopiaFechamentoImpressoraEntrega;
            ImprimirSequencialFechamentoPedidoEntrega = config.ImprimirSequencialFechamentoPedidoEntrega;
            ExibirAdicionaisMolhosPedidoEntrega = config.ExibirAdicionaisMolhosPedidoEntrega;
            ImprimirHorarioGrandePedidoEntrega = config.ImprimirHorarioGrandePedidoEntrega;
        }

        [Display(Name = "Utiliza comanda")]
        public bool UtilizaComanda { get; set; }
        [Display(Name = "Requer observações ao abrir pedido interno")]
        public bool RequerObservacaoAoAbrirPedidoInterno { get; set; }
        [Display(Name = "Imprimir nome do garçom no ticket")]
        public bool ImprimirNomeGarcomTicket { get; set; }
        [Display(Name = "Imprimir refeições e bebidas para pedido externo")]
        public bool ImprimirLanchesPedidoExterno { get; set; }
        [Display(Name = "Cidade")]        
        public int CodCidade { get; set; }
 		[Display(Name = "Texto final cupom fechamento")]
        public string TextoFinalCupomFechamento { get; set; }
        [Display(Name = "Imprimir texto cupom de fechamento pedido interno")]
        public bool ImprimirTextoCupomFechamentoInterno { get; set; }
        [Display(Name = "Imprimir texto cupom de fechamento tele-entrega")]
        public bool ImprimirTextoCupomFechamentoTeleEntrega { get; set; }
        [Display(Name = "Imprimir uma cópia do ticket de fechamento na impressora de entrega para pedidos de entrega")]
        public bool ImprimirCopiaFechamentoImpressoraEntrega { get; set; }
        [Display(Name = "Imprimir número sequencial do dia no tiket de fechamento de pedido para entrega")]
        public bool ImprimirSequencialFechamentoPedidoEntrega { get; set; }
        [Display(Name = "Exibir adicionais de molho para entrega (Catchup, Mostarda e Maionese)")]
        public bool ExibirAdicionaisMolhosPedidoEntrega { get; set; }        
        [Display(Name = "Imprimri horário geração do pedido com fonte maior no pedido entrega")]
        public bool ImprimirHorarioGrandePedidoEntrega { get; set; }
    }
}