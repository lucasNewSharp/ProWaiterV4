using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eCliente { Codigo, Nome, Endereco, Bairro, CodCidade, Telefone1, Telefone2 }
    public class Cliente : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 100;
        public const int TamMaxTelefone = 40;

        public Cliente() 
        {
            Enderecos = new List<EnderecoCliente>();
        }        

        public int Codigo { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        public virtual List<EnderecoCliente> Enderecos { get; set; }

        [Display(Name = "Telefone 1")]
        public string Telefone1 { get; set; }
        [Display(Name = "Telefone 2")]
        public string Telefone2 { get; set; }

        public override string ToString()
        {
            return Nome;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eCliente.Nome.ToString(), Nome)));

            if (!string.IsNullOrEmpty(Telefone1) && Telefone1.Length > TamMaxTelefone)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eCliente.Telefone1.ToString(), Telefone1)));

            if (!string.IsNullOrEmpty(Telefone2) && Telefone2.Length > TamMaxTelefone)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eCliente.Telefone2.ToString(), Telefone2)));

            return retorno;
        }
    }
}
