using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProWaiter.Validador.Models
{
    [Table("TBRevendedores")]
    public class Revendedor
    {
        [Key]
        [Column("Codigo")]
        public int Codigo { get; set; }
        [Column("CNPJ")]
        public string CNPJ { get; set; }
        [Column("RazaoSocial")]
        public string RazaoSocial { get; set; }
        [Column("Endereco")]
        public string Endereco { get; set; }
        [Column("Responsavel")]
        public string Responsavel { get; set; }
        [Column("Telefone1")]
        public string Telefone1 { get; set; }
        [Column("Telefone2")]
        public string Telefone2 { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Ativo")]
        public bool Ativo { get; set; }
    }
}
