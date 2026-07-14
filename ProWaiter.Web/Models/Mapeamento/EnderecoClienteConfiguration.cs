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
    public class EnderecoClienteConfiguration : IEntityTypeConfiguration<EnderecoCliente>
    {
        public void Configure(EntityTypeBuilder<EnderecoCliente> builder)
        {
            builder.ToTable("TBEnderecosClientes")
                .HasKey(e => e.Codigo);

            builder.Property(e => e.Codigo)
                .ValueGeneratedOnAdd();

            builder.HasOne(e => e.Cliente).WithMany().IsRequired()
                .HasForeignKey(e => e.CodCliente);

            builder.HasOne(c => c.Cidade)
                .WithMany()
                .HasForeignKey(c => c.CodCidade);

            builder.Property(c => c.ValorEntregaPadrao)
               .HasPrecision(6, 2);

            builder.Property(c => c.Endereco)
                .IsRequired(false)
                .HasMaxLength(EnderecoCliente.TamMaxEndereco);

            builder.Property(c => c.Bairro)
                .IsRequired(false)
                .HasMaxLength(EnderecoCliente.TamMaxBairro);
        }
    }
}