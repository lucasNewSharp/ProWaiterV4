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
    public class ComponentesComposicaoRefeicaoCardapioController : ControllerBase
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/ComponenteRefeicaos/5        
        [ProducesResponseType(typeof(ComponenteComposicaoRefeicaoCardapio), 200)]
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IActionResult GetComponentesComposicaoRefeicaoCardapio(short codRefeicao, string codTamanho)
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
