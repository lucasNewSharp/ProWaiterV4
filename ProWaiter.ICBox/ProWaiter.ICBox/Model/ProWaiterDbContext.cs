using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.ICBox.Model
{
    public class ProWaiterDbContext : DbContext
    {
        public const string NomeConnectionString = "StringConexao";
        private static string _stringDeConexao;
        public static string StringDeConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringDeConexao))
                    _stringDeConexao = ConfigurationManager.ConnectionStrings[NomeConnectionString].ConnectionString;
                return _stringDeConexao;
            }
        }


        public DbSet<Configuracao> Configuracoes { get; set; }

        public ProWaiterDbContext() : base(StringDeConexao)
        {
            Database.SetInitializer<ProWaiterDbContext>(null);
        }
    }
}
