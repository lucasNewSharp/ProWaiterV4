using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.Controllers
{
    public class TiposBebidaController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: TiposBebida
        public ActionResult Index()
        {
            return View(db.TiposBebida.OrderBy(b => b.Nome).ToList());
        }

        // GET: TiposBebida/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TipoBebida tipoBebida = db.TiposBebida.Find(id.Value);
            if (tipoBebida == null)
            {
                return NotFound();
            }
            return View(tipoBebida);
        }

        // GET: TiposBebida/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TiposBebida/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome")] TipoBebida tipoBebida)
        {
            if (ModelState.IsValid)
            {
                db.TiposBebida.Add(tipoBebida);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoBebida);
        }

        // GET: TiposBebida/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TipoBebida tipoBebida = db.TiposBebida.Find(id.Value);
            if (tipoBebida == null)
            {
                return NotFound();
            }
            return View(tipoBebida);
        }

        // POST: TiposBebida/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Nome")] TipoBebida tipoBebida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoBebida).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoBebida);
        }

        // GET: TiposBebida/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TipoBebida tipoBebida = db.TiposBebida.Find(id.Value);
            if (tipoBebida == null)
            {
                return NotFound();
            }
            return View(tipoBebida);
        }

        // POST: TiposBebida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            TipoBebida tipoBebida = db.TiposBebida.Find(id);
            try
            {
                db.TiposBebida.Remove(tipoBebida);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não é possível remover o tipo de bebida pois ele está sendo utilizado em pelo menos uma bebida!");
                return View(tipoBebida);
            }
        }

//         // // protected void Dispose(bool disposing)
//         {
//             if (disposing)
//             {
//                 // // db.Dispose();
//             }
//             // base.Dispose(disposing);
//         }
    }
}
