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
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.Controllers
{
    public class ComponentesRefeicoesController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: ComponentesRefeicoes
        public ActionResult Index()
        {
            return View(db.ComponentesRefeicao.ToList());
        }

        // GET: ComponentesRefeicoes/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ComponenteRefeicao componenteRefeicao = db.ComponentesRefeicao.Find(id);
            if (componenteRefeicao == null)
            {
                return NotFound();
            }
            return View(componenteRefeicao);
        }

        // GET: ComponentesRefeicoes/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComponentesRefeicoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome")] ComponenteRefeicao componenteRefeicao)
        {
            if (ModelState.IsValid)
            {
                if (db.ComponentesRefeicao.Any(c => c.Nome.Equals(componenteRefeicao.Nome, StringComparison.CurrentCultureIgnoreCase)))
                {
                    ModelState.AddModelError("", "Já existe um componente com este nome!");
                    return View(componenteRefeicao);
                }

                db.ComponentesRefeicao.Add(componenteRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(componenteRefeicao);
        }

        // GET: ComponentesRefeicoes/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ComponenteRefeicao componenteRefeicao = db.ComponentesRefeicao.Find(id);
            if (componenteRefeicao == null)
            {
                return NotFound();
            }
            return View(componenteRefeicao);
        }

        // POST: ComponentesRefeicoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Nome")] ComponenteRefeicao componenteRefeicao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(componenteRefeicao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(componenteRefeicao);
        }

        // GET: ComponentesRefeicoes/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ComponenteRefeicao componenteRefeicao = db.ComponentesRefeicao.Find(id);
            if (componenteRefeicao == null)
            {
                return NotFound();
            }
            return View(componenteRefeicao);
        }

        // POST: ComponentesRefeicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            ComponenteRefeicao componenteRefeicao = db.ComponentesRefeicao.Find(id);
            try
            {
                db.ComponentesRefeicao.Remove(componenteRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este componente de refeição! Possívelmente ele está sendo usado em alguma refeição cadastrada.");
                return View(componenteRefeicao);
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
