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
    internal class PedidoInternoConfiguration : EntityTypeConfiguration<PedidoInterno>
    {
        public PedidoInternoConfiguration()
        {
            ToTable("TBPedidosInternos").
                HasKey(p => p.Codigo);

            HasOptional(p => p.LocalInterno)
                .WithMany()
                .HasForeignKey(p => p.CodLocalInterno);

            HasOptional(p => p.Mesa)
                .WithMany()
                .HasForeignKey(p => p.CodMesa);
        }
    }
}
