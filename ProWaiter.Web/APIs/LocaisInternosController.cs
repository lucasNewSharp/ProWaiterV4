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
    public class LocaisInternosController : ControllerBase
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        [ProducesResponseType(typeof(LocalInterno), 200)]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IActionResult GetLocaisInternos()
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
