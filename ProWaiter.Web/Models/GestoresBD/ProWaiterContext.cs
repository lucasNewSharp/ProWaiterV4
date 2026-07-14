using Microsoft.AspNet.Identity.EntityFramework;
using NewSharp.BancoDeDados;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Models.Mapeamento;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace ProWaiter.Web.Models.GestoresBD
{
    public class ProWaiterContext : ContextoBD
    {
        public const string NomeConnectionString = "ProWaiterContext";

        private static string _stringDeConexao = null;

        public static string StringDeConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringDeConexao))
                    _stringDeConexao = ConfigurationManager.ConnectionStrings[NomeConnectionString].ConnectionString;
                return _stringDeConexao;
            }
        }

        public ProWaiterContext() : base(NomeConnectionString)
        {
            Database.SetInitializer<ProWaiterContext>(null);
        }

        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<TipoRefeicao> TiposRefeicao { get; set; }
        public DbSet<Refeicao> Refeicoes { get; set; }
        public DbSet<TamanhoRefeicao> TamanhosRefeicao { get; set; }
        public DbSet<UF> UFs { get; set; }
        public DbSet<Bebida> Bebidas { get; set; }
        public DbSet<ComponenteRefeicao> ComponentesRefeicao { get; set; }
        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<IdentityRole> Grupos { get; set; }
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

        //Modelos

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
           
            modelBuilder.Configurations.Add(new MesaConfiguration());
            modelBuilder.Configurations.Add(new UFConfiguration());
            modelBuilder.Configurations.Add(new CidadeConfiguration());
            modelBuilder.Configurations.Add(new TipoRefeicaoConfiguration());
            modelBuilder.Configurations.Add(new TamanhoRefeicaoConfiguration());
            modelBuilder.Configurations.Add(new RefeicaoConfiguration());
            modelBuilder.Configurations.Add(new BebidaConfiguration());
            modelBuilder.Configurations.Add(new ItemBalcaoConfiguration());
            modelBuilder.Configurations.Add(new ComponenteRefeicaoConfiguration());            
            modelBuilder.Configurations.Add(new PedidoConfiguration());
            modelBuilder.Configurations.Add(new PedidoInternoConfiguration());
            modelBuilder.Configurations.Add(new PedidoExternoConfiguration());
            modelBuilder.Configurations.Add(new PedidoParaLevarConfiguration());

            modelBuilder.Configurations.Add(new ClienteConfiguration());
            modelBuilder.Configurations.Add(new EnderecoClienteConfiguration());

            modelBuilder.Configurations.Add(new ItemBalcaoDoPedidoConfiguration());
            modelBuilder.Configurations.Add(new BebidaDoPedidoConfiguration());
            modelBuilder.Configurations.Add(new RefeicaoCardapioConfiguration());
            modelBuilder.Configurations.Add(new RefeicaoDoPedidoConfiguration());
            modelBuilder.Configurations.Add(new TipoBebidaConfiguration());
            modelBuilder.Configurations.Add(new ImpressoraConfiguration());
            modelBuilder.Configurations.Add(new LocaisInternosConfiguration());
            modelBuilder.Configurations.Add(new ConfiguracaoCongituration());
            modelBuilder.Configurations.Add(new ComponenteComposicaoRefeicaoCardapioConfiguration());
            modelBuilder.Configurations.Add(new ComponenteRefeicaoPedidoConfiguration());
            modelBuilder.Configurations.Add(new UnidadeComponenteComposicaoConfiguration());

            modelBuilder.Configurations.Add(new ModeloBebidaPedidoConfiguration());
            modelBuilder.Configurations.Add(new ModeloPedidoConfiguration());
            modelBuilder.Configurations.Add(new ModeloRefeicaoPedidoConfiguration());
            modelBuilder.Configurations.Add(new ModeloComponenteRefeicaoPedidoConfiguration());


            modelBuilder.Configurations.Add(new ApplicationUserConfiguration());
            modelBuilder.Configurations.Add(new UserLoginsConfiguration());
            modelBuilder.Configurations.Add(new IdentityUserRoleConfiguration());
            modelBuilder.Configurations.Add(new IdentityRoleConfiguration());
        }
    }
}
