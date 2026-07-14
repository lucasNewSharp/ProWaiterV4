using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.ICBox.Model
{
    [Table("TBConfiguracoes")]
    public class Configuracao
    {
        [Key]
        public string Codigo { get; set; }
        public string Valor { get; set; }
    }
}
