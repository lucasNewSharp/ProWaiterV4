using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using ProWaiter.Web.Models;
using Microsoft.Ajax.Utilities;
using ProWaiter.Web.Models.DTOs;
using System.Diagnostics;
using ProWaiter.Web.Models.Gestores;
using System.Net.Configuration;

namespace ProWaiter.Web.Controllers
{
    public class PedidosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        #region Index

        private const string TipoParaLevar = "Para Levar";
        private const string TipoInterno = "Interno";
        private const string TipoExterno = "Externo";

        List<object> statusEnviado = new List<object>
            {
                new { Codigo = "", Nome = "Sim / Não" },
                new { Codigo = "True", Nome = "Sim" },
                new { Codigo = "False", Nome = "Não" }
            };

        List<object> statusFechado = new List<object>
            {
                new { Codigo = "", Nome = "Sim / Não" },
                new { Codigo = "True", Nome = "Sim" },
                new { Codigo = "False", Nome = "Não" }
            };

        List<object> tiposPedido = new List<object>
            {
                new { Codigo = "", Nome = "Todos" },
                new { Codigo = TipoInterno, Nome = TipoInterno },
                new { Codigo = TipoParaLevar, Nome = TipoParaLevar },
                new { Codigo = TipoExterno, Nome = TipoExterno }
            };


        // GET: PedidosExternos
        public ActionResult Index()
        {
            ViewBag.Erro = string.Empty;
            SelectList slStatus = null;
            SelectList slStatusFechado = null;
            SelectList slTiposPedido = null;
            List<Pedido> pedidos = null;

            if (Request.Cookies["PedidosIndex"] != null)
            {
                var c = Request.Cookies["PedidosIndex"];
                string filtro = c.Values["filtro"];
                string enviado = c.Values["enviado"];
                string fechado = c.Values["fechado"];
                string tipo = c.Values["tipo"];

                DateTime? dataInicio = string.IsNullOrWhiteSpace(c.Values["dataInicio"]) ? (DateTime?)null : DateTime.Parse(c.Values["dataInicio"]);

                pedidos = ObterPedidos(filtro,
                    tipo,
                    string.IsNullOrWhiteSpace(fechado) ? (bool?)null : bool.Parse(fechado),
                    string.IsNullOrEmpty(enviado) ? (bool?)null : bool.Parse(enviado),
                    dataInicio);

                slStatus = new SelectList(statusEnviado, "Codigo", "Nome", enviado);
                slStatusFechado = new SelectList(statusFechado, "Codigo", "Nome", fechado);
                slTiposPedido = new SelectList(tiposPedido, "Codigo", "Nome", tipo);
                ViewBag.Filtro = filtro;
                ViewBag.DataInicio = dataInicio.HasValue ? dataInicio.Value.ToString("yyyy-MM-dd") : string.Empty;
            }
            else
            {

                DateTime dtInicio = DateTime.Today;
                ViewBag.DataInicio = dtInicio.ToString("yyyy-MM-dd");
                slStatus = new SelectList(statusEnviado, "Codigo", "Nome", "");
                slStatusFechado = new SelectList(statusFechado, "Codigo", "Nome", "");
                slTiposPedido = new SelectList(tiposPedido, "Codigo", "Nome", "");
                pedidos = ObterPedidos("", "", null, null, dtInicio);
            }

            string exibirFiltros = "1";
            if (Request.Cookies["ExibirFiltros"] != null)
            {
                var c = Request.Cookies["ExibirFiltros"];
                exibirFiltros = c.Values["exibirFiltros"];
            }

            ViewBag.ExibirFiltros = exibirFiltros;
            ViewBag.StatusEnviado = slStatus;
            ViewBag.SatusFechado = slStatusFechado;
            ViewBag.TiposPedido = slTiposPedido;
            ViewBag.MesasDosPedidos = ObterMesasDoPedidos();
            ViewBag.TotalPedidos = pedidos.Sum(p => p.ValorTotal);
            ViewBag.UtilizaComanda = Configuracoes.ObterInstancia().UtilizaComanda;
            ViewBag.ExibirSequencialTeleEntrega = Configuracoes.ObterInstancia().ImprimirSequencialFechamentoPedidoEntrega;
            return View(pedidos);
        }

        [HttpPost]        
        public ActionResult Index(string filtro, string tipo, bool? fechado, bool? enviado, DateTime? dataInicio, bool? liberarConsulta)
        {
            ViewBag.Erro = string.Empty;
            ViewBag.StatusEnviado = new SelectList(statusEnviado, "Codigo", "Nome");
            ViewBag.SatusFechado = new SelectList(statusFechado, "Codigo", "Nome");
            ViewBag.TiposPedido = new SelectList(tiposPedido, "Codigo", "Nome");

            HttpCookie c = new HttpCookie("PedidosIndex");
            c.Values["filtro"] = filtro;
            c.Values["enviado"] = (enviado.HasValue ? enviado.Value.ToString() : "");
            c.Values["fechado"] = (fechado.HasValue ? fechado.Value.ToString() : "");
            c.Values["dataInicio"] = dataInicio.ToString();
            c.Values["tipo"] = tipo;            
            HttpContext.Response.Cookies.Set(c);

            string exibirFiltros = "1";
            if (Request.Cookies["ExibirFiltros"] != null)
            {
                var cook = Request.Cookies["ExibirFiltros"];
                exibirFiltros = cook.Values["exibirFiltros"];
            }

            ViewBag.ExibirFiltros = exibirFiltros;
            ViewBag.Filtro = filtro;
            ViewBag.DataInicio = dataInicio.HasValue ? dataInicio.Value.ToString("yyyy-MM-dd") : string.Empty;
            ViewBag.MesasDosPedidos = ObterMesasDoPedidos();

            List<Pedido> pedidos = new List<Pedido>();
            if (dataInicio.HasValue && dataInicio.Value < DateTime.Today.AddDays(-3) && liberarConsulta.HasValue && !liberarConsulta.Value)
            {
                ViewBag.Erro = "A data de início não pode ser menor do que " + DateTime.Today.AddDays(-3).ToShortDateString() + ". Abaixo apenas pedidos abertos estão sendo listados.";                
                //Nesse caso de bloqueio de período, listamos somente pedidos em aberto, pois pode ter ocorrido de ter uma mesa em aberto em uma data anterior
                pedidos = ObterPedidos(filtro, tipo, fechado, enviado, dataInicio, true);
            }
            else
            {
                pedidos = ObterPedidos(filtro, tipo, fechado, enviado, dataInicio);
            }
            ViewBag.TotalPedidos = pedidos.Sum(p => p.ValorTotal);
            ViewBag.UtilizaComanda = Configuracoes.ObterInstancia().UtilizaComanda;
            ViewBag.ExibirSequencialTeleEntrega = Configuracoes.ObterInstancia().ImprimirSequencialFechamentoPedidoEntrega;
            return View(pedidos);
        }

