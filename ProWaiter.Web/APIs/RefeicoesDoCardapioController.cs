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
    public class RefeicoesDoCardapioController : ControllerBase
    {
        private readonly ProWaiterContext db = new ProWaiterContext();

        // GET: api/RefeicoesDoCardapio
        public IEnumerable<RefeicaoDoCardapio> GetRefeicoesDoCardapio()
        {
            try
            {
                return db.RefeicoesDoCardapio.OrderBy(r => r.Refeicao.Nome).ToList();
            }
            catch
            {
                return null;
            }
        }

        // GET: api/RefeicoesDoCardapio/5
        [ProducesResponseType(typeof(RefeicaoDoCardapio), 200)]
        public RefeicaoDoCardapio GetRefeicaoDoCardapio(short codRefeicao, string codTamanho)
        {
            try
            {
                RefeicaoDoCardapio refeicaoDoCardapio = db.RefeicoesDoCardapio.Where(r => r.CodRefeicao == codRefeicao && r.CodTamanho == codTamanho).SingleOrDefault();
                if (refeicaoDoCardapio == null)
                {
                    throw new Exception("400 Bad Request");
                }

                return refeicaoDoCardapio;
            }
            catch
            {
                return null;
            }
        }

        // GET: api/RefeicoesDoCardapio/5
        public IEnumerable<RefeicaoDoCardapio> GetRefeicoesDoCardapio(short codTipoRefeicao)
        {
            try
            {
                IEnumerable<RefeicaoDoCardapio> refeicaoDoCardapio = db.RefeicoesDoCardapio
                    .Where(r => r.Refeicao.CodTipo == codTipoRefeicao && r.Ativo == true)
                    .OrderBy(r => r.Refeicao.Nome)
                    .ToList();
                if (refeicaoDoCardapio == null)
                    return new RefeicaoDoCardapio[] { };

                return refeicaoDoCardapio;
            }
            catch
            {
                return null;
            }
        }
        
//         // // protected void Dispose(bool disposing)
//         {
//             if (disposing)
//                 // // db.Dispose();
//             // base.Dispose(disposing);
//         }
    }
}
