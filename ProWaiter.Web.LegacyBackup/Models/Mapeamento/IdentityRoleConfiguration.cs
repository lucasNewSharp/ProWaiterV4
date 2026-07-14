using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class IdentityRoleConfiguration : EntityTypeConfiguration<IdentityRole>
    {
        public IdentityRoleConfiguration()
        {
            ToTable("AspNetRoles")
                .HasKey(r => r.Id);

            HasMany(g => g.Users)
                .WithRequired()
                .HasForeignKey(g => g.UserId);
        }
    }
}