using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ComponenteRefeicaoPedidoConfiguration : EntityTypeConfiguration<ComponenteRefeicaoPedido>
    {
        public ComponenteRefeicaoPedidoConfiguration()
        {
            ToTable("TBAtribComponentesRefeicaoPedido")
                .HasKey(c => new { c.CodRefeicaoPedido, c.CodComponente });

            HasRequired(c => c.RefeicaoDoPedido)
                .WithMany()
                .HasForeignKey(c => c.CodRefeicaoPedido);

            HasRequired(c => c.ComponenteRefeicao)
                .WithMany()                
                .HasForeignKey(c => c.CodComponente);

        }
    }
}