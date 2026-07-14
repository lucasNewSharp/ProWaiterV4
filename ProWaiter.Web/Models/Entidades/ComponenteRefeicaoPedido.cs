using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eComponenteRefeicaoPedido { CodRefeicaoPedido, CodComponente, Quantidade }
    
    public class ComponenteRefeicaoPedido : IEntidadeBD, IValidatableObject
    {
        [Key, Column(Order = 1)]
        public int CodRefeicaoPedido { get; set; }        
        public virtual RefeicaoDoPedido RefeicaoDoPedido { get; set; }
        [Key, Column(Order = 2)]
        public short CodComponente { get; set; }

        
        public virtual ComponenteRefeicao ComponenteRefeicao { get; set; }

        public byte Quantidade { get; set; }

        public ComponenteRefeicaoPedido() { }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}