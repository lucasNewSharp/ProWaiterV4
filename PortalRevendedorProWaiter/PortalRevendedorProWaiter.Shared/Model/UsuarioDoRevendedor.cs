using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.Model
{
    [Table("TBUsuariosDoRevendedor")]
    public class UsuarioDoRevendedor
    {
        [Key]
        public int Codigo { get; set; }

        [ForeignKey(nameof(Revendedor))]
        public int CodRevendedor { get; set; }
        public virtual Revendedor Revendedor { get; set; }

        public string AspNetUserId { get; set; }        
    }
}
