using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloPedidoConfiguration : EntityTypeConfiguration<ModeloPedido>
    {
        public ModeloPedidoConfiguration()
        {
            ToTable("TBModelosPedido")
                .HasKey(m => m.Codigo);

            HasMany(m => m.ModelosRefeicaoPedidos)
                .WithRequired()
                .HasForeignKey(m => m.CodModeloPedido);

            HasMany(m => m.ModelosBebidaPedido)
                .WithRequired()
                .HasForeignKey(m => m.CodModeloPedido);

            Property(c => c.Acrescimo)
                .HasPrecision(6, 2);

            Property(c => c.Desconto)
                .HasPrecision(6, 2);
        }
    }
}