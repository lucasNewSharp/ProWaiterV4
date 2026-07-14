using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    public class EnderecoClienteConfiguration : EntityTypeConfiguration<EnderecoCliente>
    {
        public EnderecoClienteConfiguration()
        {
            ToTable("TBEnderecosClientes")
                .HasKey(e => e.Codigo);

            Property(e => e.Codigo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(e => e.Cliente)
                .WithMany()
                .HasForeignKey(e => e.CodCliente);

            HasOptional(c => c.Cidade)
                .WithMany()
                .HasForeignKey(c => c.CodCidade);

            Property(c => c.ValorEntregaPadrao)
               .HasPrecision(6, 2);

            Property(c => c.Endereco)
                .IsOptional()
                .HasMaxLength(EnderecoCliente.TamMaxEndereco);

            Property(c => c.Bairro)
                .IsOptional()
                .HasMaxLength(EnderecoCliente.TamMaxBairro);
        }
    }
}