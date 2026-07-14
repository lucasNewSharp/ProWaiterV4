using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class UnidadeComponenteComposicaoConfiguration : EntityTypeConfiguration<UnidadeComponenteComposicao>
    {
        public UnidadeComponenteComposicaoConfiguration()
        {
            ToTable("TBUnidadesComponenteComposicao")
                .HasKey(u => u.Codigo);

            Property(c => c.Codigo)
                .HasMaxLength(UnidadeComponenteComposicao.TamMaxCodigo)
                .IsFixedLength();

            Property(c => c.Descricao)
                .HasMaxLength(UnidadeComponenteComposicao.TamMaxDescricao);
        }
    }
}