using NewSharp.BancoDeDados;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eUF {  Codigo, Nome }

    public class UF : IValidatableObject, IEntidadeBD
    {
        public static int TamCodigo = 2;
        public static int TamMaxNome = 30;

        public UF()
        {
            Cidades = new ObservableCollection<Cidade>();
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public virtual ICollection<Cidade> Cidades { get; protected set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Codigo, Nome);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Codigo) || Codigo.Length != TamCodigo)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(null, eUF.Codigo.ToString(), Codigo)));

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eUF.Nome.ToString(), Nome)));

            return validacoes;
        }
    }
}