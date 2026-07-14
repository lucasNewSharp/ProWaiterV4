using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using System.Data.Entity.ModelConfiguration;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class MesaConfiguration : EntityTypeConfiguration<Mesa>
    {
        public MesaConfiguration()
        {
            ToTable("TBMesas").
                HasKey(m => m.Codigo);

            Property(m => m.Descricao)
                .HasMaxLength(Mesa.TamMaxDescricao)
                .IsRequired();

            HasOptional(m => m.UltimoPedido)
                .WithMany()
                .HasForeignKey(m => m.CodUltimoPedido);
        }
    }
}
