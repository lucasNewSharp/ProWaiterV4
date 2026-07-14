using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ProWaiter.Web.AutenticacaoAPI;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;

namespace ProWaiter.Web.APIs
{
    [IdentityBasicAuthentication]
    public class TiposRefeicoesController : ApiController
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