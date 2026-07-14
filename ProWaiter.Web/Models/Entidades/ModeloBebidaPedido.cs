using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ModeloBebidaPedido : IEntidadeBD, IValidatableObject
    {
        public int Codigo { get; set; }
        public int CodModeloPedido { get; set; }

        public short CodBebida { get; set; }
        public virtual Bebida Bebida { get; set; }

        public string Observacoes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}