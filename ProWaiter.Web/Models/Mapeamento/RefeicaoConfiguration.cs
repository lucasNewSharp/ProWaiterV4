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
    internal class RefeicaoConfiguration : IEntityTypeConfiguration<Refeicao>
    {
        public void Configure(EntityTypeBuilder<Refeicao> builder)
        {
            builder.ToTable("TBRefeicoes")
                .HasKey(r => r.Codigo);

            builder.HasOne(r => r.Tipo).WithMany().IsRequired()
                .HasForeignKey(r => r.CodTipo);

            builder.HasMany(r => r.ComponentesRefeicao)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>("TBAtribComponentesRefeicao", j => j.HasOne<ComponenteRefeicao>().WithMany().HasForeignKey("CodComponente"), j => j.HasOne<Refeicao>().WithMany().HasForeignKey("CodRefeicao"));
        }
    }
}
