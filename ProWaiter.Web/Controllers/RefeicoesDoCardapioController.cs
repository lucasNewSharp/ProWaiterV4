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
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Gestores;

namespace ProWaiter.Web.Controllers
{
    public class RefeicoesDoCardapioController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: RefeicoesDoCardapio
        public ActionResult Index()
        {
            var refeicoesDoCardapio = db.RefeicoesDoCardapio
                .Include(r => r.Refeicao)
                .Include(r => r.TamanhoRefeicao)
                .OrderBy(r => r.Refeicao.Nome)
                .ThenBy(r => r.TamanhoRefeicao.Nome);
            return View(refeicoesDoCardapio.ToList());
        }

        // GET: RefeicoesDoCardapio/Details/5
        public ActionResult Details(short codRefeicao, string codTamanho)
        {
            RefeicaoDoCardapio refeicaoDoCardapio = db.RefeicoesDoCardapio.Find(new object[] { codRefeicao, codTamanho });
            if (refeicaoDoCardapio == null)
                return NotFound();
            return View(refeicaoDoCardapio);
        }

        // GET: RefeicoesDoCardapio/Create
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create(string refeCarga, string tamSelecionado, byte? codImpressoraSelecionada)
        {
            var vm = new RefeicaoDoCardapioViewModel()
            {
                Ativo = true,
                ComponentesDeComposicao = ObterTodosComponentesRefeicaoDeComposicao()
            };

            if (!string.IsNullOrWhiteSpace(refeCarga))
            {
                short codRefeicao = short.Parse(refeCarga.Split('-')[0]);
                string codTamanho = refeCarga.Split('-')[1];

                RefeicaoDoCardapio refeParaCarregar = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == codRefeicao && r.CodTamanho == codTamanho).Single();

                List<UnidadeComponenteComposicao> listaUnidades = db.UnidadesComponenteComposicao.ToList().OrderBy(u => u.Descricao).ToList();
                foreach (var refVM in vm.ComponentesDeComposicao)
                {
                    ComponenteComposicaoRefeicaoCardapio comp = refeParaCarregar.ComponentesComposicaoRefeicao.Where(c => c.CodComponente == refVM.CodComponente).SingleOrDefault();
                    if (comp != null)
                    {
                        refVM.Ativo = true;
                        refVM.CalculoProporcional = comp.CalculoProporcional;
                        refVM.Valor = 0;
                        refVM.NomeComponente = comp.ComponenteRefeicao.Nome;
                        refVM.ListaUnidades = new SelectList(listaUnidades, "Codigo", "Descricao", comp.CodUnidade);
                    }
                }

                vm.DeComposicao = true;
                vm.CodRefeicao = codRefeicao;
                vm.CodTamanho = tamSelecionado;
                vm.CodImpressora = codImpressoraSelecionada.Value;
                refeCarga = null;
            }
            CarregarListasCadastro(vm);

