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
using System.Configuration;


namespace ProWaiter.Web.Controllers
{
    public class ClientesController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: Clientes
        public ActionResult Index()
        {            
            return View(db.Clientes.OrderBy(c => c.Nome).ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create

        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create(bool modal = false, string returnUrl = null)
        {
            ViewBag.Modal = modal;

            int codCidade = Configuracoes.ObterInstancia().CodCidadePadrao;
            PopularComboBoxes(codCidade);
            ViewBag.ReturnUrl = returnUrl;
            return View(new ClienteCreateViewModel() { ValorEntregaPadrao = 0 });
        }

        [HttpPost]
        public ActionResult LoadCidades(string uf)
        {
            SelectList sl = new SelectList(db.Cidades.Where(c => c.UF.Codigo == uf).OrderBy(c => c.Nome), "Codigo", "Nome");
            JsonResult res = Json(sl);
            return res;
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create([Bind("Nome,Endereco,Bairro,CodCidade,Telefone1,Telefone2,ValorEntregaPadrao,ObservacoesPadrao")] ClienteCreateViewModel cliente, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                Cliente clienteDB = new Cliente()
                {
                    Nome = cliente.Nome,
                    Telefone1 = cliente.Telefone1,
                    Telefone2 = cliente.Telefone2
                };

                EnderecoCliente endereco = new EnderecoCliente()
                {
                    Endereco = cliente.Endereco,
                    Bairro = cliente.Bairro,
                    CodCidade = cliente.CodCidade,
                    ValorEntregaPadrao = cliente.ValorEntregaPadrao,
                    ObservacoesPadrao = cliente.ObservacoesPadrao
                };

                clienteDB.Enderecos.Add(endereco);

                db.Clientes.Add(clienteDB);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (returnUrl.Contains("CodClienteSelecionado"))
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("CodClienteSelecionado") - 1);

                    if (!returnUrl.Contains("?"))
                        returnUrl += "?";
                    else returnUrl += "&";
                    returnUrl += $"CodClienteSelecionado={clienteDB.Codigo}";
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index");
            }

            PopularComboBoxes(cliente.CodCidade);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Edit(int? id, bool modal = false, string returnUrl = null)
        {
            ViewBag.Modal = modal;
            ViewBag.ReturnUrl = returnUrl;
            if (id == null)
            {
                return BadRequest();
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }            
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]        
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Edit([Bind("Codigo,Nome,Telefone1,Telefone2")] Cliente cliente, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (returnUrl.Contains("CodClienteSelecionado"))
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("CodClienteSelecionado") - 1);

                    if (!returnUrl.Contains("?"))
                        returnUrl += "?";
                    else returnUrl += "&";
                    returnUrl += $"CodClienteSelecionado={cliente.Codigo}";
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index");
            }            
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            try
            {
                db.Clientes.Remove(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este Cliente pois ele já realizou pelo menos um pedido!");
                return View(cliente);
            }
        }

        private void PopularComboBoxes(int? codCidade)
        {
            if (!codCidade.HasValue)
                throw new ArgumentNullException("codCidade");
            ViewBag.CodCidade = new SelectList(db.Cidades.OrderBy(c => c.Nome), "Codigo", "Nome", codCidade);
            Cidade cidade = db.Cidades.Where(c => c.Codigo == codCidade).Single();
            ViewBag.Estados = new SelectList(db.UFs.OrderBy(u => u.Nome), "Codigo", "Nome", cidade.CodUF);

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
