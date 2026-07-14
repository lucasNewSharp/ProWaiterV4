using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProWaiter.Web.Controllers
{
    public class ConfiguracoesCategoriasController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: ConfiguracoesCategorias
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Index(string msgErro = null, string msgSucesso = null)
        {
            if (!string.IsNullOrEmpty(msgErro))
                ModelState.AddModelError("", msgErro);

            if (!string.IsNullOrEmpty(msgSucesso))
                ViewBag.MsgSucesso = msgSucesso;

            var configs = db.TiposRefeicao
                .ToList()
                .Select(t => new ConfiguracoesCategorias()
                {
                    ID = "R" + t.Codigo,
                    Nome = t.Nome,
                    Posicao = t.Posicao,
                    CorFonte = t.CorFonte,
                    CorFundo = t.CorFundo
                })
                .ToList();

            configs.AddRange(
                db.TiposBebida
                .ToList()
                .Select(t => new ConfiguracoesCategorias()
                {
                    ID = "B" + t.Codigo,
                    Nome = t.Nome,
                    Posicao = t.Posicao,
                    CorFonte = t.CorFonte,
                    CorFundo = t.CorFundo
                })
                .ToList()
                );

            configs = configs.OrderBy(c => c.Posicao).ToList();

            return View(configs);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Salvar(FormCollection collection)
        {
            try
            {
                var ids = collection["ids"].ToString().Split(',');
                byte pos = 0;
                foreach (string id in ids)
                {
                    short codigo = short.Parse(id.Substring(1));
                    string corFundo = collection["cfu_" + id];
                    string corFonte = collection["cfo_" + id];
                    if (id.StartsWith("B"))
                    {
                        TipoBebida tipo = db.TiposBebida.SingleOrDefault(t => t.Codigo == codigo);
                        tipo.Posicao = ++pos;
                        tipo.CorFundo = corFundo;
                        tipo.CorFonte = corFonte;
                        db.Entry(tipo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        TipoRefeicao tipo = db.TiposRefeicao.SingleOrDefault(t => t.Codigo == codigo);
                        tipo.Posicao = ++pos;
                        tipo.CorFundo = corFundo;
                        tipo.CorFonte = corFonte;
                        db.Entry(tipo).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
                db.SaveChanges();

                return RedirectToAction("Index", new { msgErro = string.Empty, msgSucesso = "Categorias salvas com sucesso" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { msgErro = "Não foi possível salvar as configurações! " + ex.Message, msgSucesso = string.Empty });
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
