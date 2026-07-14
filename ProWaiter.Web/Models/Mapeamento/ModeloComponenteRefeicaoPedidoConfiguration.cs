using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloComponenteRefeicaoPedidoConfiguration : IEntityTypeConfiguration<ModeloComponenteRefeicaoPedido>
    {
        public void Configure(EntityTypeBuilder<ModeloComponenteRefeicaoPedido> builder)
        {
            builder.ToTable("TBModeloAtribComponentesRefeicaoPedido")
                .HasKey(m => new { m.CodModeloRefeicaoPedido, m.CodComponente });

            builder.HasOne(c => c.ModeloRefeicaoDoPedido).WithMany().IsRequired()
                .HasForeignKey(c => c.CodModeloRefeicaoPedido);

            builder.HasOne(c => c.ComponenteRefeicao).WithMany().IsRequired()
                .HasForeignKey(c => c.CodComponente);
        }
    }
}