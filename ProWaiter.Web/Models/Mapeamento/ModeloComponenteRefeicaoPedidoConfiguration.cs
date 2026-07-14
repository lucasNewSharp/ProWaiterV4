using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloComponenteRefeicaoPedidoConfiguration : EntityTypeConfiguration<ModeloComponenteRefeicaoPedido>
    {
        public ModeloComponenteRefeicaoPedidoConfiguration()
        {
            ToTable("TBModeloAtribComponentesRefeicaoPedido")
                .HasKey(m => new { m.CodModeloRefeicaoPedido, m.CodComponente });

            HasRequired(c => c.ModeloRefeicaoDoPedido)
                .WithMany()
                .HasForeignKey(c => c.CodModeloRefeicaoPedido);

            HasRequired(c => c.ComponenteRefeicao)
                .WithMany()
                .HasForeignKey(c => c.CodComponente);
        }
    }
}