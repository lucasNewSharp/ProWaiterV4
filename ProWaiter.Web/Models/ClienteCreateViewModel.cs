using ProWaiter.Web.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models
{
    public class ClienteCreateViewModel
    {
        public ClienteCreateViewModel()
        {            
        }

        public int Codigo { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }


        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Endereco { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Bairro { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int? CodCidade { get; set; }
        [Display(Name = "Cidade")]
        public virtual Cidade Cidade { get; set; }
        [Display(Name = "Telefone 1")]
        public string Telefone1 { get; set; }
        [Display(Name = "Telefone 2")]
        public string Telefone2 { get; set; }

        [Display(Name = "Valor padrão entrega: R$")]
        public decimal ValorEntregaPadrao { get; set; }

        [Display(Name = "Observações")]
        public string ObservacoesPadrao { get; set; }
    }
}