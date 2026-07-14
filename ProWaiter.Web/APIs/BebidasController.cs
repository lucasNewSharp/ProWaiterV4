using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;

namespace ProWaiter.Web.APIs
{
    // [IdentityBasicAuthentication]
    public class BebidasController : ControllerBase
    {
        private readonly ProWaiterContext db = new ProWaiterContext();
        
        [ProducesResponseType(typeof(Bebida), 200)]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IActionResult GetBebidas(short codTipoBebida)
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
