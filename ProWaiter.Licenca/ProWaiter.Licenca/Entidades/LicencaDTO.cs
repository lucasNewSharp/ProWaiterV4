using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Licenca.Entidades
{
    public class LicencaDTO
    {
        public const int TamMaxString = 100;
        public const int TamUF = 2;
        public const int TamSegredo = 32;

        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Segredo { get; set; }
    }
}
