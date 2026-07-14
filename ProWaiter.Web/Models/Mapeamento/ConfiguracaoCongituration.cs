using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ConfiguracaoCongituration : IEntityTypeConfiguration<Configuracao>
    {
        public void Configure(EntityTypeBuilder<Configuracao> builder)
        {
            builder.ToTable("TBConfiguracoes")
                .HasKey(c => c.Codigo);

            builder.Property(c => c.Codigo)
                .HasMaxLength(Configuracao.TamMaxCodigo);                
        }
    }
}