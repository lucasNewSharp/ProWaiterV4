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
    public class ComponentesRefeicaoController : ControllerBase
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/ComponenteRefeicaos/5        
        [ProducesResponseType(typeof(ComponenteRefeicao), 200)]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IActionResult GetComponentesRefeicao(short codRefeicao)
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
