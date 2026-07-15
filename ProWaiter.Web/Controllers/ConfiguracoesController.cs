using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProWaiter.Web.Controllers
{
    public class ConfiguracoesController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: Configuracoes/Edit
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit()
        {
            var config = new ConfiguracoesViewModel();
            CarregarListaCidades(config);
            return View(config);
        }

        // POST: Configuracoes/Edit
        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(ConfiguracoesViewModel configuracoesDTO)
        {
            try
            {
                var requerObservacaoAoAbrirPedidoInterno = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodRequerObservacaoAoAbrirPedidoInterno).Single();
                requerObservacaoAoAbrirPedidoInterno.Valor = configuracoesDTO.RequerObservacaoAoAbrirPedidoInterno.ToString();

                var utilizaComanda = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodUtilizaComanda).Single();
                utilizaComanda.Valor = configuracoesDTO.UtilizaComanda.ToString();

                var imprimirNomeGarcomTicket = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirNomeGarcomTiket).Single();
                imprimirNomeGarcomTicket.Valor = configuracoesDTO.ImprimirNomeGarcomTicket.ToString();

                var imprimirLanchesPedidoExterno = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirLanchesPedidoExterno).Single();
                imprimirLanchesPedidoExterno.Valor = configuracoesDTO.ImprimirLanchesPedidoExterno.ToString();

                var codCidadePadrao = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodCodCidadePadrao).Single();
                codCidadePadrao.Valor = configuracoesDTO.CodCidade.ToString();

                var textoFinalCupomFechamento = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodTextoFinalCupomFechamento).Single();
                textoFinalCupomFechamento.Valor = string.IsNullOrWhiteSpace(configuracoesDTO.TextoFinalCupomFechamento) ? string.Empty : configuracoesDTO.TextoFinalCupomFechamento;

                var imprimirTextoCupomFechamentoInterno = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirTextoCupomFechamentoInterno).Single();
                imprimirTextoCupomFechamentoInterno.Valor = configuracoesDTO.ImprimirTextoCupomFechamentoInterno.ToString();

                var imprimirTextoCupomFechamentoTeleEntrega = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirTextoCupomFechamentoTeleEntrega).Single();
                imprimirTextoCupomFechamentoTeleEntrega.Valor = configuracoesDTO.ImprimirTextoCupomFechamentoTeleEntrega.ToString();

                var imprimirCopiaFechamentoImpressoraEntrega = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirCopiaFechamentoImpressoraEntrega).Single();
                imprimirCopiaFechamentoImpressoraEntrega.Valor = configuracoesDTO.ImprimirCopiaFechamentoImpressoraEntrega.ToString();

                var imprimirSequencialFechamentoPedidoEntrega = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirSequencialFechamentoPedidoEntrega).Single();
                imprimirSequencialFechamentoPedidoEntrega.Valor = configuracoesDTO.ImprimirSequencialFechamentoPedidoEntrega.ToString();

                var exibirAdicionaisMolhosPedidoEntrega = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodExibirAdicionaisMolhosPedidoEntrega).Single();
                exibirAdicionaisMolhosPedidoEntrega.Valor = configuracoesDTO.ExibirAdicionaisMolhosPedidoEntrega.ToString();

                var imprimirHorarioGrandePedidoEntrega = db.Configuracoes.Where(c => c.Codigo == Configuracoes.CodImprimirHorarioGrandePedidoEntrega).Single();
                imprimirHorarioGrandePedidoEntrega.Valor = configuracoesDTO.ImprimirHorarioGrandePedidoEntrega.ToString();

                

                db.SaveChanges();
                ViewBag.Mensagem = "Configurações salvas com sucesso, reinicie o APP dos dispositivos para aplicar as novas configurações.";
                ViewBag.EhErro = false;

                Configuracoes.ObterInstancia().RecarregarConfiguracoes();
                CarregarListaCidades(configuracoesDTO);

                return View(configuracoesDTO);
            }
            catch(Exception ex)            
            {
                ViewBag.EhErro = true;
                ViewBag.Mensagem = "Erro ao tentar salvar as configurações. " + ex.ToString();
                return View();
            }
        }

        private void CarregarListaCidades(ConfiguracoesViewModel config)
        {
            Cidade cidadeSelecionada = db.Cidades.Include(c => c.UF).Where(c => c.Codigo == config.CodCidade).Single();
            config.ListaEstados = new SelectList(db.UFs.OrderBy(u => u.Nome), "Codigo", "Nome", cidadeSelecionada.UF.Codigo);
            config.ListaCidade = new SelectList(db.Cidades.Where(c => c.CodUF == cidadeSelecionada.UF.Codigo).OrderBy(c => c.Nome).ToList(), "Codigo", "Nome", config.CodCidade);
        }

        [HttpPost]
        public ActionResult CarregarCidades(string uf)
        {
            SelectList sl = new SelectList(db.Cidades.Where(c => c.UF.Codigo == uf).OrderBy(c => c.Nome), "Codigo", "Nome");
            JsonResult res = Json(sl);
            return res;
        }
    }
}