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
using ProWaiter.Web.Util;

namespace ProWaiter.Web.APIs
{
    [IdentityBasicAuthentication]
    public class MesasController : ApiController
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        [Authorize(Roles = Constantes.GrupoGarcons)]
        // GET: api/Mesas
        public IQueryable<Mesa> GetMesas()
        {            
            try
            {
                return db.Mesas.OrderBy(m => m.Descricao);
            }
            catch
            {
                return null;
            }
        }
        
        // GET: api/Mesas/5
        [Authorize(Roles = Constantes.GrupoGarcons)]
        [ResponseType(typeof(Mesa))]
        public IHttpActionResult GetMesa(short id)
        {
            try
            {
                Mesa mesa = db.Mesas.Find(id);
                if (mesa == null)
                {
                    return NotFound();
                }
                return Ok(mesa);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
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