        [HttpPost]
        public ActionResult SalvarCoockieExibirFiltros(string exibirFiltros)
        {
            HttpCookie c = new HttpCookie("ExibirFiltros");
            c.Values["exibirFiltros"] = exibirFiltros;
            HttpContext.Response.Cookies.Set(c);
            return Json(string.Empty);
        }

        private List<Pedido> ObterPedidos(string filtro, string tipo, bool? exibirPedidosFechados, bool? enviados, DateTime? dataInicio, bool somenteAbertos = false)
        {
            DateTime dtIni = DateTime.Now;
            IQueryable<PedidoExterno> pedidosExternos = db.PedidosExternos;
            IQueryable<PedidoInterno> pedidosInternos = db.PedidosInternos;
            IQueryable<PedidoParaLevar> pedidosParaLevar = db.PedidosParaLevar;
            List<Pedido> pedidosFiltrados = new List<Pedido>();

            if (somenteAbertos)
            {
                pedidosExternos = pedidosExternos.Where(p => !p.DataTermino.HasValue);
                pedidosInternos = pedidosInternos.Where(p => !p.DataTermino.HasValue);
                pedidosParaLevar = pedidosParaLevar.Where(p => !p.DataTermino.HasValue);

                pedidosFiltrados = pedidosExternos
                    .Cast<Pedido>().ToList()
                    .Union(pedidosInternos.Cast<Pedido>().ToList())
                    .Union(pedidosParaLevar.Cast<Pedido>().ToList())
                    .Union(pedidosExternos.Cast<Pedido>().ToList())
                    .ToList();

                return pedidosFiltrados.OrderBy(p => p.DataInicio).ToList();
            }

            if (tipo == TipoExterno || string.IsNullOrWhiteSpace(tipo))
            {
                pedidosExternos = db.PedidosExternos
                    .Include(p => p.Cliente);
            }

            if (tipo == TipoInterno || string.IsNullOrWhiteSpace(tipo))
            {
                pedidosInternos = db.PedidosInternos.Include(p => p.Mesa);
                if (Configuracoes.ObterInstancia().UtilizaComanda)
                {
                    pedidosInternos = pedidosInternos.Include(p => p.LocalInterno);
                }
            }

            if (tipo == TipoParaLevar || string.IsNullOrWhiteSpace(tipo))
            {
                pedidosParaLevar = db.PedidosParaLevar;
            }

            if (exibirPedidosFechados.HasValue)
            {
                if (exibirPedidosFechados.Value)
                {
                    pedidosExternos = pedidosExternos.Where(p => p.DataTermino.HasValue);
                    pedidosInternos = pedidosInternos.Where(p => p.DataTermino.HasValue);
                    pedidosParaLevar = pedidosParaLevar.Where(p => p.DataTermino.HasValue);
                }
                else
                {
                    pedidosExternos = pedidosExternos.Where(p => !p.DataTermino.HasValue);
                    pedidosInternos = pedidosInternos.Where(p => !p.DataTermino.HasValue);
                    pedidosParaLevar = pedidosParaLevar.Where(p => !p.DataTermino.HasValue);
                }
            }

            if (!string.IsNullOrEmpty(filtro))
            {
                if (short.TryParse(filtro, out short sequencia))
                {
                    pedidosExternos = pedidosExternos.Where(p => p.Cliente.Nome.Contains(filtro) || p.NomeUsuario.Contains(filtro) || p.SequencialNoDia == sequencia);
                }
                else
                {
                    pedidosExternos = pedidosExternos.Where(p => p.Cliente.Nome.Contains(filtro) || p.NomeUsuario.Contains(filtro));
                }

                pedidosInternos = pedidosInternos.Where(p => p.NomeUsuario.Contains(filtro) || db.Mesas.Any(m => m.CodUltimoPedido == p.Codigo && m.Descricao.Contains(filtro)));
                pedidosParaLevar = pedidosParaLevar.Where(p => p.NomeUsuario.Contains(filtro) || db.Mesas.Any(m => m.CodUltimoPedido == p.Codigo && m.Descricao.Contains(filtro)));
            }

            if (dataInicio.HasValue)
            {
                pedidosExternos = pedidosExternos.Where(p => p.DataInicio >= dataInicio.Value);
                pedidosInternos = pedidosInternos.Where(p => p.DataInicio >= dataInicio.Value);
                pedidosParaLevar = pedidosParaLevar.Where(p => p.DataInicio >= dataInicio.Value);
            }
            
            if (!enviados.HasValue)
                pedidosFiltrados = pedidosExternos
                    .Cast<Pedido>().ToList()
                    .Union(pedidosInternos.Cast<Pedido>().ToList())
                    .Union(pedidosParaLevar.Cast<Pedido>().ToList())
                    .ToList();
            else
                pedidosFiltrados = pedidosExternos
                    .Cast<Pedido>().ToList()
                    .Union(pedidosInternos.Cast<Pedido>().ToList())
                    .Union(pedidosParaLevar.Cast<Pedido>().ToList())
                    .Where(p => p.TodosItensEnviados == enviados.Value)
                    .ToList();

            TimeSpan tempo = DateTime.Now.Subtract(dtIni);
            Debug.WriteLine(tempo);
            return pedidosFiltrados.OrderByDescending(p => p.DataTermino).ThenBy(p => p.DataInicio).ToList();
        }

