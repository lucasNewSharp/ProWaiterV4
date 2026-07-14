using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class BebidaDoPedidoConfiguration : IEntityTypeConfiguration<BebidaDoPedido>
    {
        public void Configure(EntityTypeBuilder<BebidaDoPedido> builder)
        {
            builder.ToTable("TBAtribBebidasPedido")
                .HasKey(b => b.Codigo);

            builder.HasOne(b => b.Bebida).WithMany().IsRequired()
                .HasForeignKey(b => b.CodBebida);

            builder.Property(b => b.Valor)
                .HasPrecision(5, 2);

            builder.Property(b => b.PercDesconto)
                .HasPrecision(5, 2);
        }
    }
}
