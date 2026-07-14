using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProWaiter.Web.Models.Entidades
{    
    public class Refeicao : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 100;

        public short Codigo { get; set; }        
        public string Nome { get; set; }
        public short CodTipo { get; set; }

        public virtual TipoRefeicao Tipo { get; set; }

        [Display(Name = "Componentes")]
        public virtual ICollection<ComponenteRefeicao> ComponentesRefeicao { get; set; }

        public Refeicao()
        {
            ComponentesRefeicao = new List<ComponenteRefeicao>();
        }

        public Refeicao(string nome, TipoRefeicao tipo): this()
        {
            ComponentesRefeicao = new List<ComponenteRefeicao>();
            Tipo = tipo ?? throw new ArgumentNullException("tipo");
            Nome = nome.Trim();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo, nameof(Nome), Nome)));
         
            return retorno;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
