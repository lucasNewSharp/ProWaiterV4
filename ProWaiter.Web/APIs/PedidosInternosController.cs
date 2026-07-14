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

namespace ProWaiter.Web.APIs
{
    // [IdentityBasicAuthentication]
    public class PedidosInternosController : ControllerBase
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/PedidosInternos
        public IQueryable<PedidoInterno> GetPedidos()
        {
            return db.PedidosInternos;
        }

        // GET: api/PedidosInternos/5
        [ProducesResponseType(typeof(PedidoInterno), 200)]
        public IActionResult GetPedidoInterno(int id)
        {
            PedidoInterno pedidoInterno = db.PedidosInternos.Find(id);
            if (pedidoInterno == null)
            {
                return NotFound();
            }

            return Ok(pedidoInterno);
        }

        // PUT: api/PedidosInternos/5
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult PutPedidoInterno(int id, PedidoInterno pedidoInterno)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pedidoInterno.Codigo)
            {
                return BadRequest();
            }

            db.Entry(pedidoInterno).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoInternoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(204);
        }

        // POST: api/PedidosInternos
        [ProducesResponseType(typeof(PedidoInterno), 200)]
        public IActionResult PostPedidoInterno(PedidoInterno pedidoInterno)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pedidoInterno.NomeUsuario = User.Identity.Name;
            db.PedidosInternos.Add(pedidoInterno);
            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                string a = ex.ToString();
            }

            return CreatedAtRoute("DefaultApi", new { id = pedidoInterno.Codigo }, pedidoInterno);
        }

        // DELETE: api/PedidosInternos/5
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult DeletePedidoInterno(int id)
        {
            try
            {                
                PedidoInterno pedidoInterno = db.PedidosInternos.Where(p => p.Codigo == id).SingleOrDefault();
                if (pedidoInterno == null)
                {
                    return StatusCode(200); //Retornamos OK, pois o pedido pode já ter sido fechado, ou outro dispositivo o removeu.
                }

                Mesa mesa = db.Mesas.Where(m => m.CodUltimoPedido == id).SingleOrDefault();
                if (mesa != null)
                    mesa.UltimoPedido = null;

                db.PedidosInternos.Remove(pedidoInterno);
                db.SaveChanges();
                return StatusCode(200);
            }
            catch
            {
                return StatusCode(400);
            }
        }

//         // // protected void Dispose(bool disposing)
//         {
//             if (disposing)
//             {
//                 // // db.Dispose();
//             }
//             // base.Dispose(disposing);
//         }

        private bool PedidoInternoExists(int id)
        {
            return db.PedidosInternos.Count(e => e.Codigo == id) > 0;
        }
    }
}
