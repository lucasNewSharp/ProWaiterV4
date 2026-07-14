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
    internal class BebidaConfiguration : IEntityTypeConfiguration<Bebida>
    {
        public void Configure(EntityTypeBuilder<Bebida> builder)
        {
            builder.ToTable("TBBebidas")
                .HasKey(b => b.Codigo);

            builder.HasOne(b => b.Tipo).WithMany().IsRequired()
                .HasForeignKey(b => b.CodTipo);

            builder.HasOne(b => b.Impressora).WithMany().IsRequired()
                .HasForeignKey(b => b.CodImpressora);

            builder.Property(b => b.PercDesconto)
                .HasPrecision(5, 2);

            builder.Property(b => b.CodBarras)
                .IsRequired(false)
                .HasMaxLength(Bebida.TamMaxCodBarras);
        }
    }
}
