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
    internal class BebidaConfiguration : EntityTypeConfiguration<Bebida>
    {
        public BebidaConfiguration()
        {
            ToTable("TBBebidas")
                .HasKey(b => b.Codigo);

            HasRequired(b => b.Tipo)
                .WithMany()
                .HasForeignKey(b => b.CodTipo);

            HasRequired(b => b.Impressora)
                .WithMany()
                .HasForeignKey(b => b.CodImpressora);

            Property(b => b.PercDesconto)
                .HasPrecision(5, 2);

            Property(b => b.CodBarras)
                .IsOptional()
                .HasMaxLength(Bebida.TamMaxCodBarras);
        }
    }
}
