using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloRefeicaoPedidoConfiguration : IEntityTypeConfiguration<ModeloRefeicaoPedido>
    {
        public void Configure(EntityTypeBuilder<ModeloRefeicaoPedido> builder)
        {
            builder.ToTable("TBModeloAtribRefeicoesPedido")
                .HasKey(m => m.Codigo);

            builder.HasOne(r => r.RefeicaoDoCardapio).WithMany().IsRequired()
               .HasForeignKey(r => new { r.CodRefeicao, r.CodTamanho });

            builder.HasMany(r => r.ModeloComponentesRefeicaoPedido)
            .WithOne()
            .HasForeignKey(c => c.CodModeloRefeicaoPedido);

            builder.HasOne(r => r.Tamanho).WithMany().IsRequired()
                .HasForeignKey(r => r.CodTamanho);
        }
    }
}