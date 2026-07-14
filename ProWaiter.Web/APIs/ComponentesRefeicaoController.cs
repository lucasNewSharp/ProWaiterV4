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
    public class ComponentesRefeicaoController : ApiController
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/ComponenteRefeicaos/5        
        [ResponseType(typeof(ComponenteRefeicao))]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IHttpActionResult GetComponentesRefeicao(short codRefeicao)
        {
            try
            {
                Refeicao refeicao = db.Refeicoes.Where(r => r.Codigo == codRefeicao).SingleOrDefault();
                if (refeicao == null)
                {
                    return NotFound();
                }
                return Ok(refeicao.ComponentesRefeicao.OrderBy(c => c.Nome).ToList());
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