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
    public class ItemBalcaoDoPedidoConfiguration : IEntityTypeConfiguration<ItemBalcaoDoPedido>
    {
        public void Configure(EntityTypeBuilder<ItemBalcaoDoPedido> builder)
        {
            builder.ToTable("TBAtribItensBalcaoPedido")
                .HasKey(i => i.Codigo);

            builder.Property(i => i.Codigo)
                .ValueGeneratedOnAdd();

            builder.HasOne(i => i.ItemBalcao).WithMany().IsRequired()
                .HasForeignKey(i => i.CodItemBalcao);

            builder.Property(i => i.Valor)
                .HasPrecision(10, 2);

            builder.Property(i => i.PercDesconto)
                .HasPrecision(10, 2);

        }
    }
}