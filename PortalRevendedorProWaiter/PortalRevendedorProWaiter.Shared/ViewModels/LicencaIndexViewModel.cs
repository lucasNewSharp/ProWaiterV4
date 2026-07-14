using System;
using System.Collections.Generic;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.ViewModels
{
    public class LicencaIndexViewModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Segredo { get; set; }
        public bool Ativo { get; set; }
        public string VersaoProWaiter { get; set; }
        public string VersaoAPP { get; set; }
        public string Revendedor { get; set; }
    }
}
