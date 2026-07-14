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
    internal class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("TBPedidos");
            builder.HasKey(p => p.Codigo);
            builder.Ignore(p => p.ValorTotal);
            builder.Ignore(p => p.ValorBebidas);
            builder.Ignore(p => p.ValorItensDeBalcao);
            builder.Ignore(p => p.ValorRefeicoes);
            builder.Ignore(p => p.TodosItensEnviados);

            builder.HasMany(p => p.BebidasDoPedido)
                .WithOne()
                .HasForeignKey(p => p.CodPedido)
                .IsRequired();

            builder.HasMany(p => p.RefeicoesDoPedido)
                .WithOne()
                .HasForeignKey(p => p.CodPedido)
                .IsRequired();

            builder.HasMany(p => p.ItensBalcaoDoPedido)
                .WithOne()
                .HasForeignKey(i => i.CodPedido)
                .IsRequired();
        }
    }
}
