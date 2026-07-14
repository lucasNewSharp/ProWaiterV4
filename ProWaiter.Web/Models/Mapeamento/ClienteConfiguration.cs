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
    internal class ClienteConfiguration : EntityTypeConfiguration<Cliente>
    {
        public ClienteConfiguration()
        {
            ToTable("TBClientes")
                .HasKey(c => c.Codigo);

            Property(c => c.Nome)
                .HasMaxLength(Cliente.TamMaxNome);

            HasMany(c => c.Enderecos)
                 .WithRequired()
                 .HasForeignKey(e => e.CodCliente);
        }
    }
}
