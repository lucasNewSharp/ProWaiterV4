using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class TipoBebidaConfiguration : IEntityTypeConfiguration<TipoBebida>
    {
        public void Configure(EntityTypeBuilder<TipoBebida> builder)
        {
            builder.ToTable("TBTiposBebida").HasKey(r => r.Codigo);
        }
    }
}