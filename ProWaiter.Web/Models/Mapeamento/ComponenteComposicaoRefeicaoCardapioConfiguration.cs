using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ComponenteComposicaoRefeicaoCardapioConfiguration : IEntityTypeConfiguration<ComponenteComposicaoRefeicaoCardapio>
    {
        public void Configure(EntityTypeBuilder<ComponenteComposicaoRefeicaoCardapio> builder)
        {
            builder.ToTable("TBAtribComponentesComposicaoRefeicaoCardapio")
                .HasKey(c => new { c.CodRefeicao, c.CodTamanho, c.CodComponente });

            builder.HasOne(c => c.Refeicao).WithMany().IsRequired()
                .HasForeignKey(c => c.CodRefeicao);

            builder.HasOne(c => c.Tamanho).WithMany().IsRequired()
                .HasForeignKey(c => c.CodTamanho);

            builder.HasOne(c => c.ComponenteRefeicao).WithMany().IsRequired()
                .HasForeignKey(c => c.CodComponente);

            builder.Property(c => c.Valor)
                .HasPrecision(6, 2);

            builder.HasOne(c => c.Unidade)
                .WithMany()
                .HasForeignKey(c => c.CodUnidade);
        }
    }
}