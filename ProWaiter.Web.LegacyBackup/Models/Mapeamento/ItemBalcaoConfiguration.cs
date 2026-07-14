using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ItemBalcaoConfiguration : EntityTypeConfiguration<ItemBalcao>
    {
        public ItemBalcaoConfiguration()
        {
            ToTable("TBItensBalcao")
                .HasKey(i => i.Codigo);

            Property(i => i.Codigo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(i => i.Nome)
                .IsRequired()
                .HasMaxLength(ItemBalcao.TamMaxNome);

            Property(i => i.CodBarras)
                .IsOptional()
                .HasMaxLength(ItemBalcao.TamMaxCodBarras);

            Property(i => i.Valor)
                .IsRequired()
                .HasPrecision(10, 2);

            Property(i => i.PercDesconto)
                .IsRequired()
                .HasPrecision(5, 2);
        }
    }
}