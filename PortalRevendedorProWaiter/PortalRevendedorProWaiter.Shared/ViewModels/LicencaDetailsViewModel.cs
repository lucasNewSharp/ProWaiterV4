using System;
using System.Collections.Generic;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.ViewModels
{
    public class LicencaDetailsViewModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public DateTime? DataValidacao { get; set; }
        public DateTime? DataAtivacao { get; set; }
        public string ProcessorID { get; set; }
        public string Segredo { get; set; }
        public bool Ativo { get; set; }
        public short QuantidadeAtivacoes { get; set; }
        public string VersaoProWaiter { get; set; }
        public string VersaoAPP { get; set; }
        public string Revendedor { get; set; }
    }
}
