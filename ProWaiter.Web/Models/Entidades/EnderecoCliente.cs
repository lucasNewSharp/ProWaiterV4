using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public class EnderecoCliente : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxEndereco = 100;
        public const int TamMaxBairro = 50;        

        public int Codigo { get; set; }        

        public int CodCliente { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Bairro { get; set; }

        [Display(Name = "Cidade")]        
        public int? CodCidade { get; set; }
        [Display(Name = "Cidade")]
        public virtual Cidade Cidade { get; set; }        

        [Display(Name = "Entrega: R$")]
        public decimal ValorEntregaPadrao { get; set; }

        [Display(Name = "Observações")]
        public string ObservacoesPadrao { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(Endereco) && Endereco.Length > TamMaxEndereco)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(Endereco), Endereco)));

            if (!string.IsNullOrEmpty(Bairro) && Bairro.Length > TamMaxBairro)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(Bairro), Bairro)));

            return retorno;
        }
    }
}