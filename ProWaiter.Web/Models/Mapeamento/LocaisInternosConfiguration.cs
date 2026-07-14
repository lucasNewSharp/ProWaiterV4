using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class LocaisInternosConfiguration : IEntityTypeConfiguration<LocalInterno>
    {
        public void Configure(EntityTypeBuilder<LocalInterno> builder)
        {
            builder.ToTable("TBLocaisInternos")
                .HasKey(l => l.Codigo);

            builder.Property(l => l.Nome)
                .HasMaxLength(LocalInterno.TamMaxNome)
                .IsRequired();
        }
    }
}