using Microsoft.Owin;
using Owin;
using ProWaiter.Web.Models.Entidades;
using ProWaiter.Web.Util;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(ProWaiter.Web.Startup))]
namespace ProWaiter.Web
{
    public partial class Startup
    {
        internal const string MsgValicadaoExpirou = "A validação expirou!";
        internal const string MsgValidacao = "ATENÇÃO - VOCÊ DEVE SE CONECTAR A INTERNET PARA ATIVAR O PROWAITER EM {0} DIA(S)!";
        public static string MsgResultadoValidacao = "";
        public static Restaurante Restaurante { get; internal set; }        
        

        public void Configuration(IAppBuilder app)
        {          
            ConfigureAuth(app);
        }
    }
}


//TODO: Verificar questão da mesa nos detalhes do pedido interno, mostara em qual mesa esteve
//TODO: Verificar tela de usuários
//TODO: Verificar de remover o email do cadastro de novo usuário