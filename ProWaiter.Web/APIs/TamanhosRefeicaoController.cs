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
    public class TamanhosRefeicaoController : ControllerBase
    {
        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/TamanhosRefeicao
        [Authorize(Roles = Constantes.GrupoGarcons)]
        public IQueryable<TamanhoRefeicao> GetTamanhosRefeicao(int codRefeicao, int codTipoRefeicao)
        {
            try
            {
                return db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == codRefeicao && r.Refeicao.CodTipo == codTipoRefeicao && r.Ativo == true).Select(r => r.TamanhoRefeicao).Distinct().OrderBy(t => t.Nome);
            }
            catch
            {
                return null;
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
