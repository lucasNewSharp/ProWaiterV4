using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class ImpressoraConfiguration : EntityTypeConfiguration<Impressora>
    {
        public ImpressoraConfiguration()
        {
            ToTable("TBImpressoras")
                .HasKey(i => i.Codigo)
                .Ignore(i => i.NomeExibicao)
                .Ignore(i => i.TipoImpressao)
                .Ignore(i => i.NomeExibicaoTipoImpressao);

            Property(c => c.Codigo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}