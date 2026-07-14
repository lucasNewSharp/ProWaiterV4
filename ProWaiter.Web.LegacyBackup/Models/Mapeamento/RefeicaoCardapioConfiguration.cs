using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class RefeicaoCardapioConfiguration : EntityTypeConfiguration<RefeicaoDoCardapio>
    {
        public RefeicaoCardapioConfiguration()
        {
            ToTable("TBRefeicoesCardapio")
                .HasKey(r => new { r.CodRefeicao, r.CodTamanho });
            
            HasRequired(r => r.Refeicao)
                .WithMany()
                .HasForeignKey(r => r.CodRefeicao);

            HasRequired(r => r.TamanhoRefeicao)
                .WithMany()
                .HasForeignKey(r => r.CodTamanho);

            HasRequired(r => r.Impressora)
                .WithMany()
                .HasForeignKey(r => r.CodImpressora);

            HasMany(r => r.ComponentesComposicaoRefeicao)
                .WithRequired()
                .HasForeignKey(c => new { c.CodRefeicao, c.CodTamanho });

            Property(r => r.PercDesconto)
               .HasPrecision(5, 2);

            Property(r => r.CodBarras)
                .IsOptional()
                .HasMaxLength(RefeicaoDoCardapio.TamMaxCodBarras);
        }
    }
}
