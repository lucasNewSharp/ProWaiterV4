using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eMesa { Codigo, Descricao, CodUltimoPedido }

    public class Mesa : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxDescricao = 20;

        public Mesa() { }
        public Mesa(string descricao)
        {
            this.Descricao = descricao.Trim();
        }

        [Display(Name ="Código")]
        public short Codigo { get; set; }

        [Display(Name ="Descrição")]
        public string Descricao { get; set; }

        [Display(Name ="Último Pedido")]
        public int? CodUltimoPedido { get; set; }
        public virtual Pedido UltimoPedido { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Descricao) || Descricao.Length > TamMaxDescricao)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eMesa.Descricao.ToString(), Descricao)));

            return retorno;
        }

        public override string ToString()
        {
            return Descricao;
        }
    }
}
