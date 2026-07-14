using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProWaiter.Licenca.Entidades;

namespace ProWaiter.Licenca.Gestores
{
    public class LicencasContext : DbContext
    {        
        private static string _stringDeConexao = null;
        public static string StringDeConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringDeConexao))
                    _stringDeConexao = ConfigurationManager.ConnectionStrings["StringConexao"].ConnectionString;
                return _stringDeConexao;
            }
        }

        public LicencasContext() : base(StringDeConexao)
        {
            Database.SetInitializer<LicencasContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LicencaProWaiter>().Property(p => p.Codigo).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        public DbSet<LicencaProWaiter> Licencas { get; set; }
        public DbSet<Configuracao> Configuracoes { get; set; }
    }
}
