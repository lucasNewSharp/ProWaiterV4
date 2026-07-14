using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ModeloRefeicaoPedido : IEntidadeBD, IValidatableObject
    {
        public int Codigo { get; set; }
        public int CodModeloPedido { get; set; }

        public short CodRefeicao { get; set; }
        public virtual RefeicaoDoCardapio RefeicaoDoCardapio { get; set; }

        public string CodTamanho { get; set; }
        public virtual TamanhoRefeicao Tamanho { get; set; }

        public virtual ICollection<ModeloComponenteRefeicaoPedido> ModeloComponentesRefeicaoPedido { get; set; }

        public string Observacoes { get; set; }

        public ModeloRefeicaoPedido()
        {
            ModeloComponentesRefeicaoPedido = new List<ModeloComponenteRefeicaoPedido>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return null;
        }
    }
}