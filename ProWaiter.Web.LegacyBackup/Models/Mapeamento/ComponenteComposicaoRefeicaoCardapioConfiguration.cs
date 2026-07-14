using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ComponenteComposicaoRefeicaoCardapioConfiguration : EntityTypeConfiguration<ComponenteComposicaoRefeicaoCardapio>
    {
        public ComponenteComposicaoRefeicaoCardapioConfiguration()
        {
            ToTable("TBAtribComponentesComposicaoRefeicaoCardapio")
                .HasKey(c => new { c.CodRefeicao, c.CodTamanho, c.CodComponente });

            HasRequired(c => c.Refeicao)
                .WithMany()
                .HasForeignKey(c => c.CodRefeicao);

            HasRequired(c => c.Tamanho)
                .WithMany()
                .HasForeignKey(c => c.CodTamanho);

            HasRequired(c => c.ComponenteRefeicao)
                .WithMany()
                .HasForeignKey(c => c.CodComponente);

            Property(c => c.Valor)
                .HasPrecision(6, 2);

            HasOptional(c => c.Unidade)
                .WithMany()
                .HasForeignKey(c => c.CodUnidade);
        }
    }
}