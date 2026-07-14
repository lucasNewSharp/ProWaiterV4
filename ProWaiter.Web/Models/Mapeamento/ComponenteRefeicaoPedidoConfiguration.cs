using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ComponenteRefeicaoPedidoConfiguration : IEntityTypeConfiguration<ComponenteRefeicaoPedido>
    {
        public void Configure(EntityTypeBuilder<ComponenteRefeicaoPedido> builder)
        {
            builder.ToTable("TBAtribComponentesRefeicaoPedido")
                .HasKey(c => new { c.CodRefeicaoPedido, c.CodComponente });

            builder.HasOne(c => c.RefeicaoDoPedido).WithMany().IsRequired()
                .HasForeignKey(c => c.CodRefeicaoPedido);

            builder.HasOne(c => c.ComponenteRefeicao).WithMany().IsRequired()                
                .HasForeignKey(c => c.CodComponente);

        }
    }
}