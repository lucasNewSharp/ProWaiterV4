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
using ProWaiter.Web.Util;

namespace ProWaiter.Web.APIs
{
    // [IdentityBasicAuthentication]
    public class ModelosPedidoController : ControllerBase
    {
        private ProWaiterContext db = new ProWaiterContext();

        //Exemplo de api nomeada
        // GET: api/ModeloPedidos
        [Route("api/ModelosPedido/ExisteModelo")]
        [HttpGet]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        [ProducesResponseType(typeof(bool), 200)]
        public bool ExisteModelo()
        {
            return db.ModelosPedidos.Any();
        }

        // GET: api/ModeloPedidos
        public IQueryable<ModeloPedido> GetModelosPedidos()
        {
            return db.ModelosPedidos;
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
