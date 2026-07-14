using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloBebidaPedidoConfiguration : IEntityTypeConfiguration<ModeloBebidaPedido>
    {
        public void Configure(EntityTypeBuilder<ModeloBebidaPedido> builder)
        {
            builder.ToTable("TBModeloAtribBebidasPedido")
                .HasKey(m => m.Codigo);

            builder.HasOne(m => m.Bebida).WithMany().IsRequired()
                .HasForeignKey(m => m.CodBebida);
        }
    }
}