            return View(vm);
        }

        // POST: RefeicoesDoCardapio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Create([Bind("CodRefeicao,CodTamanho,Valor,Ativo,CodImpressora,DeComposicao,ComponentesDeComposicao,RefeicaoParaCargaSelecionada,PercDesconto,CodBarras")] RefeicaoDoCardapioViewModel refeicaoDoCardapio)
        {
            if (refeicaoDoCardapio.RefeicaoParaCargaSelecionada != null)
            {
                return RedirectToAction("Create", new { refeCarga = refeicaoDoCardapio.RefeicaoParaCargaSelecionada, tamSelecionado = refeicaoDoCardapio.CodTamanho, codImpressoraSelecionada = refeicaoDoCardapio.CodImpressora });
            }

            foreach (var comp in refeicaoDoCardapio.ComponentesDeComposicao)
            {
                if (comp.Ativo && comp.Valor == 0)
                {
                    ModelState.AddModelError("", "Exitem componentes ativos com valor 0,00");
                    break;
                }
            }

            if (ModelState.IsValid)
            {
                IItemCodigoBarras itemCodigoBarras = GestorItemCodBarras.ObterItemCodBarras(refeicaoDoCardapio.CodBarras, false);

                if (itemCodigoBarras != null)
                    ModelState.AddModelError("", $"Já existe uma item com este código de barras: {itemCodigoBarras.Nome}");
                else
                if (db.RefeicoesDoCardapio.Any(r => r.CodRefeicao == refeicaoDoCardapio.CodRefeicao && r.CodTamanho == refeicaoDoCardapio.CodTamanho))
                    ModelState.AddModelError("", "Já existe esta refeição do cardápio com este tamanho");
                else if (refeicaoDoCardapio.DeComposicao && refeicaoDoCardapio.Valor > 0)
                    ModelState.AddModelError("", "Você não pode setar um valor se o item for de composição, o valor vai em cada item");
                else
                {
                    List<ComponenteComposicaoRefeicaoCardapio> lista = new List<ComponenteComposicaoRefeicaoCardapio>();

                    if (refeicaoDoCardapio.DeComposicao)
                    {
                        foreach (var comp in refeicaoDoCardapio.ComponentesDeComposicao.Where(r => r.Ativo))
                        {
                            lista.Add(new ComponenteComposicaoRefeicaoCardapio()
                            {
                                CodRefeicao = refeicaoDoCardapio.CodRefeicao,
                                CodTamanho = refeicaoDoCardapio.CodTamanho,
                                Ativo = comp.Ativo,
                                CalculoProporcional = comp.CalculoProporcional,
                                CodComponente = comp.CodComponente,
                                Valor = comp.Valor,
                                CodUnidade = comp.CodUnidade
                            });
                        }
                    }

                    RefeicaoDoCardapio refDoCar = new RefeicaoDoCardapio()
                    {
                        CodRefeicao = refeicaoDoCardapio.CodRefeicao,
                        Ativo = refeicaoDoCardapio.Ativo,
                        CodImpressora = refeicaoDoCardapio.CodImpressora,
                        CodTamanho = refeicaoDoCardapio.CodTamanho,
                        DeComposicao = refeicaoDoCardapio.DeComposicao,
                        Valor = refeicaoDoCardapio.Valor,
                        PercDesconto = refeicaoDoCardapio.PercDesconto,
                        ComponentesComposicaoRefeicao = lista,
                        CodBarras = refeicaoDoCardapio.CodBarras
                    };
                    db.RefeicoesDoCardapio.Add(refDoCar);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            List<UnidadeComponenteComposicao> listaUnidades = db.UnidadesComponenteComposicao.ToList().OrderBy(u => u.Descricao).ToList();
            foreach (ComponenteDeComposicaoViewModel compVm in refeicaoDoCardapio.ComponentesDeComposicao)
            {
                compVm.ListaUnidades = new SelectList(listaUnidades, "Codigo", "Descricao", compVm.CodUnidade);
            }

            CarregarListasCadastro(refeicaoDoCardapio);

            return View(refeicaoDoCardapio);
        }

        private void CarregarListasCadastro(RefeicaoDoCardapioViewModel refeicaoDoCardapio)
        {
            ViewBag.CodRefeicao = new SelectList(db.Refeicoes.OrderBy(r => r.Nome), "Codigo", "Nome", refeicaoDoCardapio.CodRefeicao);
            ViewBag.CodTamanho = new SelectList(db.TamanhosRefeicao, "Codigo", "Nome", refeicaoDoCardapio.CodTamanho);
            ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(i => i.NomeExibicao), "Codigo", "NomeExibicao", refeicaoDoCardapio.CodImpressora);

            List<RefeicaoDoCardapio> refeComp = db.RefeicoesDoCardapio.Where(r => r.DeComposicao).ToList();
            List<RefeicaoParaCarga> select = new List<RefeicaoParaCarga>();
            foreach (var rc in refeComp)
            {
                select.Add(new RefeicaoParaCarga()
                {
                    Codigo = rc.CodRefeicao.ToString() + "-" + rc.CodTamanho,
                    NomeRefeicao = rc.Refeicao.Nome + " - " + rc.TamanhoRefeicao.Nome
                });
            }
            refeicaoDoCardapio.RefeicoesJaCadastradas = new SelectList(select, "Codigo", "NomeRefeicao");
        }

        // GET: RefeicoesDoCardapio/Edit/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit(short codRefeicao, string codTamanho)
        {
            RefeicaoDoCardapio refeicaoDoCardapio = db.RefeicoesDoCardapio.Find(new object[] { codRefeicao, codTamanho });
            if (refeicaoDoCardapio == null)
            {
                return NotFound();
            }

            RefeicaoDoCardapioViewModel refDoCardVM = new RefeicaoDoCardapioViewModel()
            {
                Ativo = refeicaoDoCardapio.Ativo,
                CodImpressora = refeicaoDoCardapio.CodImpressora,
                CodRefeicao = refeicaoDoCardapio.CodRefeicao,
                NomeRefeicao = refeicaoDoCardapio.Refeicao.Nome,
                CodTamanho = refeicaoDoCardapio.CodTamanho,
                NomeTamanho = refeicaoDoCardapio.TamanhoRefeicao.Nome,
                DeComposicao = refeicaoDoCardapio.DeComposicao,
                Valor = refeicaoDoCardapio.Valor,
                PercDesconto = refeicaoDoCardapio.PercDesconto,
                CodBarras = refeicaoDoCardapio.CodBarras,
                ComponentesDeComposicao = ObterTodosComponentesRefeicaoDeComposicao()
            };

            List<UnidadeComponenteComposicao> listaUnidades = db.UnidadesComponenteComposicao.ToList().OrderBy(u => u.Descricao).ToList();
            foreach (var cc in refeicaoDoCardapio.ComponentesComposicaoRefeicao)
            {
                var ccVM = refDoCardVM.ComponentesDeComposicao.Where(c => c.CodComponente == cc.CodComponente).Single();
                ccVM.Ativo = cc.Ativo;
                ccVM.CalculoProporcional = cc.CalculoProporcional;
                ccVM.CodComponente = cc.CodComponente;
                ccVM.NomeComponente = cc.ComponenteRefeicao.Nome;
                ccVM.Valor = cc.Valor;
                ccVM.CodUnidade = cc.CodUnidade;
                ccVM.ListaUnidades = new SelectList(listaUnidades, "Codigo", "Descricao", cc.CodUnidade);
            }

            ViewBag.CodRefeicao = new SelectList(db.Refeicoes.OrderBy(r => r.Nome), "Codigo", "Nome", refeicaoDoCardapio.CodRefeicao);
            ViewBag.CodTamanho = new SelectList(db.TamanhosRefeicao, "Codigo", "Nome", refeicaoDoCardapio.CodTamanho);
            ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(i => i.NomeExibicao), "Codigo", "NomeExibicao", refeicaoDoCardapio.CodImpressora);

            return View(refDoCardVM);
        }

        // POST: RefeicoesDoCardapio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Edit([Bind("CodRefeicao,CodTamanho,Valor,Ativo,CodImpressora,DeComposicao,ComponentesDeComposicao,PercDesconto,CodBarras")] RefeicaoDoCardapioViewModel refeicaoDoCardapioVM)
        {
            if (ModelState.IsValid)
            {
                IItemCodigoBarras item = GestorItemCodBarras.ObterItemCodBarras(refeicaoDoCardapioVM.CodBarras, false);

                if (item != null &&
                    ((item is RefeicaoDoCardapio refCard && (refCard.CodRefeicao != refeicaoDoCardapioVM.CodRefeicao && refCard.CodTamanho != refeicaoDoCardapioVM.CodTamanho)) || !(item is RefeicaoDoCardapio)))
                {
                    ModelState.AddModelError("", $"Já existe um item com este código de barras: {item.Nome}!");
                }
                else
                {
                    RefeicaoDoCardapio refDoCard = null;

                    refDoCard = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == refeicaoDoCardapioVM.CodRefeicao && r.CodTamanho == refeicaoDoCardapioVM.CodTamanho).Single();
                    refDoCard.Ativo = refeicaoDoCardapioVM.Ativo;
                    refDoCard.CodImpressora = refeicaoDoCardapioVM.CodImpressora;
                    refDoCard.DeComposicao = refeicaoDoCardapioVM.DeComposicao;
                    refDoCard.Valor = refeicaoDoCardapioVM.Valor;
                    refDoCard.PercDesconto = refeicaoDoCardapioVM.PercDesconto;
                    refDoCard.CodBarras = refeicaoDoCardapioVM.CodBarras;

                    refDoCard.ComponentesComposicaoRefeicao.Clear();

                    if (refeicaoDoCardapioVM.DeComposicao)
                    {
                        foreach (var comp in refeicaoDoCardapioVM.ComponentesDeComposicao.Where(c => c.Ativo))
                        {
                            ComponenteComposicaoRefeicaoCardapio compBD = new ComponenteComposicaoRefeicaoCardapio()
                            {
                                CodRefeicao = refDoCard.CodRefeicao,
                                CodTamanho = refDoCard.CodTamanho,
                                CodComponente = comp.CodComponente,
                                Ativo = comp.Ativo,
                                CalculoProporcional = comp.CalculoProporcional,
                                Valor = comp.Valor,
                                CodUnidade = comp.CodUnidade
                            };
                            refDoCard.ComponentesComposicaoRefeicao.Add(compBD);
                        }
                    }

                    db.Entry(refDoCard).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            List<UnidadeComponenteComposicao> listaUnidades = db.UnidadesComponenteComposicao.ToList().OrderBy(u => u.Descricao).ToList();
            foreach (var cc in refeicaoDoCardapioVM.ComponentesDeComposicao)
            {
                cc.ListaUnidades = new SelectList(listaUnidades, "Codigo", "Descricao", cc.CodUnidade);
            }

            var refeDB = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == refeicaoDoCardapioVM.CodRefeicao && r.CodTamanho == refeicaoDoCardapioVM.CodTamanho).Single();
            refeicaoDoCardapioVM.NomeRefeicao = refeDB.Nome;
            refeicaoDoCardapioVM.NomeTamanho = refeDB.TamanhoRefeicao.Nome;

            ViewBag.CodRefeicao = new SelectList(db.Refeicoes.OrderBy(r => r.Nome), "Codigo", "Nome", refeicaoDoCardapioVM.CodRefeicao);
            ViewBag.CodTamanho = new SelectList(db.TamanhosRefeicao, "Codigo", "Nome", refeicaoDoCardapioVM.CodTamanho);
            ViewBag.CodImpressora = new SelectList(db.Impressoras.ToList().OrderBy(i => i.NomeExibicao), "Codigo", "NomeExibicao", refeicaoDoCardapioVM.CodImpressora);
            return View(refeicaoDoCardapioVM);
        }

        private List<ComponenteDeComposicaoViewModel> ObterTodosComponentesRefeicaoDeComposicao()
        {
            List<UnidadeComponenteComposicao> lista = db.UnidadesComponenteComposicao.ToList().OrderBy(u => u.Descricao).ToList();

            List<ComponenteDeComposicaoViewModel> componentesDisponiveis = new List<ComponenteDeComposicaoViewModel>();
            foreach (var comp in db.ComponentesRefeicao.OrderBy(c => c.Nome))
                componentesDisponiveis.Add(new ComponenteDeComposicaoViewModel()
                {
                    CodComponente = comp.Codigo,
                    NomeComponente = comp.Nome,
                    CalculoProporcional = false,
                    Valor = 0,
                    Ativo = false,
                    CodUnidade = null,
                    ListaUnidades = new SelectList(lista, "Codigo", "Descricao")
                });

            return componentesDisponiveis;
        }

        // GET: RefeicoesDoCardapio/Delete/5
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Delete(short codRefeicao, string codTamanho)
        {
            RefeicaoDoCardapio refeicaoDoCardapio = db.RefeicoesDoCardapio.Find(new object[] { codRefeicao, codTamanho });
            if (refeicaoDoCardapio == null)
            {
                return NotFound();
            }
            return View(refeicaoDoCardapio);
        }

        // POST: RefeicoesDoCardapio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult DeleteConfirmed(short codRefeicao, string codTamanho)
        {
            RefeicaoDoCardapio refeicaoDoCardapio = db.RefeicoesDoCardapio.Find(new object[] { codRefeicao, codTamanho });
            try
            {
                db.RefeicoesDoCardapio.Remove(refeicaoDoCardapio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Não é possível remover a refeição do cardápio pois ela já foi utilizada em pelo menos um pedido!");
                return View(refeicaoDoCardapio);
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
