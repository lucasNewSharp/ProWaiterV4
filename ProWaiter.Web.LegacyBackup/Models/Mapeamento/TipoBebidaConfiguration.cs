using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class TipoBebidaConfiguration : EntityTypeConfiguration<TipoBebida>
    {
        public TipoBebidaConfiguration()
        {
            ToTable("TBTiposBebida").
                    HasKey(r => r.Codigo);
        }
    }
}