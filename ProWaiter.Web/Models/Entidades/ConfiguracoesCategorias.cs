using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ConfiguracoesCategorias
    {
        public string ID { get; set; }
        public string Nome { get; set; }
        public byte? Posicao { get; set; }
        public string CorFundo { get; set; }
        public string CorFonte { get; set; }
    }
}