using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortalRevendedorProWaiter.Shared.Model
{
    [Table("TBUltimaVersaoProWaiter")]
    public class UltimaVersaoProWaiter
    {        
        [Key]
        [StringLength(10)]
        public string VersaoAtual { get; set; }
    }
}
