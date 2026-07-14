using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    [Serializable]
    public class Restaurante
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(2)]
        public string UF { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Segredo { get; set; }

        [Display(Name = "Data de ativação")]
        public DateTime? DataAtivacao { get; set; }

        public long Validacao { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}