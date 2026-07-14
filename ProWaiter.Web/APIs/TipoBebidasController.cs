using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System.Threading.Tasks;
using ProWaiter.Web.AutenticacaoAPI;

namespace ProWaiter.Web.APIs
{
    [IdentityBasicAuthentication]
    public class TipoBebidasController : ApiController
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}