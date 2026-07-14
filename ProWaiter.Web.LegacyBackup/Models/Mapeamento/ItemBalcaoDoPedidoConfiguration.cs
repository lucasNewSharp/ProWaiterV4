using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ItemBalcaoDoPedidoConfiguration : EntityTypeConfiguration<ItemBalcaoDoPedido>
    {
        public ItemBalcaoDoPedidoConfiguration()
        {
            ToTable("TBAtribItensBalcaoPedido")
                .HasKey(i => i.Codigo);

            Property(i => i.Codigo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(i => i.ItemBalcao)
                .WithMany()
                .HasForeignKey(i => i.CodItemBalcao);

            Property(i => i.Valor)
                .HasPrecision(10, 2);

            Property(i => i.PercDesconto)
                .HasPrecision(10, 2);

        }
    }
}