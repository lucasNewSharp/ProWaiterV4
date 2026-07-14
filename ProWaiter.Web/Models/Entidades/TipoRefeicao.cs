using NewSharp.BancoDeDados;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eTipoRefeicao { Codigo, Nome, Posicao, CorFundo, CorFonte }

    public class TipoRefeicao : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 50;

        public TipoRefeicao()
        {
            CorFonte = "#000000";
            CorFundo = "#ffffff";
        }

        public TipoRefeicao(string nome) : this()
        {
            Nome = nome.Trim();
        }

        public short Codigo { get; set; }
        public string Nome { get; set; }
        public byte? Posicao { get; set; }
        public string CorFundo { get; set; }
        public string CorFonte { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> retorno = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Nome) || Nome.Length > TamMaxNome)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eTipoRefeicao.Nome.ToString(), Nome)));
            if (!Regex.Match(CorFundo, Constantes.RegexCor).Success)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eTipoRefeicao.CorFundo.ToString(), CorFundo)));
            if (!Regex.Match(CorFonte, Constantes.RegexCor).Success)
                retorno.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eTipoRefeicao.CorFonte.ToString(), CorFonte)));
            return retorno;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
