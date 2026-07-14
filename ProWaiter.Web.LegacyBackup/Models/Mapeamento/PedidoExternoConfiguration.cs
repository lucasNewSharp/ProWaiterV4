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
    internal class PedidoExternoConfiguration : EntityTypeConfiguration<PedidoExterno>
    {
        public PedidoExternoConfiguration()
        {
            ToTable("TBPedidosExternos").
                HasKey(p => p.Codigo);

            HasRequired(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.CodCliente);

            Property(p => p.ValorEntrega)
                .HasPrecision(6, 2);

            HasOptional(p => p.EnderecoCliente)
                .WithMany()
                .HasForeignKey(p => p.CodEnderecoEntrega);
        }
    }
}
