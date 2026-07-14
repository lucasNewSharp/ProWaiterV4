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
using ProWaiter.Web.AutenticacaoAPI;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.APIs
{
    [IdentityBasicAuthentication]
    public class BebidasController : ApiController
    {
        private readonly ProWaiterContext db = new ProWaiterContext();
        
        [ResponseType(typeof(Bebida))]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IHttpActionResult GetBebidas(short codTipoBebida)
        {
            try
            {
                return Ok(db.Bebidas.Where(b => b.CodTipo == codTipoBebida && b.Ativo).OrderBy(b => b.Nome).ToList());
            }
            catch
            {
                return BadRequest();
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