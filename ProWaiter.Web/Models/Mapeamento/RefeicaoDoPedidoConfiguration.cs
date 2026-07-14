using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class RefeicaoDoPedidoConfiguration : IEntityTypeConfiguration<RefeicaoDoPedido>
    {
        public void Configure(EntityTypeBuilder<RefeicaoDoPedido> builder)
        {
            builder.ToTable("TBAtribRefeicoesPedido")
                .HasKey(r => r.Codigo);

            builder.HasOne(r => r.RefeicaoDoCardapio).WithMany().IsRequired()
               .HasForeignKey(r => new { r.CodRefeicao, r.CodTamanho });

            builder.HasMany(r => r.ComponentesRefeicaoPedido)
            .WithOne()
            .HasForeignKey(c => c.CodRefeicaoPedido);
            
            builder.HasOne(r => r.Tamanho).WithMany().IsRequired()
                .HasForeignKey(r => r.CodTamanho);

            builder.Property(r => r.PercDesconto)
                .HasPrecision(5, 2);
        }
    }
}
