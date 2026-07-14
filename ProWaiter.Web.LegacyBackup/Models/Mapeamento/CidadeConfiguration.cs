using ProWaiter.Web.Models;
using ProWaiter.Web.Models.Entidades;
using System.Data.Entity.ModelConfiguration;


namespace ProWaiter.Web.Models.Mapeamento
{
    internal class CidadeConfiguration : EntityTypeConfiguration<Cidade>
    {
        public CidadeConfiguration()
        {
            ToTable("TBCidades");
            HasKey(c => c.Codigo);
            
            HasRequired(c => c.UF)
                .WithMany()
                .HasForeignKey(c => c.CodUF);

            Property(c => c.Nome)
                .HasMaxLength(Cidade.TamMaxNome)
                .IsRequired();
        }
    }
}