        private Dictionary<int, string> ObterMesasDoPedidos()
        {
            var mesas = db.Mesas.Where(m => m.CodUltimoPedido.HasValue);
            Dictionary<int, string> retorno = new Dictionary<int, string>();
            foreach (Mesa m in mesas)
                retorno.Add(m.CodUltimoPedido.Value, m.Descricao);
            return retorno;
        }

        #endregion

        #region Edit

        [HttpPost]
        public ActionResult SalvarCoockieEditarPedido(bool marcarCodBarrasEnviado, bool exibirSomenteItensBalcaoSemCodBarras)
        {
            HttpCookie c = new HttpCookie("CookieEditarPedido");
            c.Values["marcarCodBarrasEnviado"] = marcarCodBarrasEnviado.ToString();
            c.Values["exibirSomenteItensBalcaoSemCodBarras"] = exibirSomenteItensBalcaoSemCodBarras.ToString();
            HttpContext.Response.Cookies.Set(c);
            return Json(string.Empty);
        }

        // GET: Pedidos/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Edit(int? id, string nomeModalAberta = "", int? codEnderecoSelecionado = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            if (Request.Cookies["CookieEditarPedido"] == null)
            {
                SalvarCoockieEditarPedido(false, false);
            }

            //var cook = Request.Cookies["CookieEditarPedido"];
            //if (cook != null)
            //{
            //    ViewBag.MarcarCodBarrasComoEnviada = bool.Parse(cook.Values["marcarCodBarrasEnviado"].ToString());
            //    ViewBag.ExibirSomenteItensBalcaoSemCodBarras = bool.Parse(cook.Values["exibirSomenteItensBalcaoSemCodBarras"].ToString());
            //}

