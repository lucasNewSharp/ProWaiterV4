using ProWaiter.Web.AutenticacaoAPI;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.Gestores;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProWaiter.Web.APIs
{
    [IdentityBasicAuthentication]
    public class CriarPedidoInternoController : ApiController
    {
        public class MesaDTO
        {
            public PedidoInterno UltimoPedido { get; set; }
            public short Codigo { get; set; }
            public string Descricao { get; set; }
            public int? CodUltimoPedido { get; set; }
            public string Observacoes { get; set; }
        }

        private ProWaiterContext db = new ProWaiterContext();

        // POST: api/CriarPedidoInterno
        [ResponseType(typeof(MesaDTO))]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IHttpActionResult Post(MesaDTO mesaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mesa mesa = null;
            try
            {
                mesa = db.Mesas.Where(m => m.Codigo == mesaDTO.Codigo).Single();
                Pedido pedido = (new GestorPedidos(db)).CriarPedido(User.Identity, mesa, mesaDTO.Observacoes);
                mesaDTO.Codigo = mesa.Codigo;
                mesaDTO.CodUltimoPedido = mesa.CodUltimoPedido;
                mesaDTO.UltimoPedido = (PedidoInterno)mesa.UltimoPedido;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

            return Ok(mesaDTO);
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
