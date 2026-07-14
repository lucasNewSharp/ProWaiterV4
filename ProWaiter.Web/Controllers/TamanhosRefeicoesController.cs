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
    public class TamanhosRefeicoesController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: TamanhoRefeicaos
        public ActionResult Index(string msgErro, string msgSucesso, bool? posicoesSalvas)
        {
            ViewBag.PosicoesSalvas = posicoesSalvas;
            ViewBag.MsgErro = msgErro;
            ViewBag.MsgSucesso = msgSucesso;
            return View(db.TamanhosRefeicao.ToList());
        }

        // GET: TamanhoRefeicaos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TamanhoRefeicao tamanhoRefeicao = db.TamanhosRefeicao.Find(id);
            if (tamanhoRefeicao == null)
            {
                return NotFound();
            }
            return View(tamanhoRefeicao);
        }

        // GET: TamanhoRefeicaos/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TamanhoRefeicaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("Codigo,Nome")] TamanhoRefeicao tamanhoRefeicao)
        {
            if (ModelState.IsValid)
            {
                char maiorCodigo = char.Parse(db.TamanhosRefeicao.Max(c => c.Codigo));
                tamanhoRefeicao.Codigo = (++maiorCodigo).ToString();

                db.TamanhosRefeicao.Add(tamanhoRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tamanhoRefeicao);
        }

        // GET: TamanhoRefeicaos/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TamanhoRefeicao tamanhoRefeicao = db.TamanhosRefeicao.Find(id);
            if (tamanhoRefeicao == null)
            {
                return NotFound();
            }
            return View(tamanhoRefeicao);
        }

        // POST: TamanhoRefeicaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("Codigo,Nome")] TamanhoRefeicao tamanhoRefeicao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tamanhoRefeicao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tamanhoRefeicao);
        }

        // GET: TamanhoRefeicaos/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            TamanhoRefeicao tamanhoRefeicao = db.TamanhosRefeicao.Find(id);
            if (tamanhoRefeicao == null)
            {
                return NotFound();
            }
            return View(tamanhoRefeicao);
        }

        // POST: TamanhoRefeicaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(string id)
        {
            TamanhoRefeicao tamanhoRefeicao = db.TamanhosRefeicao.Find(id);
            try
            {                
                db.TamanhosRefeicao.Remove(tamanhoRefeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este tamanho de refeição!\r\nEle está sendo utilizado no cardápio.");
                return View(tamanhoRefeicao);
            }
        }


        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult SalvarPosicao(FormCollection collection)
        {
            try
            {
                var ids = collection["ids"].ToString().Split(',');
                byte pos = 0;
                foreach (string id in ids)
                {                    
                    TamanhoRefeicao tam = db.TamanhosRefeicao.Find(id);
                    tam.Posicao = ++pos;
                    db.Entry(tam).State = EntityState.Modified;                    
                }
                db.SaveChanges();

                return RedirectToAction("Index", new { msgErro = "", msgSucesso = "Posições salvas com sucesso", posicoesSalvas = true });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { msgErro = "Não foi possível salvar as configurações! " + ex.Message, msgSucesso = "", posicoesSalvas = true });
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
