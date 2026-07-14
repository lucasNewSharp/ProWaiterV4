using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum ePedido { Codigo, DataInicio, DataTermino, NomeUsuario, Acrescimos, Descontos, Observacoes }
    public abstract class Pedido : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNomeUsuario = 256;

        protected Pedido()
        {
            DataInicio = DateTime.Now;
            BebidasDoPedido = new List<BebidaDoPedido>();
            RefeicoesDoPedido = new List<RefeicaoDoPedido>();
            ItensBalcaoDoPedido = new List<ItemBalcaoDoPedido>();
        }
        protected Pedido(DateTime dataInicio, string nomeUsuario, decimal acrescimos, decimal descontos, string observacoes) : this()
        {
            NomeUsuario = nomeUsuario;

            DataInicio = dataInicio;
            Acrescimos = acrescimos;
            Descontos = descontos;
            Observacoes = observacoes;
        }

        public int Codigo { get; set; }
        [Display(Name = "Início")]
        public DateTime DataInicio { get; set; }
        [Display(Name = "Término")]
        public DateTime? DataTermino { get; set; }
        [Display(Name = "Usuário")]
        public string NomeUsuario { get; set; }
        [Display(Name = "Acréscimos")]
        public Decimal Acrescimos { get; set; }
        [Display(Name = "Descontos")]
        public Decimal Descontos { get; set; }
        [Display(Name = "Observações")]
        public string Observacoes { get; set; }
        [Display(Name = "Bebidas")]
        public virtual ICollection<BebidaDoPedido> BebidasDoPedido { get; protected set; }
        [Display(Name = "Refeições")]
        public virtual ICollection<RefeicaoDoPedido> RefeicoesDoPedido { get; protected set; }
        [Display(Name = "Itens de balcão")]
        public virtual ICollection<ItemBalcaoDoPedido> ItensBalcaoDoPedido { get; protected set; }

        public bool TodosItensEnviados
        {
            get
            {
                return (BebidasDoPedido == null || BebidasDoPedido.Count() == 0 || BebidasDoPedido.All(b => b.Enviado))
                    && (RefeicoesDoPedido == null || RefeicoesDoPedido.Count() == 0 || RefeicoesDoPedido.All(r => r.Enviado));
            }
        }

        [Display(Name = "Total Bebidas")]
        public decimal ValorBebidas
        {
            get { return BebidasDoPedido.Sum(b => b.Valor); }
        }
        [Display(Name = "Total Reifeições")]
        public decimal ValorRefeicoes
        {
            get { return RefeicoesDoPedido.Sum(r => r.Valor + r.Acrescimo); }
        }
        [Display(Name = "Total Itens de Balcão")]
        public decimal ValorItensDeBalcao
        {
            get { return ItensBalcaoDoPedido.Sum(i => i.Valor); }
        }
        [Display(Name = "Total")]
        public decimal ValorTotal
        {
            get 
            {
                decimal valorEntrega = 0;

                if (this is PedidoExterno)
                    valorEntrega = ((PedidoExterno)this).ValorEntrega;

                return ValorBebidas + ValorRefeicoes + ValorItensDeBalcao + Acrescimos + valorEntrega - Descontos; 
            }
        }
        public override string ToString()
        {
            return string.Format("Pedido abstrato {0}", Codigo);
        }
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (DataTermino.HasValue && DataTermino.Value < DataInicio)
                validacoes.Add(new ValidationResult("A DataTermino deve ser nula ou maior do que a DataInicio do Pedido " + Codigo.ToString() + "!"));
            if (Acrescimos < 0)
                validacoes.Add(new ValidationResult("Os acréscimos do pedido (" + Codigo.ToString() + ") devem ser maior ou igual a zero!"));
            if (Descontos < 0)
                validacoes.Add(new ValidationResult("Os descontos do pedido (" + Codigo.ToString() + ") devem ser maior ou igual a zero!"));

            return validacoes;
        }
    }
}
