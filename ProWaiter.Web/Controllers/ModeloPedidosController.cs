using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProWaiter.Web.Models.DTOs;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.Controllers
{
    public class ModeloPedidosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: ModeloPedidos
        public ActionResult Index()
        {
            return View(db.ModelosPedidos.OrderBy(m => m.Nome).ToList());
        }

        // GET: ModeloPedidos/Details/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloPedido modeloPedido = db.ModelosPedidos.Find(id);
            if (modeloPedido == null)
            {
                return HttpNotFound();
            }
            return View(modeloPedido);
        }

        // GET: ModeloPedidos/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloPedido modeloPedido = db.ModelosPedidos.Find(id);
            if (modeloPedido == null)
            {
                return HttpNotFound();
            }
            return View(modeloPedido);
        }

        // POST: ModeloPedidos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind(Include = "Codigo,Nome,Desconto,Acrescimo,Observacoes")] ModeloPedido modeloPedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modeloPedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modeloPedido);
        }

        // GET: ModeloPedidos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModeloPedido modeloPedido = db.ModelosPedidos.Find(id);
            if (modeloPedido == null)
            {
                return HttpNotFound();
            }
            return View(modeloPedido);
        }

        // POST: ModeloPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(int id)
        {
            ModeloPedido modeloPedido = db.ModelosPedidos.Find(id);
            db.ModelosPedidos.Remove(modeloPedido);
            db.SaveChanges();
            return RedirectToAction("Index");
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
