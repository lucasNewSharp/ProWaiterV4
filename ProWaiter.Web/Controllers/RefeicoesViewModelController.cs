using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProWaiter.Web.Controllers
{
    public class RefeicoesViewModelController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: RefeicoesViewModel
        public ActionResult Index()
        {
            List<ComponenteRefeicao> componentes = db.ComponentesRefeicao.OrderBy(c => c.Nome).ToList();
            List<Refeicao> refeicoes = db.Refeicoes.OrderBy(r => r.Nome).ToList();
            return View(refeicoes.Select(r => new RefeicaoViewModel(r, componentes)).ToList());
        }

        // GET: RefeicoesViewModel/Details/5
        public ActionResult Details(string id)
        {
            short numCodigo = 0;
            if (string.IsNullOrEmpty(id) || !short.TryParse(id, out numCodigo))
                return BadRequest();
            Refeicao refeicao = db.Refeicoes.Where(r => r.Codigo == numCodigo).FirstOrDefault();
            if (refeicao == null)
                return NotFound();

            return View(new RefeicaoViewModel(refeicao, refeicao.ComponentesRefeicao));
        }

        // GET: RefeicoesViewModel/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create()
        {
            ViewBag.CodTipo = new SelectList(db.TiposRefeicao.OrderBy(t => t.Nome), "Codigo", "Nome");

            List<KeyValuePair<string, string>> listaDisponiveis = new List<KeyValuePair<string, string>>();
            foreach (ComponenteRefeicao componente in db.ComponentesRefeicao.OrderBy(c => c.Nome))
                listaDisponiveis.Add(new KeyValuePair<string, string>(componente.Codigo.ToString(), componente.Nome));

            ViewBag.Componentes = new BootstrapDualListModelView() { ListaDisponiveis = listaDisponiveis };
            return View();
        }

        // POST: RefeicoesViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                short codTipo = short.Parse(collection["CodTipo"]);
                TipoRefeicao tipo = db.TiposRefeicao.Single(t => t.Codigo == codTipo);

                Refeicao refeicao = new Refeicao(collection["Nome"], tipo);

                foreach (string strCodComponente in collection["ItensSelecionados"].ToString().Split(','))
                {
                    if (String.IsNullOrEmpty(strCodComponente)) continue;
                    short codComponente = short.Parse(strCodComponente);
                    ComponenteRefeicao componente = db.ComponentesRefeicao.Single(c => c.Codigo == codComponente);
                    refeicao.ComponentesRefeicao.Add(componente);
                }

                db.Refeicoes.Add(refeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: RefeicoesViewModel/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short id)
        {
            Refeicao refeicao = db.Refeicoes.Where(r => r.Codigo == id).FirstOrDefault();
            if (refeicao == null)
                return NotFound();

            ViewBag.Tipos = new SelectList(db.TiposRefeicao.OrderBy(t => t.Nome), "Codigo", "Nome", refeicao.Tipo);
            List<ComponenteRefeicao> componentes = db.ComponentesRefeicao.OrderBy(c => c.Nome).ToList();
            return View(new RefeicaoViewModel(refeicao, componentes));
        }

        // POST: RefeicoesViewModel/Edit/5
        [HttpPost]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short id, FormCollection collection)
        {
            Refeicao refeicao = db.Refeicoes.Where(r => r.Codigo == id).FirstOrDefault();
            if (refeicao == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                AtualizarRefeicaoComFormulario(db, refeicao, collection);

                db.Entry(refeicao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Tipos = new SelectList(db.TiposRefeicao.OrderBy(t => t.Nome), "Codigo", "Nome", refeicao.Tipo);
            List<ComponenteRefeicao> componentes = db.ComponentesRefeicao.OrderBy(c => c.Nome).ToList();
            return View(new RefeicaoViewModel(refeicao, componentes));
        }

        private void AtualizarRefeicaoComFormulario(ProWaiterContext db, Refeicao refeicao, FormCollection collection)
        {
            refeicao.Nome = collection["Nome"];
            short codTipo = short.Parse(collection["CodTipo"]);
            refeicao.Tipo = db.TiposRefeicao.Where(t => t.Codigo == codTipo).Single();
            refeicao.ComponentesRefeicao.Clear();
            foreach (string strCodComponente in collection["ItensSelecionados"].ToString().Split(','))
            {
                if (String.IsNullOrEmpty(strCodComponente)) continue;
                short codComponente = short.Parse(strCodComponente);
                ComponenteRefeicao componente = db.ComponentesRefeicao.Single(c => c.Codigo == codComponente);
                refeicao.ComponentesRefeicao.Add(componente);
            }
        }

        // GET: RefeicoesViewModel/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Refeicao refeicao = db.Refeicoes.SingleOrDefault(r => r.Codigo == id);
            if (refeicao == null)
            {
                return NotFound();
            }
            return View(new RefeicaoViewModel(refeicao, refeicao.ComponentesRefeicao));
        }

        // POST: RefeicoesViewModel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short id, FormCollection collection)
        {
            Refeicao refeicao = db.Refeicoes.SingleOrDefault(r => r.Codigo == id);
            TipoRefeicao tipo = null;
            if (refeicao == null)
            {
                return NotFound();
            }
            try
            {
                tipo = refeicao.Tipo;
                db.Refeicoes.Remove(refeicao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não é possível remover esta refeição pois ela está sendo usada no cardápio!");
                refeicao.Tipo = tipo;
                return View(new RefeicaoViewModel(refeicao, refeicao.ComponentesRefeicao));
            }
        }

        [HttpPost]
        public ActionResult CriarComponenteRefeicao(string nome)
        {
            if (db.ComponentesRefeicao.Any(c => c.Nome.Equals(nome, StringComparison.CurrentCultureIgnoreCase)))
                return StatusCode((int)400, "Já existe um componente com o nome \"" + nome +"\"!");

            ComponenteRefeicao componente = new ComponenteRefeicao(nome);

            db.ComponentesRefeicao.Add(componente);
            db.SaveChanges();
            return Json(componente);
        }
    }
}
