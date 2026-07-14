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
    public class RefeicaoCardapioConfiguration : IEntityTypeConfiguration<RefeicaoDoCardapio>
    {
        public void Configure(EntityTypeBuilder<RefeicaoDoCardapio> builder)
        {
            builder.ToTable("TBRefeicoesCardapio")
                .HasKey(r => new { r.CodRefeicao, r.CodTamanho });
            
            builder.HasOne(r => r.Refeicao).WithMany().IsRequired()
                .HasForeignKey(r => r.CodRefeicao);

            builder.HasOne(r => r.TamanhoRefeicao).WithMany().IsRequired()
                .HasForeignKey(r => r.CodTamanho);

            builder.HasOne(r => r.Impressora).WithMany().IsRequired()
                .HasForeignKey(r => r.CodImpressora);

            builder.HasMany(r => r.ComponentesComposicaoRefeicao)
                .WithOne().IsRequired()
                .HasForeignKey(c => new { c.CodRefeicao, c.CodTamanho });

            builder.Property(r => r.PercDesconto)
               .HasPrecision(5, 2);

            builder.Property(r => r.CodBarras)
                .IsRequired(false)
                .HasMaxLength(RefeicaoDoCardapio.TamMaxCodBarras);
        }
    }
}
