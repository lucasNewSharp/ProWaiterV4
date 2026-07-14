using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ConfiguracaoCongituration : EntityTypeConfiguration<Configuracao>
    {
        public ConfiguracaoCongituration()
        {
            ToTable("TBConfiguracoes")
                .HasKey(c => c.Codigo);

            Property(c => c.Codigo)
                .HasMaxLength(Configuracao.TamMaxCodigo);                
        }
    }
}