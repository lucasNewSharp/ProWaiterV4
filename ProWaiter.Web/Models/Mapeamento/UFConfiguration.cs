using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;

namespace ProWaiter.Web.Models.Mapeamento
{
    //http://blogs.msdn.com/b/adonet/archive/2010/12/06/ef-feature-ctp5-fluent-api-samples.aspx

    internal class UFConfiguration : IEntityTypeConfiguration<UF>
    {
        public void Configure(EntityTypeBuilder<UF> builder)
        {
            builder.HasKey(u => u.Codigo);
            builder.ToTable("TBUFs");

            builder.HasMany(u => u.Cidades)
                .WithOne(c => c.UF).IsRequired()
                .HasForeignKey(u => u.CodUF);

            builder.Property(u => u.Codigo)                
                .HasMaxLength(UF.TamCodigo)
                .IsFixedLength();

            builder.Property(u => u.Nome)
                .HasMaxLength(UF.TamMaxNome)
                .IsRequired();

        }
    }
}
