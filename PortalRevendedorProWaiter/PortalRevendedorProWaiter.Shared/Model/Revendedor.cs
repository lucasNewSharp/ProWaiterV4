using PortalRevendedorProWaiter.Shared.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.Model
{
    [Table("TBRevendedores")]
    public class Revendedor
    {
        [Key]
        public int Codigo { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(18, MinimumLength = 18)]
        public string CNPJ { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(500)]
        public string RazaoSocial { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(500)]
        public string Endereco { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(200)]
        public string Responsavel { get; set; }

        [CampoRequeridoObrigatorio]
        [StringLength(14, MinimumLength = 14)]
        public string Telefone1 { get; set; }

        [StringLength(14, MinimumLength = 14)]
        public string Telefone2 { get; set; }

        public bool Ativo { get; set; }

        public virtual ICollection<Licenca> Licencas { get; set; }
        public virtual ICollection<UsuarioDoRevendedor> Usuarios { get; set; }
    }
}
