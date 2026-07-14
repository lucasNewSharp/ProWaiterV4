using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class ModeloRefeicaoPedidoConfiguration : EntityTypeConfiguration<ModeloRefeicaoPedido>
    {
        public ModeloRefeicaoPedidoConfiguration()
        {
            ToTable("TBModeloAtribRefeicoesPedido")
                .HasKey(m => m.Codigo);

            HasRequired(r => r.RefeicaoDoCardapio)
               .WithMany()
               .HasForeignKey(r => new { r.CodRefeicao, r.CodTamanho });

            HasMany(r => r.ModeloComponentesRefeicaoPedido)
            .WithOptional()
            .HasForeignKey(c => c.CodModeloRefeicaoPedido);

            HasRequired(r => r.Tamanho)
                .WithMany()
                .HasForeignKey(r => r.CodTamanho);
        }
    }
}