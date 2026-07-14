using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProWaiter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            HtmlHelper.ClientValidationEnabled = true;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Configuração necessária pois como temos uma atribuição many to many com uma
            //auto referencia com a tabla TBAtribComponentesRefeicaoPedido, que é um objeto
            //e está refenrenciada na TBAtribRefeioesPedido, temos a classe ComponenteRefeicaoPedido 
            //que possui a própria RefeicaoDoPedido, então o JSon da api entra em loop pois quando
            //vai instanciar a atribuição "componente", instancia a propria refeição do pedido, que por sua vez possui o componente, que por sua
            //vez possui a propria refeição do pedido etc.....
            //Então dizemos para o Json para ignorar o loop de referencia

            //Essa linha me custou horas de trabalho pra descobrir.
            HttpConfiguration config = GlobalConfiguration.Configuration;
            
            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Messages";
            DefaultModelBinder.ResourceClassKey = "Messages";
        }
    }
}
