using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.Controllers
{
    public class TiposRefeicaoController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: TiposRefeicao
        public ActionResult Index()
        {
            return View(db.TiposRefeicao.ToList());
        }

        // GET: TiposRefeicao/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoRefeicao tipoRefeicao = db.TiposRefeicao.Find(id);
            if (tipoRefeicao == null)
            {
                return HttpNotFound();
            }
            return View(tipoRefeicao);
        }

        // GET: TiposRefeicao/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TiposRefeicao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind(Include = "Codigo,Nome")] TipoRefeicao tipoRefeicao)
        {
            if (ModelState.IsValid)
            {
                db.TiposRefeicao.Add(tipoRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoRefeicao);
        }

        // GET: TiposRefeicao/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoRefeicao tipoRefeicao = db.TiposRefeicao.Find(id);
            if (tipoRefeicao == null)
            {
                return HttpNotFound();
            }
            return View(tipoRefeicao);
        }

        // POST: TiposRefeicao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind(Include = "Codigo,Nome")] TipoRefeicao tipoRefeicao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoRefeicao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoRefeicao);
        }

        // GET: TiposRefeicao/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoRefeicao tipoRefeicao = db.TiposRefeicao.Find(id);
            if (tipoRefeicao == null)
            {
                return HttpNotFound();
            }
            return View(tipoRefeicao);
        }

        // POST: TiposRefeicao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            TipoRefeicao tipoRefeicao = db.TiposRefeicao.Find(id);
            try
            {
                db.TiposRefeicao.Remove(tipoRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não é possível remover este tipo de refeição pois existe pelo menos uma refeição deste tipo castrada no sistema!");
                return View(tipoRefeicao);
            }
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
