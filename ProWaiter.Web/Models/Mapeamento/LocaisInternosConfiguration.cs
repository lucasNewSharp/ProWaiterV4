using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class LocaisInternosConfiguration : EntityTypeConfiguration<LocalInterno>
    {
        public LocaisInternosConfiguration()
        {
            ToTable("TBLocaisInternos")
                .HasKey(l => l.Codigo);

            Property(l => l.Nome)
                .HasMaxLength(LocalInterno.TamMaxNome)
                .IsRequired();
        }
    }
}