using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Gestores
{
    public static class GestorItemCodBarras
    {
        public static IItemCodigoBarras ObterItemCodBarras(string codBarras, bool track = true)
        {
            ProWaiterContext db = new ProWaiterContext();

            if (string.IsNullOrWhiteSpace(codBarras))
                return null;

            IQueryable<ItemBalcao> queryItensBalcao = db.ItensBacao.Where(i => i.CodBarras == codBarras);

            if (!track)
                queryItensBalcao = queryItensBalcao.AsNoTracking();

            ItemBalcao itemBalcao = queryItensBalcao.SingleOrDefault();
            if (itemBalcao != null)
                return itemBalcao;

            IQueryable<Bebida> queryBebidas = db.Bebidas.Where(b => b.CodBarras == codBarras);

            if (!track)
                queryBebidas = queryBebidas.AsNoTracking();

            Bebida bebida = db.Bebidas.Where(b => b.CodBarras == codBarras).SingleOrDefault();

            if (bebida != null)
                return bebida;

            IQueryable<RefeicaoDoCardapio> queryRefeicao = db.RefeicoesDoCardapio.Where(c => c.CodBarras == codBarras);

            if (!track)
                queryRefeicao = queryRefeicao.AsNoTracking();

            RefeicaoDoCardapio refeicao = queryRefeicao.SingleOrDefault();

            if (refeicao != null)
                return refeicao;

            return null;
        }
    }
}