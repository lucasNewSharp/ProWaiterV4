using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Licenca.Entidades
{
    [Table("TBConfiguracoes")]
    public class Configuracao
    {
        public const int TamMaxCodigo = 256;

        public const string CodVersao = "Versao";
        public const string CodValidador = "Validador";
        
        [Key]
        public string Codigo { get; set; }
        [Required]
        [StringLength(TamMaxCodigo)]
        public string Valor { get; set; }

        public Configuracao() { }
    }
}
