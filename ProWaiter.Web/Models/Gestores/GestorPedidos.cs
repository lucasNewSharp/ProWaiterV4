using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.GestoresBD;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Gestores
{
    public class GestorPedidos
    {
        ProWaiterContext db = null;

        public GestorPedidos(ProWaiterContext db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            this.db = db;
        }

        public Pedido CriarPedido(System.Security.Principal.IIdentity usuario, Mesa mesa, string observacoes)
        {
            return CriarPedido(usuario, mesa, null, observacoes);
        }

        public Pedido CriarPedido(System.Security.Principal.IIdentity usuario, Mesa mesa, LocalInterno localInterno, string observacoes)
        {
            PedidoInterno pedido = new PedidoInterno(DateTime.Now, usuario.Name, 0, 0, observacoes);
            pedido.LocalInterno = localInterno;
            pedido.Mesa = mesa;

            try
            {
                db.Database.BeginTransaction();
                db.PedidosInternos.Add(pedido);
                db.SaveChanges();
                db.Mesas.Attach(mesa);  //Atachamos pois essa mesa pode vir de um HttpRequest e pode estar em outro contexto, então o nosso ProWaiterContext precisa saber que esta instância pertence ao seu dbset              
                mesa.UltimoPedido = pedido;
                db.Entry(mesa).State = EntityState.Modified;
                db.SaveChanges();
            }
            finally
            {
                if (db.Database.CurrentTransaction != null) db.Database.CurrentTransaction.Commit();
            }

            return pedido;
        }

        public Pedido CriarPedido(System.Security.Principal.IIdentity usuario, string observacoes)
        {
            PedidoParaLevar pedido = new PedidoParaLevar(DateTime.Now, usuario.Name, 0, 0, observacoes);

            try
            {
                db.Database.BeginTransaction();
                db.PedidosParaLevar.Add(pedido);
                db.SaveChanges();
            }
            finally
            {
                if (db.Database.CurrentTransaction != null) db.Database.CurrentTransaction.Commit();
            }

            return pedido;
        }
    }
}