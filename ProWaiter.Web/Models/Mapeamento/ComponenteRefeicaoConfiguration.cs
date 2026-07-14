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
    internal class ComponenteRefeicaoConfiguration : IEntityTypeConfiguration<ComponenteRefeicao>
    {
        public void Configure(EntityTypeBuilder<ComponenteRefeicao> builder)
        {
            builder.ToTable("TBComponentesRefeicao")
                .HasKey(c => c.Codigo);
        }
    }
}
