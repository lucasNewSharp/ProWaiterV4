using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.Gestores;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProWaiter.Web.Controllers
{
    public class PedidosInternosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pedidos");
        }

        #region Details

        // GET: PedidosInternos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoInterno pedidoInterno = db.PedidosInternos.Find(id);
            if (pedidoInterno == null)
            {
                return HttpNotFound();
            }
            Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == id.Value).SingleOrDefault();
            ViewBag.Mesa = (mesa == null ? "Sem Mesa" : mesa.Descricao);
            ViewBag.ErrosImpressao = TempData["ErrosImpressao"];
            return View(pedidoInterno);
        }

        #endregion

        #region Create

        // GET: PedidosInternos/Create
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create()
        {
            ViewBag.Mesas = new SelectList(db.Mesas.Where(m => !m.CodUltimoPedido.HasValue).OrderBy(m => m.Descricao), "Codigo", "Descricao");
            if (Configuracoes.ObterInstancia().UtilizaComanda)
                AdicionarLocaisInternosViewBag();
            return View();
        }

        private void AdicionarLocaisInternosViewBag()
        {
            List<LocalInterno> locaisInternos = db.LocaisInternos.OrderBy(l => l.Nome).ToList();
            locaisInternos.Insert(0, new LocalInterno() { Codigo = 0, Nome = "Não selecionado" });
            ViewBag.LocaisInternos = new SelectList(locaisInternos, "Codigo", "Nome");
        }

        // POST: PedidosInternos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create(int? codMesa, short? codLocalInterno, string observacoes = null)
        {
            if (ModelState.IsValid)
            {
                Mesa mesa = db.Mesas.Where(m => m.Codigo == codMesa).SingleOrDefault();
                if (mesa == null)
                {
                    return HttpNotFound();
                }

                Configuracoes config = Configuracoes.ObterInstancia();

                LocalInterno localInterno = null;
                if (config.UtilizaComanda)
                {                    
                    if (codLocalInterno.HasValue && codLocalInterno.Value > 0) //Zero é o "Não selecionado" aí setamos nulo
                    {
                        localInterno = db.LocaisInternos.SingleOrDefault(l => l.Codigo == codLocalInterno);
                        if (localInterno == null)
                        {
                            return HttpNotFound();
                        }
                    }
                    else
                    {
                        ViewBag.Mesas = new SelectList(db.Mesas.Where(m => !m.CodUltimoPedido.HasValue).OrderBy(m => m.Descricao), "Codigo", "Descricao");
                        AdicionarLocaisInternosViewBag();
                        ViewBag.MensagemErro = "Escolha o local de entrega";
                        return View();
                    }
                }

                if (config.RequerObservacaoAoAbrirPedidoInterno && string.IsNullOrWhiteSpace(observacoes))
                {
                    ViewBag.Mesas = new SelectList(db.Mesas.Where(m => !m.CodUltimoPedido.HasValue).OrderBy(m => m.Descricao), "Codigo", "Descricao");
                    AdicionarLocaisInternosViewBag();
                    ViewBag.MensagemErro = "O sistema requer as observações no pedido interno";
                    return View();
                }

                GestorPedidos gPedidos = new Models.Gestores.GestorPedidos(db);
                Pedido pedido = gPedidos.CriarPedido(User.Identity, mesa, localInterno, observacoes);
                return RedirectToAction("Edit", "Pedidos", new { Id = pedido.Codigo });
            }

            ViewBag.Mesas = new SelectList(db.Mesas.Where(m => !m.CodUltimoPedido.HasValue).OrderBy(m => m.Descricao), "Codigo", "Descricao");
            if (Configuracoes.ObterInstancia().UtilizaComanda)
                AdicionarLocaisInternosViewBag();
            return View();
        }

        #endregion

        #region Delete

        // GET: PedidosInternos/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoInterno pedidoInterno = db.PedidosInternos.Find(id);
            if (pedidoInterno == null)
            {
                return HttpNotFound();
            }
            Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == id.Value).SingleOrDefault();
            ViewBag.Mesa = (mesa == null ? "Sem Mesa" : mesa.Descricao);
            return View(pedidoInterno);
        }

        // POST: PedidosInternos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult DeleteConfirmed(int id)
        {
            PedidoInterno pedidoInterno = null;
            db.IniciarTransacao();
            try
            {
                Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == id).SingleOrDefault();
                if (mesa != null)
                    mesa.CodUltimoPedido = null;
                pedidoInterno = db.PedidosInternos.Find(id);
                db.PedidosInternos.Remove(pedidoInterno);
                db.SaveChanges();
                return RedirectToAction("Index", "Pedidos");
            }
            catch (Exception ex)
            {
                db.SetarRollBack();
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este Pedido!\r\n" + ex.Message);
                return View(pedidoInterno);
            }
            finally
            {
                db.FinalizarTransacao();
            }
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
    }
}
