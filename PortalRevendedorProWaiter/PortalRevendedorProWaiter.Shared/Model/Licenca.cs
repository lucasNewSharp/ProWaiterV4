using PortalRevendedorProWaiter.Shared.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.Model
{
    [Table("TBLicencas")]
    public class Licenca
    {
        [Key]
        public int Codigo { get; set; }

        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Endereco { get; set; }

        [StringLength(100)]
        public string Cidade { get; set; }

        [StringLength(2, MinimumLength = 2)]
        public string UF { get; set; }

        public DateTime? DataValidacao { get; set; }

        public DateTime? DataAtivacao { get; set; }

        public string ProcessorID { get; set; }

        [CampoRequeridoObrigatorio]
        public string Segredo { get; set; }
        
        public bool Ativo { get; set; }

        public short QuantidadeAtivacoes { get; set; }

        [ForeignKey(nameof(Revendedor))]
        public int? CodRevendedor { get; set; }
        public virtual Revendedor Revendedor { get; set; }

        public bool LiberarNotificacaoAtualizacao { get; set; }

        [StringLength(10)]
        public string VersaoProWaiter { get; set; }

        [StringLength(10)]
        public string VersaoAPP { get; set; }
    }
}
