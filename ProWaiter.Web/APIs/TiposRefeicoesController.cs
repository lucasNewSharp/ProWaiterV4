using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;

namespace ProWaiter.Web.APIs
{
    // [IdentityBasicAuthentication]
    public class TiposRefeicoesController : ControllerBase
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        // GET: api/TiposRefeicoes
        public IQueryable<TipoRefeicao> GetTiposRefeicao()
        {
            try
            {
                return db.RefeicoesDoCardapio.Where(r => r.Ativo).Select(r => r.Refeicao.Tipo).Distinct().OrderBy(t => t.Nome);
            }
            catch
            {
                return null;
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