            PopularViewBagParaEdicao(pedido, codEnderecoSelecionado);
            ViewBag.NomeModalAberta = nomeModalAberta;
            ViewBag.ErrosImpressao = TempData["ErrosImpressao"];
            return View(pedido);
        }

        private void PopularViewBagParaEdicao(Pedido pedido, int? codEnderecoSelecionado = null)
        {
            bool exibirSomenteItensBalcaoSemCodigoBarras = false;
            var cook = Request.Cookies["CookieEditarPedido"];
            if(cook != null)
            {
                ViewBag.MarcarCodBarrasComoEnviada = bool.Parse(cook.Values["marcarCodBarrasEnviado"].ToString());
                ViewBag.ExibirSomenteItensBalcaoSemCodBarras = bool.Parse(cook.Values["exibirSomenteItensBalcaoSemCodBarras"].ToString());
                exibirSomenteItensBalcaoSemCodigoBarras = bool.Parse(cook.Values["exibirSomenteItensBalcaoSemCodBarras"]);
            }

            ViewBag.CodTipoRefeicao = new SelectList(db.RefeicoesDoCardapio.Select(r => r.Refeicao.Tipo).Distinct().OrderBy(t => t.Posicao), "Codigo", "Nome");
            ViewBag.CodRefeicao = new SelectList(new RefeicaoDoCardapio[] { }, "Codigo", "Nome");
            ViewBag.CodTipoBebida = new SelectList(db.Bebidas.Where(b => b.Ativo).Select(b => b.Tipo).Distinct().OrderBy(t => t.Posicao), "Codigo", "Nome");
            ViewBag.CodBebida = new SelectList(new Bebida[] { }, "Codigo", "Nome");

            if (exibirSomenteItensBalcaoSemCodigoBarras)
                ViewBag.CodItensBalcao = new SelectList(db.ItensBacao.Where(i => i.Ativo && i.CodBarras == null).OrderBy(i => i.Nome), "Codigo", "Nome");
            else
                ViewBag.CodItensBalcao = new SelectList(db.ItensBacao.Where(i => i.Ativo).OrderBy(i => i.Nome), "Codigo", "Nome");

            if (pedido is PedidoInterno)
            {
                ViewBag.DescricaoMesa = db.Mesas.Where(m => m.CodUltimoPedido == pedido.Codigo).SingleOrDefault();
                if (Configuracoes.ObterInstancia().UtilizaComanda)
                {
                    List<LocalInterno> locaisInternos = db.LocaisInternos.OrderBy(l => l.Nome).ToList();
                    locaisInternos.Insert(0, new LocalInterno() { Codigo = 0, Nome = "Não selecionado" });
                    short? codSelecionado = ((PedidoInterno)pedido).CodLocalInterno;
                    ViewBag.CodLocalInterno = new SelectList(locaisInternos, "Codigo", "Nome", codSelecionado ?? 0);
                }
            }
            else if (pedido is PedidoExterno pedExterno)
            {
                ViewBag.NomeCliente = db.Clientes.Where(c => c.Codigo == pedExterno.CodCliente).Single().Nome;
                ViewBag.ValorEntrega = pedExterno.ValorEntrega;
                ViewBag.ImprimirCopiaImpressoraEntrega = Configuracoes.ObterInstancia().ImprimirCopiaFechamentoImpressoraEntrega;
                ViewBag.ExibirAdicionaisMolhosPedidoEntrega = Configuracoes.ObterInstancia().ExibirAdicionaisMolhosPedidoEntrega;
                if (!codEnderecoSelecionado.HasValue)
                    codEnderecoSelecionado = pedExterno.CodEnderecoEntrega;
                ViewBag.EnderecosEntrega = new SelectList(pedExterno.Cliente.Enderecos.ToList(), "Codigo", "Endereco", codEnderecoSelecionado.Value);
            }
            else
                ViewBag.NomeCliente = "Pedido Para Levar";

            ViewBag.RefeicoesCustomizadas = pedido.RefeicoesDoPedido.Where(r => !string.IsNullOrEmpty(r.Observacoes)).ToList();

            List<CodigoDescricaoDTO> modelos = new List<CodigoDescricaoDTO>();
            db.ModelosPedidos.OrderBy(m => m.Nome).ForEach(m => modelos.Add(new CodigoDescricaoDTO() { Codigo = m.Codigo, Descricao = m.Nome }));
            ViewBag.Modelos = modelos;
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult ReimprimirBebidas(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            LocalInterno localInterno = null;

            GestorImpressoes gImprs = GestorImpressoes.Instancia;
            Mesa mesa = null;
            if (pedido is PedidoInterno)
            {
                mesa = db.Mesas.Where(m => m.CodUltimoPedido == pedido.Codigo).SingleOrDefault();
                localInterno = ((PedidoInterno)pedido).LocalInterno;
            }

            if (!pedido.BebidasDoPedido.Any(b => b.Enviado))
            {
                TempData["ErrosImpressao"] = "Não existem bebidas já enviadas para serem reimpressas";
            }
            else
            {
                GestorImpressoes.RetornoImpressaoBebidas retImprBebidas = gImprs.ReimprimirBebidas(pedido, mesa, localInterno);
                AddErrosImpressaoBebidas(retImprBebidas, pedido);
            }


            if (pedido is PedidoInterno)
                return RedirectToAction("Details", "PedidosInternos", new { id = pedido.Codigo });
            if (pedido is PedidoExterno)
                return RedirectToAction("Details", "PedidosExternos", new { id = pedido.Codigo });
            return RedirectToAction("Details", "PedidosParaLevar", new { id = pedido.Codigo });
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult ReimprimirRefeicoes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            GestorImpressoes gImprs = GestorImpressoes.Instancia;
            Mesa mesa = null;
            LocalInterno localInterno = null;
            if (pedido is PedidoInterno)
            {
                mesa = db.Mesas.Where(m => m.CodUltimoPedido == pedido.Codigo).SingleOrDefault();
                localInterno = ((PedidoInterno)pedido).LocalInterno;
            }

            if (!pedido.RefeicoesDoPedido.Any(r => r.Enviado))
            {
                TempData["ErrosImpressao"] = "Não existem refeições já enviadas para serem reimpressas";
            }
            else
            {
                GestorImpressoes.RetornoImpressaoRefeicoes retImprRefeicoes = gImprs.ReimprimirRefeicoes(pedido, mesa, localInterno);
                AddErrosImpressaoRefeicoes(retImprRefeicoes, pedido);
            }

            if (pedido is PedidoInterno)
                return RedirectToAction("Details", "PedidosInternos", new { id = pedido.Codigo });
            if (pedido is PedidoExterno)
                return RedirectToAction("Details", "PedidosExternos", new { id = pedido.Codigo });
            return RedirectToAction("Details", "PedidosParaLevar", new { id = pedido.Codigo });
        }


        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult RemoverRefeicao(int codRefeicaoDoPedido)
        {
            RefeicaoDoPedido refDoPed = db.RefeicoesDoPedido.SingleOrDefault(r => r.Codigo == codRefeicaoDoPedido);
            if (refDoPed == null)
                return HttpNotFound();

            Pedido ped = db.Pedidos.Find(refDoPed.CodPedido);
            db.RefeicoesDoPedido.Remove(refDoPed);
            ped.RefeicoesDoPedido.Remove(refDoPed);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = ped.Codigo });
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult RemoverBebida(int codBebidaDoPedido)
        {
            BebidaDoPedido bebDoPed = db.BebidasDosPedidos.SingleOrDefault(r => r.Codigo == codBebidaDoPedido);
            if (bebDoPed == null)
                return HttpNotFound();
            Pedido ped = db.Pedidos.SingleOrDefault(p => p.Codigo == bebDoPed.CodPedido);
            if (ped == null)
                return HttpNotFound();

            db.BebidasDosPedidos.Remove(bebDoPed);
            ped.BebidasDoPedido.Remove(bebDoPed);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = ped.Codigo });
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult RemoverItemBalcao(int codItemBalcaoPedido)
        {
            ItemBalcaoDoPedido item = db.ItensBalcaoDoPedido.SingleOrDefault(i => i.Codigo == codItemBalcaoPedido);
            if (item == null)
                return HttpNotFound();
            Pedido ped = db.Pedidos.SingleOrDefault(p => p.Codigo == item.CodPedido);
            if (ped == null)
                return HttpNotFound();

            db.ItensBalcaoDoPedido.Remove(item);
            ped.ItensBalcaoDoPedido.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Edit", new { Id = ped.Codigo });
        }

        public ActionResult ObterRefeicoes(short codTipoRefeicao)
        {
            SelectList sl = new SelectList(db.RefeicoesDoCardapio
                .Where(r => r.Ativo && r.Refeicao.CodTipo == codTipoRefeicao)
                .Select(r => new
                {
                    Codigo = r.CodRefeicao.ToString() + ";" + r.CodTamanho.ToString(),
                    Nome = r.Refeicao.Nome + " (" + r.TamanhoRefeicao.Nome + ")"
                })
                .OrderBy(r => r.Nome).ToList(),
                "Codigo", "Nome");
            JsonResult res = Json(sl);
            return res;
        }

        [HttpPost]
        public ActionResult ObterComponentes(short codRefeicao, string codTamanho)
        {
            RefeicaoDoCardapio refDoCardapio = db.RefeicoesDoCardapio.SingleOrDefault(r => r.CodRefeicao == codRefeicao && r.CodTamanho == codTamanho);

            List<object> lista = new List<object>();

            if (refDoCardapio.DeComposicao)
            {
                lista = new List<object>(refDoCardapio.ComponentesComposicaoRefeicao.OrderBy(c => c.ComponenteRefeicao.Nome)
                   .Select(c => new
                   {
                       Codigo = c.CodComponente.ToString(),
                       Nome = c.ComponenteRefeicao.Nome,
                       PossuiUnidade = !string.IsNullOrWhiteSpace(c.CodUnidade)
                   }));
            }
            else
            {
                lista = new List<object>(refDoCardapio.Refeicao.ComponentesRefeicao.OrderBy(c => c.Nome)
                   .Select(c => new
                   {
                       Codigo = c.Codigo.ToString(),
                       Nome = c.Nome
                   }));
            }
            JsonResult res = Json(new { lista, refDoCardapio.DeComposicao });
            return res;
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarValorEntrega(int codPedido, decimal valorEntrega)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == codPedido).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            ((PedidoExterno)pedido).ValorEntrega = valorEntrega;
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AdicionarRefeicao(int codPedido, short codRefeicao, string codTamanho, int quantidade, List<ComponenteRefeicaoPedidoViewModel> codComponentes, string observacao)
        {
            Pedido pedido = db.Pedidos.SingleOrDefault(p => p.Codigo == codPedido);
            if (pedido == null)
                return HttpNotFound();

            AdicionarRefeicaoHelper(pedido, codRefeicao, codTamanho, quantidade, codComponentes, observacao);

            db.SaveChanges();

            return Json(string.Empty);
        }
        
        public ActionResult ObterBebidas(short codTipoBebida)
        {
            SelectList sl = new SelectList(db.Bebidas.Where(b => b.Ativo && b.CodTipo == codTipoBebida).OrderBy(b => b.Nome).ToList(), "Codigo", "Nome");
            JsonResult res = Json(sl);
            return res;
        }
        
        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult ObterItensBalcao(bool somenteItensBalcaoSemCodBarras)
        {
            IQueryable<ItemBalcao> query = db.ItensBacao.Where(i => i.Ativo);
            if (somenteItensBalcaoSemCodBarras)
                query = query.Where(i => i.CodBarras == null);

            SelectList sl = new SelectList(query.OrderBy(b => b.Nome).ToList(), "Codigo", "Nome");
            JsonResult res = Json(sl);
            return res;
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AdicionarBebida(int codPedido, short codBebida, int quantidade, string observacoes)
        {
            Pedido pedido = db.Pedidos.SingleOrDefault(p => p.Codigo == codPedido);
            if (pedido == null)
                return HttpNotFound();

            AdicionarBebidaHelper(pedido, codBebida, quantidade, observacoes);
            db.SaveChanges();

            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AdicionarItemBalcao(int codPedido, int codItemBalcao, int quantidade)
        {
            Pedido pedido = db.Pedidos.SingleOrDefault(p => p.Codigo == codPedido);
            if (pedido == null)
                return HttpNotFound();

            AdicionarItemBalcaoHelper(pedido, codItemBalcao, quantidade);
            db.SaveChanges();

            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AdicionarItemCodBarras(int codPedido, string codBarras, int quantidade, bool marcarCodBarrasEnviado)
        {
            Pedido pedido = db.Pedidos.SingleOrDefault(p => p.Codigo == codPedido);
            if (pedido == null)
                return HttpNotFound();

            IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(codBarras, false);
            if (item == null)
                return Json(new { ok = false });

            if (item is ItemBalcao itemBalcao)
                AdicionarItemBalcaoHelper(pedido, itemBalcao.Codigo, quantidade);
            else if (item is Bebida beb)
                AdicionarBebidaHelper(pedido, beb.Codigo, quantidade, null, marcarCodBarrasEnviado);
            else if (item is RefeicaoDoCardapio refe)
            {
                List<ComponenteRefeicaoPedidoViewModel> codComponentes = new List<ComponenteRefeicaoPedidoViewModel>();
                if (!refe.DeComposicao)
                {
                    refe.Refeicao.ComponentesRefeicao.ForEach(c => codComponentes.Add(new ComponenteRefeicaoPedidoViewModel()
                    {
                        CodComponente = c.Codigo,
                        Quantidade = 1
                    }));
                }
                AdicionarRefeicaoHelper(pedido, refe.CodRefeicao, refe.CodTamanho, quantidade, codComponentes, null, marcarCodBarrasEnviado);
            }

            db.SaveChanges();

            return Json(new { ok = true });
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarComponente(int codRefeicaoPedido, ComponenteRefeicaoPedidoViewModel componente, bool adicionar)
        {
            RefeicaoDoPedido refeicaoDoPedido = db.RefeicoesDoPedido.Where(r => r.Codigo == codRefeicaoPedido).SingleOrDefault();
            if (refeicaoDoPedido == null)
                return HttpNotFound();

            ComponenteRefeicaoPedido compRefExistente = refeicaoDoPedido.ComponentesRefeicaoPedido.Where(c => c.CodComponente == componente.CodComponente).SingleOrDefault();
            if (compRefExistente != null)
                refeicaoDoPedido.ComponentesRefeicaoPedido.Remove(compRefExistente);

            if (adicionar)
            {
                ComponenteRefeicaoPedido compRef = new ComponenteRefeicaoPedido()
                {
                    CodComponente = componente.CodComponente,
                    CodRefeicaoPedido = codRefeicaoPedido,
                    Quantidade = componente.Quantidade
                };
                refeicaoDoPedido.ComponentesRefeicaoPedido.Add(compRef);
            }

            refeicaoDoPedido.RecalcularValorRefeicao();
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        public ActionResult ObterStringToolTipComponentesSelecionados(int codRefeicaoPedido)
        {
            RefeicaoDoPedido refeicaoDoPedido = db.RefeicoesDoPedido.Where(r => r.Codigo == codRefeicaoPedido).SingleOrDefault();
            if (refeicaoDoPedido == null)
                return HttpNotFound();

            string tooltip = string.Empty;
            foreach (var comp in refeicaoDoPedido.ComponentesRefeicaoPedido)
            {
                tooltip += comp.ComponenteRefeicao.Nome + "(" + comp.Quantidade + ") <br />";
            }

            return Json(tooltip);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarObservacaoRefeicaoPedido(int codRefeicaoPedido, string observacao)
        {
            RefeicaoDoPedido refeicaoDoPedido = db.RefeicoesDoPedido.Where(r => r.Codigo == codRefeicaoPedido).SingleOrDefault();
            if (refeicaoDoPedido == null)
                return HttpNotFound();
            refeicaoDoPedido.Observacoes = observacao;
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarObservacaoBebidaPedido(int codBebidaPedido, string observacao)
        {
            BebidaDoPedido bebidaDoPedido = db.BebidasDosPedidos.Where(r => r.Codigo == codBebidaPedido).SingleOrDefault();
            if (bebidaDoPedido == null)
                return HttpNotFound();
            bebidaDoPedido.Observacoes = observacao;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
            }
            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarLocalEntregaPedidoInterno(int codPedido, short? codLocalInterno)
        {
            PedidoInterno pedido = db.PedidosInternos.Where(p => p.Codigo == codPedido).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            LocalInterno li = db.LocaisInternos.SingleOrDefault(l => l.Codigo == codLocalInterno);
            pedido.CodLocalInterno = li == null ? (short?)null : li.Codigo;
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarObservacaoPedido(int codPedido, string observacoes)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == codPedido).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            pedido.Observacoes = observacoes;
            db.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AtualizarEnderecoEntregaPedido(int codPedido, int codEndereco)
        {
            var objFalse = new { ok = false };

            PedidoExterno pedido = db.PedidosExternos.Where(p => p.Codigo == codPedido).SingleOrDefault();
            if (pedido == null)
                return Json(objFalse);

            EnderecoCliente endereco = db.EnderecosClientes.Where(e => e.Codigo == codEndereco).SingleOrDefault();
            if (endereco == null)
                return Json(objFalse);

            try
            {
                pedido.CodEnderecoEntrega = codEndereco;
                db.SaveChanges();
                return Json(new
                {
                    ok = true,
                    endereco.Endereco,
                    endereco.Bairro,
                    Cidade = endereco.Cidade?.Nome,
                    endereco.ValorEntregaPadrao,
                    endereco.ObservacoesPadrao
                });
            }
            catch
            {
                return Json(objFalse);
            }
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult EnviarItensACozinha(int Id)
        {
            GestorImpressoes.RetornoImpressaoBebidas retImprBebidas = null;
            GestorImpressoes.RetornoImpressaoRefeicoes retImprRefeicoes = null;

            Pedido pedido = db.Pedidos.SingleOrDefault(p => p.Codigo == Id);
            if (pedido == null)
                return HttpNotFound();

            Mesa mesa = null;
            LocalInterno localInterno = null;
            if (pedido is PedidoInterno)
            {
                mesa = db.Mesas.Where(m => m.CodUltimoPedido == pedido.Codigo).SingleOrDefault();
                localInterno = ((PedidoInterno)pedido).LocalInterno;
            }

            GestorImpressoes gImprs = GestorImpressoes.Instancia;

            List<BebidaDoPedido> bebidas = pedido.BebidasDoPedido.Where(b => !b.Enviado).ToList();
            List<RefeicaoDoPedido> refeicoes = pedido.RefeicoesDoPedido.Where(r => !r.Enviado).ToList();

            Exception excessao = null;
            db.IniciarTransacao();
            try
            {
                retImprBebidas = gImprs.ImprimirBebidas(pedido, bebidas, mesa, localInterno);

                foreach (BebidaDoPedido beb in retImprBebidas.BebidasEnviadas)
                    beb.Enviado = true;
                foreach (BebidaDoPedido beb in retImprBebidas.BebidasNaoEnviadas)
                    beb.Enviado = false;

                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.SetarRollBack();
                excessao = ex;
            }
            if (!db.FinalizarTransacao())
            {
                if (excessao != null)
                    throw excessao;
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Não foi possível enviar as bebidas do pedido " + pedido.Codigo + "!");
            }
            AddErrosImpressaoBebidas(retImprBebidas, pedido);

            db.IniciarTransacao();
            try
            {
                retImprRefeicoes = gImprs.ImprimirRefeicoes(pedido, refeicoes, mesa, localInterno);

                foreach (RefeicaoDoPedido refe in retImprRefeicoes.RefeicoesEnviadas)
                    refe.Enviado = true;
                foreach (RefeicaoDoPedido refe in retImprRefeicoes.RefeicoesNaoEnviadas)
                    refe.Enviado = false;

                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.SetarRollBack();
                excessao = ex;
            }
            if (!db.FinalizarTransacao())
            {
                if (excessao != null)
                    throw excessao;
                throw new HttpException((int)HttpStatusCode.InternalServerError, "Não foi possível enviar as refeições do pedido " + pedido.Codigo + "!");
            }
            AddErrosImpressaoRefeicoes(retImprRefeicoes, pedido);

            return RedirectToAction("Edit", new { Id = pedido.Codigo });
        }

        private void AddErrosImpressaoBebidas(GestorImpressoes.RetornoImpressaoBebidas retImprBebidas, Pedido pedido)
        {
            if (retImprBebidas != null && retImprBebidas.Erros.Count > 0)
            {
                string msg = "Não foi possível enviar todas as bebidas do pedido " + pedido.Codigo + "! para a(s) impressora(s). ";
                if (retImprBebidas != null && retImprBebidas.BebidasNaoEnviadas.Count > 0)
                    msg += " Problemas na(s) impressoras(s) " + retImprBebidas.ImpressorasComProblema + ". ";

                if (retImprBebidas != null && retImprBebidas.Erros.Count > 0)
                    msg += retImprBebidas.Erros.First().ToString();

                TempData["ErrosImpressao"] = msg;
            }
        }

        private void AddErrosImpressaoRefeicoes(GestorImpressoes.RetornoImpressaoRefeicoes retImprRefeicoes, Pedido pedido)
        {
            if (retImprRefeicoes != null && retImprRefeicoes.Erros.Count > 0)
            {
                string msg = " Não foi possível enviar todas as refeições do pedido " + pedido.Codigo + " para a(s) impressora(s)! ";
                if (retImprRefeicoes != null && retImprRefeicoes.RefeicoesNaoEnviadas.Count > 0)
                    msg += " Problemas na(s) impressoras(s) " + retImprRefeicoes.ImpressorasComProblema + ". ";

                if (retImprRefeicoes != null && retImprRefeicoes.Erros.Count > 0)
                {
                    Exception ex = retImprRefeicoes.Erros.FirstOrDefault();
                    if (ex != null)
                        msg += ex.ToString();
                }

                TempData["ErrosImpressao"] += msg;
            }
        }

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult FecharPedido(int Codigo,
            decimal? acrecimos,
            decimal? descontos,
            string acrescimosRefCustomizadas,
            decimal? valorRecebido,
            bool imprimirCopiaFechamentoImprEntrega,
            string observacoes,
            bool maionese,
            bool catchup,
            bool mostarda)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == Codigo).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            if (!(pedido is PedidoExterno) && (!pedido.TodosItensEnviados))
            {
                ModelState.AddModelError(string.Empty, "Este pedido não pode ser fechado pois ainda possui itens não enviados à cozinha/bar.");
                PopularViewBagParaEdicao(pedido);
                return View("Edit", pedido);
            }

            if (!string.IsNullOrEmpty(acrescimosRefCustomizadas))
            {
                foreach (string acresRefCust in acrescimosRefCustomizadas.Split(';'))
                {
                    var chaveValor = acresRefCust.Split(':');
                    var refeicao = pedido.RefeicoesDoPedido.SingleOrDefault(r => r.Codigo == int.Parse(chaveValor[0]));
                    if (refeicao != null)
                        refeicao.Acrescimo = decimal.Parse(chaveValor[1].Replace(",", "").Replace(".", ","));
                }
            }

            pedido.Acrescimos = acrecimos ?? 0;
            pedido.Descontos = descontos ?? 0;
            pedido.DataTermino = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(observacoes))
                pedido.Observacoes = observacoes;

            if (pedido is PedidoExterno pedidoExterno && Configuracoes.ObterInstancia().ExibirAdicionaisMolhosPedidoEntrega)
            {
                pedidoExterno.Maionese = maionese;
                pedidoExterno.Catchup = catchup;
                pedidoExterno.Mostarda = mostarda;
            }

            Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == pedido.Codigo).SingleOrDefault();
            if (mesa != null)
                mesa.UltimoPedido = null;

            db.SaveChanges();

            try
            {
                if (!GestorImpressoes.Instancia.ImprimirFechamentoPedido(pedido, valorRecebido, imprimirCopiaFechamentoImprEntrega))
                    throw new ApplicationException("Não foi possível imprimir o pedido no caixa ou na entrega!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "O pedido foi fechado mas não foi possível imprimi-lo no caixa ou na entrega!\r\n" + ex.Message);
                PopularViewBagParaEdicao(pedido);
                return View("Edit", pedido);
            }
            return RedirectToAction("Index", "Pedidos");
        }

        #endregion

        #region Helper Ajax

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult ObterValorAtualPedido(int codPedido)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == codPedido).SingleOrDefault();

            if (pedido == null)
                throw new ApplicationException("Não foi possível obter o valor do pedido");

            JsonResult res = Json(pedido.ValorTotal.ToString());
            return res;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Cadastro de Modelo

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult CadastrarModelo(int Codigo, decimal? acrecimos, decimal? descontos, string nomeModelo, bool excluirPedido)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == Codigo).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(nomeModelo))
            {
                ModelState.AddModelError(string.Empty, "O nome do modelo não pode ser vazio!\r\n");
                PopularViewBagParaEdicao(pedido);
                return View("Edit", pedido);
            }

            try
            {
                //Criamos o modelo
                ModeloPedido modeloPedido = new ModeloPedido()
                {
                    Acrescimo = acrecimos ?? 0,
                    Desconto = descontos ?? 0,
                    Nome = nomeModelo,
                    Observacoes = pedido.Observacoes.ValorOuNulo(),
                    ModelosBebidaPedido = new List<ModeloBebidaPedido>(),
                    ModelosRefeicaoPedidos = new List<ModeloRefeicaoPedido>()
                };

                //Criamos os modelos de bebida
                pedido.BebidasDoPedido.ForEach(b => modeloPedido.ModelosBebidaPedido.Add(new ModeloBebidaPedido()
                {
                    Bebida = b.Bebida,
                    CodBebida = b.CodBebida,
                    Observacoes = b.Observacoes.ValorOuNulo()
                }));

                //Criamos os modelos de refeicao com seus componentes
                foreach (var r in pedido.RefeicoesDoPedido)
                {
                    var mod = new ModeloRefeicaoPedido()
                    {
                        CodRefeicao = r.CodRefeicao,
                        RefeicaoDoCardapio = r.RefeicaoDoCardapio,
                        CodTamanho = r.CodTamanho,
                        Tamanho = r.Tamanho,
                        Observacoes = r.Observacoes.ValorOuNulo()
                    };

                    r.ComponentesRefeicaoPedido.ForEach(c => mod.ModeloComponentesRefeicaoPedido.Add(new ModeloComponenteRefeicaoPedido()
                    {
                        CodComponente = c.CodComponente,
                        ComponenteRefeicao = c.ComponenteRefeicao,
                        Quantidade = c.Quantidade,
                        ModeloRefeicaoDoPedido = mod
                    }));

                    modeloPedido.ModelosRefeicaoPedidos.Add(mod);
                };

                db.ModelosPedidos.Add(modeloPedido);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopularViewBagParaEdicao(pedido);
                return View("Edit", pedido);
            }

            try
            {
                if (excluirPedido)
                {
                    db = new ProWaiterContext();
                    pedido = db.Pedidos.Find(Codigo);
                    if (pedido is PedidoInterno)
                    {
                        Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == Codigo).SingleOrDefault();
                        if (mesa != null)
                            mesa.CodUltimoPedido = null;
                    }
                    db.Pedidos.Remove(pedido);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopularViewBagParaEdicao(pedido);
                return View("Edit", pedido);
            }

            return RedirectToAction("Index", "Pedidos");
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult AdicionarItensDoModelo(int Codigo, int codModelo)
        {
            Pedido pedido = db.Pedidos.Where(p => p.Codigo == Codigo).SingleOrDefault();
            if (pedido == null)
                return HttpNotFound();

            ModeloPedido mod = db.ModelosPedidos.Find(codModelo);
            if (mod == null)
                return HttpNotFound();

            pedido.Acrescimos += mod.Acrescimo;
            pedido.Descontos += mod.Desconto;
            pedido.Observacoes = mod.Observacoes.ValorOuNulo();

            foreach (var refeicaoModelo in mod.ModelosRefeicaoPedidos)
            {
                List<ComponenteRefeicaoPedidoViewModel> codComponentes = new List<ComponenteRefeicaoPedidoViewModel>();
                refeicaoModelo.ModeloComponentesRefeicaoPedido.ForEach(m => codComponentes.Add(new ComponenteRefeicaoPedidoViewModel()
                {
                    CodComponente = m.CodComponente,
                    CodRefeicaoPedido = m.ModeloRefeicaoDoPedido.CodRefeicao,
                    Quantidade = m.Quantidade
                }));

                AdicionarRefeicaoHelper(pedido, refeicaoModelo.CodRefeicao, refeicaoModelo.CodTamanho, 1, codComponentes, null);
            }

            foreach (var bebidaModelo in mod.ModelosBebidaPedido)
            {
                AdicionarBebidaHelper(pedido, bebidaModelo.CodBebida, 1, null);
            }

            db.SaveChanges();
            return Json(string.Empty);
            //PopularViewBagParaEdicao(pedido);
            //return View("Edit", pedido);            
        }

        #endregion

        private bool AdicionarRefeicaoHelper(Pedido pedido, short codRefeicao, string codTamanho, int quantidade, List<ComponenteRefeicaoPedidoViewModel> codComponentes, string observacao, bool marcarComoEnviado = false)
        {
            RefeicaoDoCardapio refDoCardapio = db.RefeicoesDoCardapio.SingleOrDefault(r => r.CodRefeicao == codRefeicao && r.CodTamanho == codTamanho);
            if (refDoCardapio == null)
                return false;

            bool enviado = marcarComoEnviado;
            if (pedido is PedidoExterno && !Configuracoes.ObterInstancia().ImprimirLanchesPedidoExterno)
            {
                enviado = true;
            }

            for (int i = 0; i < quantidade; i++)
            {
                List<ComponenteRefeicaoPedido> componentes = new List<ComponenteRefeicaoPedido>();
                if (codComponentes != null)
                    foreach (ComponenteRefeicaoPedidoViewModel comp in codComponentes)
                        componentes.Add(new ComponenteRefeicaoPedido()
                        {
                            CodComponente = comp.CodComponente,
                            Quantidade = comp.Quantidade,
                        });

                RefeicaoDoPedido refDoPedido = new RefeicaoDoPedido(pedido, refDoCardapio, componentes);
                refDoPedido.Observacoes = observacao;
                refDoPedido.NomeUsuario = User.Identity.Name;
                refDoPedido.DataHora = DateTime.Now;
                refDoPedido.Enviado = enviado;
                db.RefeicoesDoPedido.Add(refDoPedido);
            }

            return true;
        }

        private bool AdicionarBebidaHelper(Pedido pedido, short codBebida, int quantidade, string observacoes, bool marcarComoEnviado = false)
        {
            bool enviado = marcarComoEnviado;
            if (pedido is PedidoExterno && !Configuracoes.ObterInstancia().ImprimirLanchesPedidoExterno)
            {
                enviado = true;
            }

            Bebida bebida = db.Bebidas.SingleOrDefault(b => b.Codigo == codBebida);
            if (bebida == null)
                return false;

            for (int i = 0; i < quantidade; i++)
                db.BebidasDosPedidos.Add(new BebidaDoPedido(pedido, bebida) { Observacoes = observacoes, NomeUsuario = User.Identity.Name, DataHora = DateTime.Now, Enviado = enviado });

            return true;
        }

        private bool AdicionarItemBalcaoHelper(Pedido pedido, int codItemBalcao, int quantidade)
        {
            ItemBalcao item = db.ItensBacao.Where(i => i.Codigo == codItemBalcao).SingleOrDefault();
            if (item == null)
                return false;

            for (int i = 0; i < quantidade; i++)
                db.ItensBalcaoDoPedido.Add(new ItemBalcaoDoPedido(pedido, item) { NomeUsuario = User.Identity.Name, DataHora = DateTime.Now });

            return true;
        }
    }
}
