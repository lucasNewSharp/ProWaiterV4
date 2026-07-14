using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class UnidadeComponenteComposicaoConfiguration : IEntityTypeConfiguration<UnidadeComponenteComposicao>
    {
        public void Configure(EntityTypeBuilder<UnidadeComponenteComposicao> builder)
        {
            builder.ToTable("TBUnidadesComponenteComposicao")
                .HasKey(u => u.Codigo);

            builder.Property(c => c.Codigo)
                .HasMaxLength(UnidadeComponenteComposicao.TamMaxCodigo)
                .IsFixedLength();

            builder.Property(c => c.Descricao)
                .HasMaxLength(UnidadeComponenteComposicao.TamMaxDescricao);
        }
    }
}