using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloPedidoConfiguration : IEntityTypeConfiguration<ModeloPedido>
    {
        public void Configure(EntityTypeBuilder<ModeloPedido> builder)
        {
            builder.ToTable("TBModelosPedido")
                .HasKey(m => m.Codigo);

            builder.HasMany(m => m.ModelosRefeicaoPedidos)
                .WithOne().IsRequired()
                .HasForeignKey(m => m.CodModeloPedido);

            builder.HasMany(m => m.ModelosBebidaPedido)
                .WithOne().IsRequired()
                .HasForeignKey(m => m.CodModeloPedido);

            builder.Property(c => c.Acrescimo)
                .HasPrecision(6, 2);

            builder.Property(c => c.Desconto)
                .HasPrecision(6, 2);
        }
    }
}