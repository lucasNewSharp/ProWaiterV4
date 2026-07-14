using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ProWaiter.Web.Models.Mapeamento
{
    internal class CidadeConfiguration : IEntityTypeConfiguration<Cidade>
    {
        public void Configure(EntityTypeBuilder<Cidade> builder)
        {
            builder.ToTable("TBCidades");
            builder.HasKey(c => c.Codigo);
            
            builder.HasOne(c => c.UF).WithMany().IsRequired()
                .HasForeignKey(c => c.CodUF);

            builder.Property(c => c.Nome)
                .HasMaxLength(Cidade.TamMaxNome)
                .IsRequired();
        }
    }
}
