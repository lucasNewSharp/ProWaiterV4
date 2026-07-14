using Microsoft.EntityFrameworkCore;
using ProWaiter.Web.Models;
using ProWaiter.Web.Models.GestoresBD;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ProWaiter.Web.APIs
{
    public class ConfiguracoesController : ControllerBase
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

        [ProducesResponseType(typeof(ConfiguracoesDTO), 200)]        
        public IActionResult GetConfiguracoes()
        {
            return Ok(new ConfiguracoesDTO(Configuracoes.ObterInstancia()));
        }
    }
}
