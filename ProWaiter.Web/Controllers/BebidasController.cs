using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Util;
using ProWaiter.Web.Models.Gestores;




namespace ProWaiter.Web.Controllers
{
    public class BebidasController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: Bebidas
        public ActionResult Index()
        {
            return View(db.Bebidas.OrderBy(b => b.Nome).ToList());
        }

        // GET: Bebidas/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bebida bebida = db.Bebidas.Find(id);
            if (bebida == null)
            {
                return NotFound();
            }
            return View(bebida);
        }

        // GET: Bebidas/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            ViewBag.CodTipo = new SelectList(db.TiposBebida.OrderBy(t => t.Nome), "Codigo", "Nome");
            ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(t => t.NomeExibicao), "Codigo", "NomeExibicao");
            Bebida beb = new Bebida() { Ativo = true };
            return View(beb);
        }

        // POST: Bebidas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome,Valor,Ativo,CodTipo,Tipo,CodImpressora,Impressora, PercDesconto, CodBarras")] Bebida bebida)
        {
            if (ModelState.IsValid)
            {
                if (db.Bebidas.Any(b => b.Nome.Equals(bebida.Nome, StringComparison.CurrentCultureIgnoreCase)))
                {
                    ModelState.AddModelError("", "Já existe uma bebida com este nome");
                }

                IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(bebida.CodBarras, false);
                if (item != null)
                {
                    ModelState.AddModelError("", $"Já existe uma item com este código de barras: {item.Nome}");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.CodTipo = new SelectList(db.TiposBebida.OrderBy(t => t.Nome), "Codigo", "Nome");
                    ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(t => t.NomeExibicao), "Codigo", "NomeExibicao");
                    return View(bebida);
                }

                db.Bebidas.Add(bebida);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CodTipo = new SelectList(db.TiposBebida.OrderBy(t => t.Nome), "Codigo", "Nome");
            ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(t => t.NomeExibicao), "Codigo", "NomeExibicao");
            return View(bebida);
        }

        // GET: Bebidas/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bebida bebida = db.Bebidas.Find(id);
            if (bebida == null)
            {
                return NotFound();
            }

            ViewBag.Tipos = new SelectList(db.TiposBebida.OrderBy(t => t.Nome), "Codigo", "Nome", bebida.Tipo);
            ViewBag.Impressoras = new SelectList(db.Impressoras.ToList().OrderBy(t => t.NomeExibicao), "Codigo", "NomeExibicao", bebida.Impressora);
            return View(bebida);
        }

        // POST: Bebidas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Nome,Valor,Ativo,Tipo,CodTipo,Impressora,CodImpressora,PercDesconto,CodBarras")] Bebida bebida)
        {
            if (ModelState.IsValid)
            {
                if (db.Bebidas.Any(b => b.Nome.Equals(bebida.Nome, StringComparison.CurrentCultureIgnoreCase) && b.Codigo != bebida.Codigo))
                {
                    ModelState.AddModelError("", "Já existe uma bebida com este nome");
                }

                IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(bebida.CodBarras, false);
                if (item != null && ((item is Bebida beb && beb.Codigo != bebida.Codigo) || !(item is Bebida)))
                {
                    ModelState.AddModelError("", $"Já existe um item com este código de barras: {item.Nome}!");
                }

                if (ModelState.IsValid)
                {
                    db.Entry(bebida).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Tipos = new SelectList(db.TiposBebida.OrderBy(t => t.Nome), "Codigo", "Nome", bebida.Tipo);
            ViewBag.Impressoras = new SelectList(db.Impressoras.ToList().OrderBy(t => t.NomeExibicao), "Codigo", "NomeExibicao", bebida.Impressora);
            return View(bebida);
        }

        // GET: Bebidas/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bebida bebida = db.Bebidas.Find(id);
            if (bebida == null)
            {
                return NotFound();
            }
            return View(bebida);
        }

        // POST: Bebidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short id)
        {
            Bebida bebida = db.Bebidas.Find(id);
            try
            {
                db.Bebidas.Remove(bebida);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir esta bebida!\r\nPossivelmente ela esteja sendo utilizada em um pedido já realizado.");
                return View(bebida);
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
