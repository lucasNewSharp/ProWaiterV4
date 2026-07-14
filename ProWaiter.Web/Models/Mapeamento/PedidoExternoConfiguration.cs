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
    internal class PedidoExternoConfiguration : IEntityTypeConfiguration<PedidoExterno>
    {
        public void Configure(EntityTypeBuilder<PedidoExterno> builder)
        {
            builder.ToTable("TBPedidosExternos").HasKey(p => p.Codigo);

            builder.HasOne(p => p.Cliente).WithMany().IsRequired()
                .HasForeignKey(p => p.CodCliente);

            builder.Property(p => p.ValorEntrega)
                .HasPrecision(6, 2);

            builder.HasOne(p => p.EnderecoCliente)
                .WithMany()
                .HasForeignKey(p => p.CodEnderecoEntrega);
        }
    }
}
