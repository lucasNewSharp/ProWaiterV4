using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProWaiter.Web.Controllers
{
    public class PedidosExternosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pedidos");
        }

        #region Details

        // GET: PedidosExternos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoExterno pedidoExterno = db.PedidosExternos.Find(id);
            if (pedidoExterno == null)
            {
                return HttpNotFound();
            }
            ViewBag.ErrosImpressao = TempData["ErrosImpressao"];
            return View(pedidoExterno);
        }

        #endregion

        #region Create

        // GET: PedidosExternos/Create
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create(int? codClienteSelecionado = null, int? codEnderecoSelecionado = null)
        {
            var pedExterno = new PedidoExterno();

            ViewBag.CodCliente = new SelectList(db.Clientes.OrderBy(c => c.Nome), "Codigo", "Nome", codClienteSelecionado);

            Cliente cliente = null;
            if (codClienteSelecionado.HasValue)
            {
                cliente = db.Clientes.Where(c => c.Codigo == codClienteSelecionado.Value).Single();                
            }
            else
            {
                cliente = db.Clientes.OrderBy(c => c.Nome).FirstOrDefault();             
            }

            if (cliente != null)
            {
                pedExterno.Cliente = cliente;                

                EnderecoCliente endereco = null;
                if (codEnderecoSelecionado.HasValue)
                {
                    endereco = cliente.Enderecos.Where(e => e.Codigo == codEnderecoSelecionado).SingleOrDefault();
                }
                else
                {
                    endereco = cliente.Enderecos.FirstOrDefault();
                }

                if (endereco != null)
                {
                    pedExterno.EnderecoCliente = endereco;
                    pedExterno.ValorEntrega = endereco.ValorEntregaPadrao;
                    pedExterno.Observacoes = endereco.ObservacoesPadrao;
                }

                ViewBag.CodEnderecoEntrega = new SelectList(cliente.Enderecos.OrderBy(c => c.Endereco), "Codigo", "Endereco", codEnderecoSelecionado);
            }

            return View(pedExterno);
        }

        // GET: PedidosExternos/Create
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        [ActionName("CreatePeloTelefone")]
        public ActionResult Create()
        {
            string telefone = Configuracoes.ObterInstancia().ObterUltimoTelefoneDetectado(db);

            Cliente cliente = null;
            if (string.IsNullOrWhiteSpace(telefone))
            {
                cliente = db.Clientes.OrderBy(c => c.Nome).FirstOrDefault();
            }
            else
            {
                cliente = db.Clientes.Where(c => c.Telefone1.Contains(telefone) || c.Telefone2.Contains(telefone) || c.Nome.Contains(telefone)).FirstOrDefault();
            }

            var pedExterno = new PedidoExterno();

            ViewBag.CodCliente = new SelectList(db.Clientes.OrderBy(c => c.Nome), "Codigo", "Nome", cliente?.Codigo);

            if (cliente != null)
            {
                pedExterno.Cliente = cliente;                

                EnderecoCliente endereco = cliente.Enderecos.FirstOrDefault();
                int? codEnderecoSelecionado = null;
                if (endereco != null)
                {
                    pedExterno.EnderecoCliente = endereco;
                    codEnderecoSelecionado = endereco.Codigo;
                    pedExterno.ValorEntrega = endereco.ValorEntregaPadrao;
                    pedExterno.Observacoes = endereco.ObservacoesPadrao;
                }

                ViewBag.CodEnderecoEntrega = new SelectList(cliente.Enderecos.OrderBy(c => c.Endereco), "Codigo", "Endereco", codEnderecoSelecionado);
            }
            else
            {
                ViewBag.NovoTelefoneCadastro = telefone;
            }

            return View("Create", pedExterno);
        }

        [HttpPost]
        public ActionResult FiltrarClientes(string filtro)
        {
            var clientes = db.Clientes
                .Where(c => c.Nome.Contains(filtro) || (!string.IsNullOrEmpty(c.Telefone1) && c.Telefone1.Contains(filtro)) || (!string.IsNullOrEmpty(c.Telefone2) && c.Telefone2.Contains(filtro)))
                .OrderBy(c => c.Nome)
                .Select(c => new { Codigo = c.Codigo, Nome = c.Nome });

            return Json(clientes);
        }

        [HttpPost]
        public ActionResult LoadDadosCliente(int codCliente)
        {
            Cliente cliente = db.Clientes.Where(c => c.Codigo == codCliente).Single();

            List<KeyValuePair<string, string>> enderecos = new List<KeyValuePair<string, string>>();
            foreach (var end in cliente.Enderecos)
                enderecos.Add(new KeyValuePair<string, string>(end.Codigo.ToString(), end.Endereco));

            EnderecoCliente endereco = cliente.Enderecos.FirstOrDefault();
            JsonResult res = Json(new
            {
                Nome = cliente.Nome,
                Telefone1 = cliente.Telefone1,
                Telefone2 = cliente.Telefone2,
                Enderecos = enderecos,
                
                Endereco = endereco?.Endereco,
                Bairro = endereco?.Bairro,
                Cidade = endereco?.Cidade?.Nome,
                CodCidade = endereco?.Cidade.Codigo.ToString(),
                ValorEntregaPadrao = endereco?.ValorEntregaPadrao,
                ObservacoesPadrao = endereco?.ObservacoesPadrao
            });
            return res;
        }

        [HttpPost]
        public ActionResult LoadEnderecoCliente(int codCliente, int codEndereco)
        {
            Cliente cliente = db.Clientes.Where(c => c.Codigo == codCliente).Single();
            EnderecoCliente endereco = cliente.Enderecos.Where(e => e.Codigo == codEndereco).Single();

            JsonResult res = Json(new
            {
                Endereco = endereco?.Endereco,
                Bairro = endereco?.Bairro,
                Cidade = endereco?.Cidade?.Nome,
                CodCidade = endereco?.Cidade?.Codigo.ToString(),
                ValorEntregaPadrao = endereco?.ValorEntregaPadrao,
                ObservacoesPadrao = endereco?.ObservacoesPadrao
            });
            return res;
        }

        // POST: PedidosExternos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Create([Bind(Include = "CodCliente, ValorEntrega, Observacoes,CodEnderecoEntrega")] PedidoExterno pedidoExterno)
        {
            return CreateHelper(pedidoExterno);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        [ActionName("CreatePeloTelefone")]
        public ActionResult CreatePeloTelefone([Bind(Include = "CodCliente, ValorEntrega, Observacoes,CodEnderecoEntrega")] PedidoExterno pedidoExterno)
        {
            return CreateHelper(pedidoExterno);
        }

        private ActionResult CreateHelper(PedidoExterno pedidoExterno)
        {
            if (ModelState.IsValid && pedidoExterno.CodCliente != 0)
            {
                GerarSequencialDaTeleNoDia(pedidoExterno);
                db.PedidosExternos.Add(pedidoExterno);
                pedidoExterno.NomeUsuario = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Edit", "Pedidos", new { Id = pedidoExterno.Codigo });
            }
            if (pedidoExterno.Cliente == null)
                ModelState.AddModelError("", "Não é possível criar um pedido externo pois o cliente não foi selecionado!");

            ViewBag.CodCliente = new SelectList(db.Clientes, "Codigo", "Nome", pedidoExterno.CodCliente);
            return View(pedidoExterno);
        }

        #endregion

        #region Delete

        // GET: PedidosExternos/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoExterno pedidoExterno = db.PedidosExternos.Find(id);
            if (pedidoExterno == null)
            {
                return HttpNotFound();
            }
            return View(pedidoExterno);
        }

        // POST: PedidosExternos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores + "," + Constantes.GrupoCaixas)]
        public ActionResult DeleteConfirmed(int id)
        {
            PedidoExterno pedidoExterno = db.PedidosExternos.Find(id);
            db.PedidosExternos.Remove(pedidoExterno);
            db.SaveChanges();
            return RedirectToAction("Index", "Pedidos");
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        public void GerarSequencialDaTeleNoDia(PedidoExterno pedido)
        {
            short sequencial = 0;
            DateTime dtInicio = new DateTime(pedido.DataInicio.Year, pedido.DataInicio.Month, pedido.DataInicio.Day);
            DateTime dtFim = dtInicio.AddDays(1);
            PedidoExterno pedidoDB = db.PedidosExternos.Where(p => p.DataInicio > dtInicio && p.DataInicio < dtFim).OrderBy(p => p.Codigo).ToList().LastOrDefault();
            if (pedidoDB == null)
                sequencial = 1;
            else
                sequencial = (short)(pedidoDB.SequencialNoDia + 1);

            pedido.SequencialNoDia = sequencial;
        }
    }
}
