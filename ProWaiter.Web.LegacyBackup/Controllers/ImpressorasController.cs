using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System.Management;

namespace ProWaiter.Web.Controllers
{
    public class ImpressorasController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: Impressoras
        public ActionResult Index()
        {
            return View(db.Impressoras.ToList());
        }

        // GET: Impressoras/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impressora impressora = db.Impressoras.Find(id);
            if (impressora == null)
            {
                return HttpNotFound();
            }
            return View(impressora);
        }

        // GET: Impressoras/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            ViewBag.NomesImpressoras = GestorImpressoes.Instancia.ObterNomesImpressorasInstaladas();
            ViewBag.NomesTipoImpressao = new SelectList(GestorImpressoes.Instancia.ObterTiposImpressoras(), "Valor", "Nome");
            return View();
        }

        // POST: Impressoras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind(Include = "Codigo,Nome,Local,NomeTipoImpressao,EhDoCaixa,EhDeEntrega,BuzinaAtivada,Ip,Porta")] Impressora impressora)
        {
            if (ModelState.IsValid)
            {
                db.Impressoras.Add(impressora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(impressora);
        }

        // GET: Impressoras/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impressora impressora = db.Impressoras.Find(id);
            if (impressora == null)
            {
                return HttpNotFound();
            }
            ViewBag.NomesImpressoras = GestorImpressoes.Instancia.ObterNomesImpressorasInstaladas();
            ViewBag.NomesTipoImpressao = new SelectList(GestorImpressoes.Instancia.ObterTiposImpressoras(), "Valor", "Nome", impressora.NomeTipoImpressao);
            return View(impressora);
        }

        // POST: Impressoras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind(Include = "Codigo,Nome,Local,NomeTipoImpressao,EhDoCaixa,EhDeEntrega,BuzinaAtivada,Ip,Porta")] Impressora impressora)
        {
            if (ModelState.IsValid)
            {
                db.Entry(impressora).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NomesImpressoras = GestorImpressoes.Instancia.ObterNomesImpressorasInstaladas();
            ViewBag.NomesTipoImpressao = new SelectList(GestorImpressoes.Instancia.ObterTiposImpressoras(), "Valor", "Nome", impressora.NomeTipoImpressao);
            return View(impressora);
        }

        // GET: Impressoras/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impressora impressora = db.Impressoras.Find(id);
            if (impressora == null)
            {
                return HttpNotFound();
            }
            return View(impressora);
        }

        // POST: Impressoras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(byte id)
        {
            Impressora impressora = db.Impressoras.Find(id);
            try
            {
                db.Impressoras.Remove(impressora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não é possível remover esta impressora pois ela está sendo utilizada por alguma refeição do cardápio!");
                return View(impressora);
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
