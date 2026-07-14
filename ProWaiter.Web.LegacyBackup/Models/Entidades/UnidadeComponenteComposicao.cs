using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eUnidadeComponenteComposicao { Codigo, Descricao }

    public class UnidadeComponenteComposicao : IValidatableObject, IEntidadeBD
    {
        public const string CodPartes = "PR";
        public const string CodPorcao = "PÇ";
        public const string CodUnidade = "UN";

        public const int TamMaxDescricao = 20;
        public const int TamMaxCodigo = 2;

        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public UnidadeComponenteComposicao() { }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}