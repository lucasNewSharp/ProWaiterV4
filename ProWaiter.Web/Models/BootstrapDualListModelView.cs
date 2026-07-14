using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models
{
    public class BootstrapDualListModelView
    {
        public BootstrapDualListModelView()
        {
            this.ListaDisponiveis = new List<KeyValuePair<string, string>>();
            this.ListaSelecionados = new List<KeyValuePair<string, string>>();
        }
        public List<KeyValuePair<string,string>> ListaDisponiveis { get; set; }
        public List<KeyValuePair<string, string>> ListaSelecionados { get; set; }
    }
}