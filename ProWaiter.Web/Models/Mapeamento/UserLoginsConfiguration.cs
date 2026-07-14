using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Mapeamento
{
    internal class UserLoginsConfiguration : EntityTypeConfiguration<IdentityUserLogin>
    {
        public UserLoginsConfiguration()
        {
            ToTable("AspNetUserLogins")
                .HasKey(a => new { a.LoginProvider, a.ProviderKey, a.UserId });
        }
    }
}