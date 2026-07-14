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
    internal class BebidaDoPedidoConfiguration : EntityTypeConfiguration<BebidaDoPedido>
    {
        public BebidaDoPedidoConfiguration()
        {
            ToTable("TBAtribBebidasPedido")
                .HasKey(b => b.Codigo);

            HasRequired(b => b.Bebida)
                .WithMany()
                .HasForeignKey(b => b.CodBebida);

            Property(b => b.Valor)
                .HasPrecision(5, 2);

            Property(b => b.PercDesconto)
                .HasPrecision(5, 2);
        }
    }
}
