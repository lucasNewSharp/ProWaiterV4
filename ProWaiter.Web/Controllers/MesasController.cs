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
using ProWaiter.Web.Util;
using ProWaiter.Web.Models.Entidades;

namespace ProWaiter.Web.Controllers
{
    public class MesasController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: Mesas
        public ActionResult Index()
        {
            return View(db.Mesas.OrderBy(m => m.Descricao).ToList());
        }

        // GET: Mesas/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Mesa mesa = db.Mesas.Find(id);
            if (mesa == null)
            {
                return NotFound();
            }
            return View(mesa);
        }

        // GET: Mesas/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mesas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Descricao")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                var mesaBD = db.Mesas.Where(m => m.Descricao.ToUpper() == mesa.Descricao).SingleOrDefault();
                if(mesaBD != null)
                {
                    ModelState.AddModelError("", "Já existe um cadastrado com o nome " + mesa.Descricao);
                    return View(mesa);
                }

                db.Mesas.Add(mesa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mesa);
        }

        // GET: Mesas/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Mesa mesa = db.Mesas.Find(id);
            if (mesa == null)
            {
                return NotFound();
            }
            return View(mesa);
        }

        // POST: Mesas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Descricao")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                //Existe outro cadastro com esse nome
                var mesaBD = db.Mesas.Where(m => m.Descricao.ToUpper() == mesa.Descricao && m.Codigo != mesa.Codigo).SingleOrDefault();
                if (mesaBD != null)
                {
                    ModelState.AddModelError("", "Já existe um outro cadastrado com o nome " + mesa.Descricao);
                    return View(mesa);
                }

                db.Entry(mesa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mesa);
        }

        // GET: Mesas/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Mesa mesa = db.Mesas.Find(id);
            if (mesa == null)
            {
                return NotFound();
            }
            return View(mesa);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            Mesa mesa = db.Mesas.Find(id);
            try
            {
                db.Mesas.Remove(mesa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                mesa = db.Mesas.Find(id);
                string nome = Configuracoes.ObterInstancia().UtilizaComanda ? "Comanda" : "Mesa";
                ModelState.AddModelError(string.Empty, $"Não foi possível excluir esta {nome}!\r\nEla está vinculada a um pedido.");
                return View(mesa);
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
