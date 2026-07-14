using System;
using System.Collections.Generic;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.ViewModels
{
    public class RevendedorIndexViewModel
    {
        public int Codigo { get; set; }
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string Endereco { get; set; }
        public string Responsavel { get; set; }
        public string Telefone1 { get; set; }
        public string Telefone2 { get; set; }
        public bool Ativo { get; set; }
    }
}
