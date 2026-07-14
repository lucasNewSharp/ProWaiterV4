using System.Data.Entity.ModelConfiguration;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;

namespace ProWaiter.Web.Models.Mapeamento
{
    //http://blogs.msdn.com/b/adonet/archive/2010/12/06/ef-feature-ctp5-fluent-api-samples.aspx

    internal class UFConfiguration : EntityTypeConfiguration<UF>
    {
        public UFConfiguration()
        {
            HasKey(u => u.Codigo);
            ToTable("TBUFs");

            HasMany(u => u.Cidades)
                .WithRequired(c => c.UF)
                .HasForeignKey(u => u.CodUF);

            Property(u => u.Codigo)                
                .HasMaxLength(UF.TamCodigo)
                .IsFixedLength();

            Property(u => u.Nome)
                .HasMaxLength(UF.TamMaxNome)
                .IsRequired();

        }
    }
}
