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
    public class ComponentesComposicaoRefeicaoCardapioController : ApiController
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/ComponenteRefeicaos/5        
        [ResponseType(typeof(ComponenteComposicaoRefeicaoCardapio))]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IHttpActionResult GetComponentesComposicaoRefeicaoCardapio(short codRefeicao, string codTamanho)
        {
            try
            {
                RefeicaoDoCardapio refeicao = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == codRefeicao && r.CodTamanho == codTamanho).SingleOrDefault();
                if (refeicao == null)
                {
                    return NotFound();
                }
                return Ok(refeicao.ComponentesComposicaoRefeicao.OrderBy(c => c.ComponenteRefeicao.Nome).ToList());
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