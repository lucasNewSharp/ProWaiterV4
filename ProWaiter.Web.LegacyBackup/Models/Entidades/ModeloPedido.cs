using NewSharp.BancoDeDados;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class ModeloPedido : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 256;

        public int Codigo { get; set; }        
        public string Nome { get; set; }
        [Display(Name = "Desconto: R$")]                
        public decimal Desconto { get; set; }
        [Display(Name = "Acréscimo: R$")]
        public decimal Acrescimo { get; set; }
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        public virtual ICollection<ModeloBebidaPedido> ModelosBebidaPedido { get; set; }
        public virtual ICollection<ModeloRefeicaoPedido> ModelosRefeicaoPedidos { get; set; }

        public ModeloPedido()
        {
            ModelosBebidaPedido = new List<ModeloBebidaPedido>();
            ModelosRefeicaoPedidos = new List<ModeloRefeicaoPedido>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> erros = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                erros.Add(new ValidationResult($"O nome do modelo ({Codigo}) deve ter no máximo {TamMaxNome} caracteres"));
            if (Acrescimo < 0)
                erros.Add(new ValidationResult($"Os acréscimos do pedido ({ Codigo }) devem ser maior ou igual a zero!"));
            if (Desconto < 0)
                erros.Add(new ValidationResult($"Os descontos do pedido ({Codigo}) devem ser maior ou igual a zero!"));

            return erros;
        }
    }
}