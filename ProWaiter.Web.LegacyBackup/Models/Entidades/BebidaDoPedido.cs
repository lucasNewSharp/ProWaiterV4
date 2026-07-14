using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eBebidaDoPedido { Codigo, CodPedido, CodBebida, Observacoes, Valor }
    public class BebidaDoPedido : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxObservacoes = int.MaxValue;

        public BebidaDoPedido() { }
        public BebidaDoPedido(Pedido pedido, Bebida bebida)
        {
            if (pedido == null)
                throw new ArgumentNullException("pedido");
            CodPedido = pedido.Codigo;
            Bebida = bebida ?? throw new ArgumentNullException("bebida");            
            AplicarDesconto();
        }

        public int Codigo { get; set; }
        public int CodPedido { get; set; }
        public short CodBebida { get; set; }
        public virtual Bebida Bebida { get; set; }
        public string Observacoes { get; set; }
        public decimal Valor { get; set; }
        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }
        public DateTime? DataHora { get; set; }
        public decimal PercDesconto { get; set; }

        public bool Enviado { get; set; }

        public void AplicarDesconto()
        {
            PercDesconto = Bebida.PercDesconto;
            Valor = Bebida.PercDesconto > 0 ? (Bebida.Valor * ((100 - Bebida.PercDesconto) / 100)) : Bebida.Valor;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(Observacoes) && Observacoes.Length > TamMaxObservacoes)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eBebidaDoPedido.Observacoes.ToString(), Observacoes)));

            if (Valor < 0)
                validacoes.Add(new ValidationResult("O Valor da bebida do pedido (" + Codigo.ToString() + " do pedido " + CodPedido.ToString() + ") devem ser maior ou igual a zero!"));

            return validacoes;
        }
    }
}
