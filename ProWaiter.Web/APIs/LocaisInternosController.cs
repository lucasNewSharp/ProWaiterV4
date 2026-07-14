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
    public class LocaisInternosController : ApiController
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        [ResponseType(typeof(LocalInterno))]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IHttpActionResult GetLocaisInternos()
        {
            try
            {
                return Ok(db.LocaisInternos.OrderBy(l => l.Nome).ToList());
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