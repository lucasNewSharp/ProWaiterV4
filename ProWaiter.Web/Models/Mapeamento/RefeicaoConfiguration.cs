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
    internal class RefeicaoConfiguration : EntityTypeConfiguration<Refeicao>
    {
        public RefeicaoConfiguration()
        {
            ToTable("TBRefeicoes")
                .HasKey(r => r.Codigo);

            HasRequired(r => r.Tipo)
                .WithMany()
                .HasForeignKey(r => r.CodTipo);

            HasMany(r => r.ComponentesRefeicao)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("CodRefeicao");
                    m.MapRightKey("CodComponente");
                    m.ToTable("TBAtribComponentesRefeicao");
                });
        }
    }
}
