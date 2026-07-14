using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloBebidaPedidoConfiguration : EntityTypeConfiguration<ModeloBebidaPedido>
    {
        public ModeloBebidaPedidoConfiguration()
        {
            ToTable("TBModeloAtribBebidasPedido")
                .HasKey(m => m.Codigo);

            HasRequired(m => m.Bebida)
                .WithMany()
                .HasForeignKey(m => m.CodBebida);
        }
    }
}