using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Licenca.Entidades
{
    [Table("TBLicenca")]
    public class LicencaProWaiter
    {
        public const int TamMaxNome = 100;
        public const int TamMaxEndereco = 100;
        public const int TamMaxCidade = 100;
        public const int TamUf = 2;
        public const int TamSegredo = 32;

        [Key]
        public byte Codigo { get; set; }

        [Required]
        [StringLength(TamMaxNome)]
        public string Nome { get; set; }

        [Required]
        [StringLength(TamMaxEndereco)]
        public string Endereco { get; set; }

        [Required]
        [StringLength(TamMaxCidade)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(TamUf, MinimumLength = TamUf)]
        public string UF { get; set; }

        [Required]
        [StringLength(TamSegredo, MinimumLength = TamSegredo)]
        public string Segredo { get; set; }

        [Required]
        public DateTime DataAtivacao { get; set; }

        [Required]
        public long Validacao { get; set; }

        [Required]
        public bool Ativo { get; set; }
    }
}
