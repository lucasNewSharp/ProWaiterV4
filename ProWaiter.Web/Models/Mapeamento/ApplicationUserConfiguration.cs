using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class ApplicationUserConfiguration : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserConfiguration()
        {
            ToTable("AspNetUsers")
                .HasKey(a => a.Id);

            HasMany(u => u.Roles)
              .WithRequired()
              .HasForeignKey(u => u.UserId);
        }
    }
}