using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class IdentityUserRoleConfiguration : EntityTypeConfiguration<IdentityUserRole>
    {
        public IdentityUserRoleConfiguration()
        {
            ToTable("AspNetUserRoles")
                .HasKey(i => new { i.UserId, i.RoleId });
        }
    }
}