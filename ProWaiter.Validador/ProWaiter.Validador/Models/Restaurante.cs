using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProWaiter.Validador.Models
{
    public class Restaurante
    {
        public Restaurante() { }

        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Segredo { get; set; }
        public DateTime? DataAtivacao { get; set; }
        public long Validacao { get; set; }
        public string VersaoProWaiter { get; set; }
        public string VersaoAPP { get; set; }
    }
}
