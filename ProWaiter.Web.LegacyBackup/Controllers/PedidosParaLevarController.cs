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
using ProWaiter.Web.Models.Gestores;

namespace ProWaiter.Web.Controllers
{
    public class PedidosParaLevarController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: PedidosParaLevar
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pedidos");
        }

        // GET: PedidosParaLevar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoParaLevar pedidoParaLevar = db.PedidosParaLevar.Find(id);
            if (pedidoParaLevar == null)
            {
                return HttpNotFound();
            }
            ViewBag.ErrosImpressao = TempData["ErrosImpressao"];
            return View(pedidoParaLevar);
        }

        // GET: PedidosParaLevar/Create
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create()
        {
            if (ModelState.IsValid)
            {
                GestorPedidos gPedidos = new Models.Gestores.GestorPedidos(db);
                Pedido pedido = gPedidos.CriarPedido(User.Identity, null);
                return RedirectToAction("Edit", "Pedidos", new { Id = pedido.Codigo });
            }

            return View();
        }

        // GET: PedidosParaLevar/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoParaLevar pedidoParaLevar = db.PedidosParaLevar.Find(id);
            if (pedidoParaLevar == null)
            {
                return HttpNotFound();
            }
            return View(pedidoParaLevar);
        }

        // POST: PedidosParaLevar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult DeleteConfirmed(int id)
        {
            PedidoParaLevar pedidoParaLevar = db.PedidosParaLevar.Find(id);
            db.Pedidos.Remove(pedidoParaLevar);
            db.SaveChanges();
            return RedirectToAction("Index", "Pedidos");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
