
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ProWaiter.Web.Controllers
{
    public class ConfiguracaoDescontosController : Controller
    {
        private ProWaiterContext db = new ProWaiterContext();

        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Index()
        {
            ConfiguracaoDescontosViewModel confDesc = new ConfiguracaoDescontosViewModel();

            db.Bebidas.Where(b => b.Ativo).OrderBy(b => b.Nome)
                .ToList().ForEach(b => confDesc.Bebidas.Add(new ConfiguracaoDescontosViewModel.BebidaVM(b)));

            db.RefeicoesDoCardapio.Where(r => r.Ativo).OrderBy(r => r.Refeicao.Nome)
                .ToList().ForEach(r => confDesc.Refeicoes.Add(new ConfiguracaoDescontosViewModel.RefeicaoCardapioVM(r)));

            db.ItensBacao.Where(i => i.Ativo)
                .ToList().ForEach(i => confDesc.ItensBalcao.Add(new ConfiguracaoDescontosViewModel.ItemBalcaoVM(i)));

            return View(confDesc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.GrupoAdministradores)]
        public ActionResult Salvar(ConfiguracaoDescontosViewModel confDesc)
        {
            try
            {
                foreach(var bebVM in confDesc.Bebidas)
                {
                    Bebida beb = db.Bebidas.Find(bebVM.Codigo);
                    if(beb == null)
                    {
                        throw new ApplicationException("Erro ao tentar obter Bebida");
                    }
                    if(beb.PercDesconto != bebVM.PercDesconto)
                    {
                        beb.PercDesconto = bebVM.PercDesconto;
                        db.Entry(beb).State = EntityState.Modified;
                    }
                }

                foreach(var refVM in confDesc.Refeicoes)
                {
                    RefeicaoDoCardapio refCardapio = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == refVM.CodRefeicao && r.CodTamanho ==  refVM.CodTamanho).SingleOrDefault();
                    if(refCardapio == null)
                    {
                        throw new ApplicationException("Erro ao tentar obter Bebida");
                    }
                    if(refCardapio.PercDesconto != refVM.PercDesconto)
                    {
                        refCardapio.PercDesconto = refVM.PercDesconto;
                        db.Entry(refCardapio).State = EntityState.Modified;
                    }
                }
                
                foreach(var ibVM in confDesc.ItensBalcao)
                {
                    ItemBalcao ite = db.ItensBacao.Where(i => i.Codigo == ibVM.Codigo).SingleOrDefault();
                    if(ite == null)
                    {
                        throw new ApplicationException("Erro ao tentar obter o item de balcão");
                    }
                    if(ite.PercDesconto != ibVM.PercDesconto)
                    {
                        ite.PercDesconto = ibVM.PercDesconto;
                        db.Entry(ite).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return RedirectToAction("Index");
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
