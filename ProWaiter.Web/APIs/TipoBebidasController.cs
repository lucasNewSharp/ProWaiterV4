using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System.Threading.Tasks;


namespace ProWaiter.Web.APIs
{
    // [IdentityBasicAuthentication]
    public class TipoBebidasController : ControllerBase
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        // GET: api/TipoBebidas
        public IQueryable<TipoBebida> GetTiposBebida()
        {
            try
            {
                return db.Bebidas.Where(b => b.Ativo).Select(be => be.Tipo).Distinct().OrderBy(t => t.Nome);
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
