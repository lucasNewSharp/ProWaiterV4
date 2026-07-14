using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ModeloComponenteRefeicaoPedido : IEntidadeBD, IValidatableObject
    {
        [Key, Column(Order = 1)]
        public int CodModeloRefeicaoPedido { get; set; }
        public virtual ModeloRefeicaoPedido ModeloRefeicaoDoPedido { get; set; }
        [Key, Column(Order = 2)]
        public short CodComponente { get; set; }
        public virtual ComponenteRefeicao ComponenteRefeicao { get; set; }
        public byte Quantidade { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}