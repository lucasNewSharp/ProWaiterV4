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
    internal class TamanhoRefeicaoConfiguration : IEntityTypeConfiguration<TamanhoRefeicao>
    {
        public void Configure(EntityTypeBuilder<TamanhoRefeicao> builder)
        {
            builder.ToTable("TBTamanhosRefeicao")
                .HasKey(t => t.Codigo);                
        }
    }
}
