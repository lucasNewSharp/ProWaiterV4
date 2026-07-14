using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ItemBalcaoConfiguration : IEntityTypeConfiguration<ItemBalcao>
    {
        public void Configure(EntityTypeBuilder<ItemBalcao> builder)
        {
            builder.ToTable("TBItensBalcao")
                .HasKey(i => i.Codigo);

            builder.Property(i => i.Codigo)
                .ValueGeneratedOnAdd();

            builder.Property(i => i.Nome)
                .IsRequired()
                .HasMaxLength(ItemBalcao.TamMaxNome);

            builder.Property(i => i.CodBarras)
                .IsRequired(false)
                .HasMaxLength(ItemBalcao.TamMaxCodBarras);

            builder.Property(i => i.Valor)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(i => i.PercDesconto)
                .IsRequired()
                .HasPrecision(5, 2);
        }
    }
}