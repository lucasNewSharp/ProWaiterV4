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
    internal class ComponenteRefeicaoConfiguration : EntityTypeConfiguration<ComponenteRefeicao>
    {
        public ComponenteRefeicaoConfiguration()
        {
            ToTable("TBComponentesRefeicao")
                .HasKey(c => c.Codigo);
        }
    }
}
