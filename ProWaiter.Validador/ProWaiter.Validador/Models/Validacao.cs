using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProWaiter.Validador.Models
{
    [Table("TBLicencas")]
    public class Validacao
    {
        public Validacao() { }

        [Column("Codigo")]
        [Key]
        public int Codigo { get; set; }
        [Column("Nome")]
        public string Nome { get; set; }
        [Column("Endereco")]
        public string Endereco { get; set; }
        [Column("Cidade")]
        public string Cidade { get; set; }
        [Column("UF")]
        public string UF { get; set; }
        [Column("DataValidacao")]
        public DateTime? DataValidacao { get; set; }
        [Column("DataAtivacao")]
        public DateTime? DataAtivacao { get; set; }
        [Column("ProcessorID")]
        public string ProcessorID { get; set; }
        [Column("Segredo")]
        public string Segredo { get; set; }
        [Column("Ativo")]
        public bool Ativo { get; set; }
        [Column("QuantidadeAtivacoes")]
        public short QuantidadeAtivacoes { get; set; }

        [Column("CodRevendedor")]
        [ForeignKey(nameof(Revendedor))]
        public int? CodRevendedor { get; set; }
        public virtual Revendedor Revendedor { get; set; }

        [Column("LiberarNotificacaoAtualizacao")]
        public bool LiberarNotificacaoAtualizacao { get; set; }
        [Column("VersaoProWaiter")]
        public string VersaoProWaiter { get; set; }
        [Column("VersaoApp")]
        public string VersaoApp { get; set; }

    }
}
