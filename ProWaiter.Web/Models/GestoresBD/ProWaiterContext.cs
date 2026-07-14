using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.Mapeamento;
using System.Reflection;

namespace ProWaiter.Web.Models.GestoresBD
{
    public class ProWaiterContext : IdentityDbContext<ApplicationUser>
    {
        public ProWaiterContext() { } // Para facilitar refatoração gradativa se necessário
        
        public ProWaiterContext(DbContextOptions<ProWaiterContext> options) : base(options) { }

        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<TipoRefeicao> TiposRefeicao { get; set; }
        public DbSet<Refeicao> Refeicoes { get; set; }
        public DbSet<TamanhoRefeicao> TamanhosRefeicao { get; set; }
        public DbSet<UF> UFs { get; set; }
        public DbSet<Bebida> Bebidas { get; set; }
        public DbSet<ComponenteRefeicao> ComponentesRefeicao { get; set; }
        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<BebidaDoPedido> BebidasDosPedidos { get; set; }
        public DbSet<RefeicaoDoCardapio> RefeicoesDoCardapio { get; set; }
        public DbSet<RefeicaoDoPedido> RefeicoesDoPedido { get; set; }
        public DbSet<PedidoExterno> PedidosExternos { get; set; }
        public DbSet<PedidoInterno> PedidosInternos { get; set; }
        public DbSet<PedidoParaLevar> PedidosParaLevar { get; set; }
        public DbSet<TipoBebida> TiposBebida { get; set; }
        public DbSet<Impressora> Impressoras { get; set; }
        public DbSet<LocalInterno> LocaisInternos { get; set; }
        public DbSet<Configuracao> Configuracoes { get; set; }
        public DbSet<ComponenteComposicaoRefeicaoCardapio> ComponentesComposicaoRefeicoesCardapio { get; set; }
        public DbSet<UnidadeComponenteComposicao> UnidadesComponenteComposicao { get; set; }
        public DbSet<EnderecoCliente> EnderecosClientes { get; set; }
        public DbSet<ItemBalcao> ItensBacao { get; set; }
        public DbSet<ItemBalcaoDoPedido> ItensBalcaoDoPedido { get; set; }
        public DbSet<Licenca> Licencas { get; set; }
        public DbSet<ModeloPedido> ModelosPedidos { get; set; }
        public DbSet<ConfiguracoesCategorias> ConfiguracoesCategorias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProWaiterV4;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
