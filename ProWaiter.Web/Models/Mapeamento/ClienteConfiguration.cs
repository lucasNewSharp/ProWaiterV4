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
    internal class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("TBClientes")
                .HasKey(c => c.Codigo);

            builder.Property(c => c.Nome)
                .HasMaxLength(Cliente.TamMaxNome);

            builder.HasMany(c => c.Enderecos)
                 .WithOne().IsRequired()
                 .HasForeignKey(e => e.CodCliente);
        }
    }
}
