using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eComponenteComposicaoRefeicaoCardapio { CodRefeicao, CodTamanho, CodComponente, Valor, CalculoProporcional }

    public class ComponenteComposicaoRefeicaoCardapio : IEntidadeBD, IValidatableObject
    {
        public short CodRefeicao { get; set; }
        public virtual Refeicao Refeicao { get; set; }

        public string CodTamanho { get; set; }
        public virtual TamanhoRefeicao Tamanho { get; set; }

        public short CodComponente { get; set; }
        public virtual ComponenteRefeicao ComponenteRefeicao { get; set; }

        public decimal Valor { get; set; }
        public bool CalculoProporcional { get; set; }
        public bool Ativo { get; set; }

        public string CodUnidade { get; set; }
        public virtual UnidadeComponenteComposicao Unidade { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {            
            List<ValidationResult> retorno = new List<ValidationResult>();

            string chave = $"CodRefeicao: {CodRefeicao} - CodTamanho: {CodTamanho} = CodComponente: {CodComponente}";

            if (string.IsNullOrWhiteSpace(CodTamanho))
                retorno.Add(new ValidationResult(this.ObterMensagemErro(chave, nameof(CodTamanho), CodTamanho)));

            return retorno;
        }
    }
}