using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eConfiguracao { Codigo, Valor }

    public class Configuracao : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxCodigo = 256;

        public string Codigo { get; set; }
        public string Valor { get; set; }

        public Configuracao() { }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}