using ProWaiter.Web.Models;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProWaiter.Web.APIs
{
    public class ConfiguracoesController : ApiController
    {
        class ConfiguracoesDTO
        {
            public bool UtilizaComanda { get; set; }
            public bool RequerObservacaoAoAbrirPedidoInterno { get; set; }

            public ConfiguracoesDTO(Configuracoes config)
            {
                UtilizaComanda = config.UtilizaComanda;
                RequerObservacaoAoAbrirPedidoInterno = config.RequerObservacaoAoAbrirPedidoInterno;
            }
        }

        [ResponseType(typeof(ConfiguracoesDTO))]        
        public IHttpActionResult GetConfiguracoes()
        {
            return Ok(new ConfiguracoesDTO(Configuracoes.ObterInstancia()));
        }
    }
}
