using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class ImpressoraConfiguration : IEntityTypeConfiguration<Impressora>
    {
        public void Configure(EntityTypeBuilder<Impressora> builder)
        {
            builder.ToTable("TBImpressoras");
            builder.HasKey(i => i.Codigo);
            builder.Ignore(i => i.NomeExibicao);
            builder.Ignore(i => i.TipoImpressao);
            builder.Ignore(i => i.NomeExibicaoTipoImpressao);

            builder.Property(c => c.Codigo).ValueGeneratedOnAdd();
        }
    }
}