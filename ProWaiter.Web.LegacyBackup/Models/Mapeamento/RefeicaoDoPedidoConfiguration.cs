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
    public class RefeicaoDoPedidoConfiguration : EntityTypeConfiguration<RefeicaoDoPedido>
    {
        public RefeicaoDoPedidoConfiguration()
        {
            ToTable("TBAtribRefeicoesPedido")
                .HasKey(r => r.Codigo);

            HasRequired(r => r.RefeicaoDoCardapio)
               .WithMany()
               .HasForeignKey(r => new { r.CodRefeicao, r.CodTamanho });

            HasMany(r => r.ComponentesRefeicaoPedido)
            .WithOptional()
            .HasForeignKey(c => c.CodRefeicaoPedido);
            
            HasRequired(r => r.Tamanho)
                .WithMany()
                .HasForeignKey(r => r.CodTamanho);

            Property(r => r.PercDesconto)
                .HasPrecision(5, 2);
        }
    }
}
