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
    internal class PedidoInternoConfiguration : IEntityTypeConfiguration<PedidoInterno>
    {
        public void Configure(EntityTypeBuilder<PedidoInterno> builder)
        {
            builder.ToTable("TBPedidosInternos");

            builder.HasOne(p => p.LocalInterno)
                .WithMany()
                .HasForeignKey(p => p.CodLocalInterno);

            builder.HasOne(p => p.Mesa)
                .WithMany()
                .HasForeignKey(p => p.CodMesa);
        }
    }
}
