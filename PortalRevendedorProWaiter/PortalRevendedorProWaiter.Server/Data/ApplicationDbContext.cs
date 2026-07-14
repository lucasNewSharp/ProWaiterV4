using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalRevendedorProWaiter.Shared.Model;

namespace PortalRevendedorProWaiter.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UltimaVersaoProWaiter> UltimaVersaoProWaiter { get; set; }
        public DbSet<Revendedor> Revendedores { get; set; }
        public DbSet<UsuarioDoRevendedor> UsuariosDoRevendedor { get; set; }
        public DbSet<Licenca> Licencas { get; set; }
    }
}
