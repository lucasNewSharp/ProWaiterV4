using NewSharp.BancoDeDados;
using ProWaiter.Web.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public enum eImpressora { Codigo, Nome, Local, NomeTipoImpressao, EhDoCaixa, EhDeEntrega, Ip, Porta }

    public class Impressora : IEntidadeBD, IValidatableObject
    {
        public const int TamMaxNome = 100;
        public const int TamMaxLocal = 50;
        public const int TamMaxTipoImpressao = 50;
        public const int TamMaxIp = 15;

        public byte Codigo { get; set; }
        public string Nome { get; set; }
        public string Local { get; set; }
        [Display(Name = "Tipo de Impressão")]
        public string NomeTipoImpressao { get; set; }
        [Display(Name = "Tipo de Impressão")]
        public Type TipoImpressao
        {
            get { return Type.GetType(NomeTipoImpressao); }
        }
        public string NomeExibicao { get { return String.Format("{0} ({1})", Local, Nome); } }
        [Display(Name = "Tipo de Impressão")]
        public string NomeExibicaoTipoImpressao { get { return GestorImpressoes.Instancia.ObterNomeParaExibicao(TipoImpressao); } }

        [Display(Name = "Caixa")]
        public bool EhDoCaixa { get; set; }

        [Display(Name = "Entrega")]
        public bool EhDeEntrega { get; set; }

        [Display(Name ="Buzina Ativada")]
        public bool BuzinaAtivada { get; set; }

        [Display(Name = "IP")]
        public string Ip { get; set; }
        public int Porta { get; set; }

        public Impressora() { }
        public Impressora(string nome, string local, string nomeTipoImpressao, bool ehDoCaixa, bool ehDeEntrega, string ip, int porta)
        {
            if (String.IsNullOrEmpty(nome))
                throw new ArgumentNullException("nome");
            if (String.IsNullOrEmpty(local))
                throw new ArgumentNullException("local");
            if (String.IsNullOrEmpty(nomeTipoImpressao))
                throw new ArgumentNullException("nomeTipoImpressora");

            Nome = nome.Trim();
            Local = local.Trim();
            NomeTipoImpressao = nomeTipoImpressao.Trim();

            EhDoCaixa = ehDoCaixa;
            EhDeEntrega = ehDeEntrega;
            Ip = ip ?? throw new ArgumentNullException(nameof(Ip));
            Porta = porta;
        }

        public override string ToString()
        {
            return Local;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validacoes = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nome) || Nome.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eImpressora.Nome.ToString(), Nome)));
            if (string.IsNullOrWhiteSpace(Local) || Local.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eImpressora.Local.ToString(), Local)));
            if (string.IsNullOrWhiteSpace(NomeTipoImpressao) || NomeTipoImpressao.Length > TamMaxNome)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), eImpressora.NomeTipoImpressao.ToString(), NomeTipoImpressao)));
            if(string.IsNullOrWhiteSpace(Ip) || Ip.Length > TamMaxIp)
                validacoes.Add(new ValidationResult(this.ObterMensagemErro(Codigo.ToString(), nameof(Ip), Ip)));

            return validacoes;
        }
    }
}