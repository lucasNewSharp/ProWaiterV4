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
using ProWaiter.Web.Models.Gestores;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.Controllers
{
    public class ItensBalcaoController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: ItensBalcao
        public ActionResult Index()
        {
            return View(db.ItensBacao.ToList());
        }

        // GET: ItensBalcao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ItemBalcao itemBalcao = db.ItensBacao.Find(id);
            if (itemBalcao == null)
            {
                return NotFound();
            }
            return View(itemBalcao);
        }

        // GET: ItensBalcao/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View(new ItemBalcao() { Valor = 0, PercDesconto = 0, Ativo = true });
        }

        // POST: ItensBalcao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome,Valor,PercDesconto,CodBarras,Ativo")] ItemBalcao itemBalcao)
        {
            if (db.ItensBacao.Any(b => b.Nome.Equals(itemBalcao.Nome, StringComparison.CurrentCultureIgnoreCase)))
            {
                ModelState.AddModelError("", "Já existe um item com este nome");
            }

            IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(itemBalcao.CodBarras, false);
            if (item != null)
            {
                ModelState.AddModelError("", $"Já existe uma item com este código de barras: {item.Nome}");
            }

            if (ModelState.IsValid)
            {
                db.ItensBacao.Add(itemBalcao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(itemBalcao);
        }

        // GET: ItensBalcao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ItemBalcao itemBalcao = db.ItensBacao.Find(id);
            if (itemBalcao == null)
            {
                return NotFound();
            }
            return View(itemBalcao);
        }

        // POST: ItensBalcao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Codigo,Nome,Valor,PercDesconto,CodBarras,Ativo")] ItemBalcao itemBalcao)
        {
            IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(itemBalcao.CodBarras, false);
            if (item != null && ((item is ItemBalcao itBalcao && itBalcao.Codigo != itemBalcao.Codigo) || !(item is ItemBalcao)))
            {
                ModelState.AddModelError("", $"Já existe um item com este código de barras: {item.Nome}!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(itemBalcao).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(itemBalcao);
        }

        // GET: ItensBalcao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ItemBalcao itemBalcao = db.ItensBacao.Find(id);
            if (itemBalcao == null)
            {
                return NotFound();
            }
            return View(itemBalcao);
        }

        // POST: ItensBalcao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemBalcao itemBalcao = db.ItensBacao.Find(id);
            try
            {
                db.ItensBacao.Remove(itemBalcao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este item!\r\nPossivelmente ele esteja sendo utilizada em um pedido já realizado.");
                return View(itemBalcao);
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
