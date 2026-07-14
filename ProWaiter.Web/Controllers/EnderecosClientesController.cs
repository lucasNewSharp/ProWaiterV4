using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class EnderecosClientesController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: EnderecoClientes/Create        
        public ActionResult Create(int codCliente, int codCidade, string returnUrl = null)
        {
            if (codCidade == 0)
                codCidade = Configuracoes.ObterInstancia().CodCidadePadrao;

            Cidade cidade = db.Cidades.Where(c => c.Codigo == codCidade).Single();
            
            ViewBag.CodUF = new SelectList(db.UFs.OrderBy(u => u.Nome), "Codigo", "Nome", cidade.CodUF);
            ViewBag.Cidades = cidade.UF.Cidades.OrderBy(c => c.Nome).ToList();

            ViewBag.ReturnUrl = returnUrl;
            return View(new EnderecoClienteViewModel() { CodCliente = codCliente, CodCidade = cidade.Codigo });
        }

        // POST: EnderecoClientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]        
        public ActionResult Create([Bind("CodCliente,Endereco,Bairro,CodCidade,ValorEntregaPadrao,ObservacoesPadrao")] EnderecoClienteViewModel enderecoCliente, string returnUrl = null)
        {
            EnderecoCliente enderecoDB = new EnderecoCliente();

            if (ModelState.IsValid)
            {
                enderecoDB = new EnderecoCliente()
                {
                    CodCliente = enderecoCliente.CodCliente,
                    CodCidade = enderecoCliente.CodCidade,
                    Bairro = enderecoCliente.Bairro,
                    Endereco = enderecoCliente.Endereco,
                    ObservacoesPadrao = enderecoCliente.ObservacoesPadrao,
                    ValorEntregaPadrao = enderecoCliente.ValorEntregaPadrao
                };

                db.EnderecosClientes.Add(enderecoDB);
                db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.Contains("Clientes/Edit/"))//Retorno para cadastro a partir de tela de clientes, não mudamos a url de retorno
                {

                }
                else if (returnUrl.Contains("Pedidos/Edit/"))
                {
                    if (returnUrl.Contains("/?codEnderecoSelecionado"))
                    {
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("/?codEnderecoSelecionado"));
                    }
                    returnUrl += $"/?codEnderecoSelecionado={enderecoDB.Codigo}";
                }
                else
                {
                    if (returnUrl.Contains("CodClienteSelecionado"))
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("CodClienteSelecionado") - 1);

                    if (!returnUrl.Contains("?"))
                        returnUrl += "?";
                    else returnUrl += "&";
                    returnUrl += $"CodClienteSelecionado={enderecoDB.CodCliente}&CodEnderecoSelecionado={enderecoDB.Codigo}";
                }

            }
            return Redirect(returnUrl);
        }

        // GET: EnderecoClientes/Edit/5
        public ActionResult Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return BadRequest();
            }
            ViewBag.ReturnUrl = returnUrl;
            EnderecoCliente enderecoCliente = db.EnderecosClientes.Find(id);
            if (enderecoCliente == null)
            {
                return NotFound();
            }

            if (enderecoCliente.CodCidade.HasValue)
            {
                ViewBag.Cidades = new SelectList(enderecoCliente.Cidade.UF.Cidades.OrderBy(c => c.Nome), "Codigo", "Nome", enderecoCliente.CodCidade);
                ViewBag.CodUF = new SelectList(db.UFs.OrderBy(u => u.Nome), "Codigo", "Nome", enderecoCliente.Cidade.CodUF);
            }
            else
            {
                int cod = Configuracoes.ObterInstancia().CodCidadePadrao;
                Cidade cidade = db.Cidades.Where(c => c.Codigo == cod).Single();
                ViewBag.Cidades = new SelectList(cidade.UF.Cidades.OrderBy(c => c.Nome), "Codigo", "Nome", cidade.Codigo);
                ViewBag.CodUF = new SelectList(db.UFs.OrderBy(u => u.Nome), "Codigo", "Nome", cidade.CodUF);
            }
            return View(enderecoCliente);
        }

        // POST: EnderecoClientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Codigo,CodCliente,Endereco,Bairro,CodCidade,ValorEntregaPadrao,ObservacoesPadrao")] EnderecoCliente enderecoCliente, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enderecoCliente).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.CodCidade = new SelectList(db.Cidades, "Codigo", "Nome", enderecoCliente.CodCidade);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.Contains("Clientes/Edit/"))//Retorno para cadastro a partir de tela de clientes, não mudamos a url de retorno
                {

                }
                else if (returnUrl.Contains("Pedidos/Edit/"))
                {
                    if (returnUrl.Contains("/?codEnderecoSelecionado"))
                    {
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("/?codEnderecoSelecionado"));
                    }
                    returnUrl += $"/?codEnderecoSelecionado={enderecoCliente.Codigo}";
                }
                else
                {
                    if (returnUrl.Contains("CodClienteSelecionado"))
                        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("CodClienteSelecionado") - 1);

                    if (!returnUrl.Contains("?"))
                        returnUrl += "?";
                    else returnUrl += "&";
                    returnUrl += $"CodClienteSelecionado={enderecoCliente.CodCliente}&CodEnderecoSelecionado={enderecoCliente.Codigo}";
                }

            }
            return Redirect(returnUrl);
        }

        //// GET: EnderecoClientes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest();
        //    }
        //    EnderecoCliente enderecoCliente = db.EnderecosClientes.Find(id);
        //    if (enderecoCliente == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(enderecoCliente);
        //}

        [HttpPost]
        public ActionResult PodeExcluirEnderecoCliente(int id)
        {
            if(db.PedidosExternos.Any(p => p.CodEnderecoEntrega == id))
            {
                return Json(false);
            }
            return Json(true);
        }

        // POST: EnderecoClientes/Delete/5        
        public ActionResult Delete(int id, string returnUrl)
        {
            try
            {
                EnderecoCliente enderecoCliente = db.EnderecosClientes.Find(id);
                db.EnderecosClientes.Remove(enderecoCliente);
                db.SaveChanges();                
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não foi possível excluir este endereço pois ele já recebeu pelo menos um pedido!");                
            }
            return Redirect(returnUrl);
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

    public class EnderecoClienteViewModel
    {
        public EnderecoClienteViewModel()
        {

        }

        public EnderecoClienteViewModel(EnderecoCliente endereco)
        {
            CodCliente = endereco.CodCliente;
            Endereco = endereco.Endereco;
            Bairro = endereco.Bairro;
            CodCidade = endereco.CodCidade;
        }

        public int CodCliente { get; set; }

        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Bairro { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int? CodCidade { get; set; }

        [Display(Name = "Entrega: R$")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal ValorEntregaPadrao { get; set; }

        [Display(Name = "Observações")]
        public string ObservacoesPadrao { get; set; }
    }
}


// GET: EnderecoClientes
/*public ActionResult Index()
{
    var enderecosClientes = db.EnderecosClientes.Include(e => e.Cidade).Include(e => e.Cliente);
    return View(enderecosClientes.ToList());
}

// GET: EnderecoClientes/Details/5
public ActionResult Details(int? id)
{
    if (id == null)
    {
        return BadRequest();
    }
    EnderecoCliente enderecoCliente = db.EnderecosClientes.Find(id);
    if (enderecoCliente == null)
    {
        return NotFound();
    }
    return View(enderecoCliente);
}*/
