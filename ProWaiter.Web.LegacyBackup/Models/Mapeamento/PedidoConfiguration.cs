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
    internal class PedidoConfiguration : EntityTypeConfiguration<Pedido>
    {
        public PedidoConfiguration()
        {
            ToTable("TBPedidos")
                .HasKey(p => p.Codigo)
                .Ignore(p => p.ValorTotal)
                .Ignore(p => p.ValorBebidas)
                .Ignore(p => p.ValorItensDeBalcao)
                .Ignore(p => p.ValorRefeicoes)
                .Ignore(p => p.TodosItensEnviados);

            HasMany(p => p.BebidasDoPedido)
                .WithRequired()
                .HasForeignKey(p => p.CodPedido);

            HasMany(p => p.RefeicoesDoPedido)
                .WithRequired()
                .HasForeignKey(p => p.CodPedido);

            HasMany(p => p.ItensBalcaoDoPedido)
                .WithRequired()
                .HasForeignKey(i => i.CodPedido);
        }
    }
}
