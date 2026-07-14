using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
    public class LocaisInternosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: LocaisInternos
        public ActionResult Index()
        {
            return View(db.LocaisInternos.OrderBy(l => l.Nome).ToList());
        }

        // GET: LocaisInternos/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            LocalInterno localInterno = db.LocaisInternos.Find(id);
            if (localInterno == null)
            {
                return NotFound();
            }
            return View(localInterno);
        }

        // GET: LocaisInternos/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: LocaisInternos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome")] LocalInterno localInterno)
        {
            if (ModelState.IsValid)
            {
                var localBD = db.LocaisInternos.Where(l => l.Nome.ToUpper() == localInterno.Nome.ToUpper()).SingleOrDefault();
                if(localBD != null)
                {
                    ModelState.AddModelError("", "Já existe um cadastro com o nome " + localInterno.Nome);
                    return View(localInterno);
                }

                db.LocaisInternos.Add(localInterno);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(localInterno);
        }

        // GET: LocaisInternos/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            LocalInterno localInterno = db.LocaisInternos.Find(id);
            if (localInterno == null)
            {
                return NotFound();
            }
            return View(localInterno);
        }

        // POST: LocaisInternos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Nome")] LocalInterno localInterno)
        {
            if (ModelState.IsValid)
            {
                var localBD = db.LocaisInternos.Where(l => l.Nome.ToUpper() == localInterno.Nome.ToUpper() && l.Codigo != localInterno.Codigo).SingleOrDefault();
                if (localBD != null)
                {
                    ModelState.AddModelError("", "Já existe um outro cadastro com o nome " + localInterno.Nome);
                    return View(localInterno);
                }

                db.Entry(localInterno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(localInterno);
        }

        // GET: LocaisInternos/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            LocalInterno localInterno = db.LocaisInternos.Find(id);
            if (localInterno == null)
            {
                return NotFound();
            }
            return View(localInterno);
        }

        // POST: LocaisInternos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            LocalInterno localInterno = db.LocaisInternos.Find(id);
            db.LocaisInternos.Remove(localInterno);
            db.SaveChanges();
            return RedirectToAction("Index");
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
