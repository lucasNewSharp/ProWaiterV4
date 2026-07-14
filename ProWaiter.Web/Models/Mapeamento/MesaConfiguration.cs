using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class MesaConfiguration : IEntityTypeConfiguration<Mesa>
    {
        public void Configure(EntityTypeBuilder<Mesa> builder)
        {
            builder.ToTable("TBMesas").HasKey(m => m.Codigo);

            builder.Property(m => m.Descricao)
                .HasMaxLength(Mesa.TamMaxDescricao)
                .IsRequired();

            builder.HasOne(m => m.UltimoPedido)
                .WithMany()
                .HasForeignKey(m => m.CodUltimoPedido);
        }
    }
}
