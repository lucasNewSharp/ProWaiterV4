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
    public class ModelosPedidoController : ApiController
    {
        private ProWaiterContext db = new ProWaiterContext();

        //Exemplo de api nomeada
        // GET: api/ModeloPedidos
        [Route("api/ModelosPedido/ExisteModelo")]
        [HttpGet]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        [ResponseType(typeof(bool))]
        public bool ExisteModelo()
        {
            return db.ModelosPedidos.Any();
        }

        // GET: api/ModeloPedidos
        public IQueryable<ModeloPedido> GetModelosPedidos()
        {
            return db.ModelosPedidos;
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