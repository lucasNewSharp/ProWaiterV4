using System;

namespace NewSharp.BancoDeDados
{
    public interface IEntidadeBD
    {
    }
}

namespace System
{
    public static class ClassExtensions
    {
        public static string ObterMensagemErro(this NewSharp.BancoDeDados.IEntidadeBD that, string idEntidade, string propriedade)
        {
            return ObterMensagemErro(that, idEntidade, propriedade, null);
        }

        public static string ObterMensagemErro(this NewSharp.BancoDeDados.IEntidadeBD that, int idEntidade, string propriedade, object valor)
        {
            return ObterMensagemErro(that, idEntidade.ToString(), propriedade, valor);
        }

        public static string ObterMensagemErro(this NewSharp.BancoDeDados.IEntidadeBD that, string idEntidade, string propriedade, object valor)
        {
            return string.Format(
                "Não é possível setar a propriedade {0} da entidade {1} ({2}) com valor ({3}) ",
                propriedade,
                that.GetType().Name,
                idEntidade ?? "<vazio>",
                valor ?? "<vazio>"
            );
        }

        public static string ValorOuNulo(this string that)
        {
            return string.IsNullOrWhiteSpace(that) ? null : that.Trim();
        }

        public static object ValorOuDBNull(this string that)
        {
            return string.IsNullOrWhiteSpace(that) ? DBNull.Value : (object)that.Trim();
        }
    }
}

namespace NewSharp.Ferramentas.Impressoras.Termicas
{
    public enum eAlinhamento { Padrao, Esquerda, Centralizado, Direita }
    [System.Flags] public enum eFonte { Padrao = 0, FonteA = 0, FonteB = 1, FonteC = 2, ModoDestaque = 8, AlturaDupla = 16, LarguraDupla = 32, ModoEnfase = 128 }
    public enum eLinguagem { AmericaLatina, EUA }
    public enum eTabelaDeCaracteres { EUA_PadraoEuropa, Latin9_ISO8859_15, PC860_Portugues, Latin2_ISO8859_2 }
    public enum eStatusImpressora { Normal, TampaAberta, SemPapel, Desconhecido }

    public interface IImpressoraTermica
    {
        bool ConectarImpressora(string nomeImpressora);
        bool ConectarImpressora(string ip, int porta);
        bool DesconectarImpressora();
        void IniciarImpressora();
        eStatusImpressora ObterStatusImpressora();
        bool ImprimirTexto(string texto);
        bool SelecionarAlinhamento(eAlinhamento alinhamento = eAlinhamento.Padrao);
        bool SelecionarFonte(eFonte fonte = eFonte.Padrao);
        bool ImprimirLinhasEmBranco(int numeroDeLinhas = 1);
        bool CortarPapel();
        bool AtivarAltoFalante();
        bool ExecutarAutoTeste();
        bool SelecionarConjuntoInternacionalDeCaracteres(eLinguagem linguagem = eLinguagem.AmericaLatina);
        bool SelecionarTabelaDeCodigoDeCaracteres(eTabelaDeCaracteres tabelaDeCaracteres = eTabelaDeCaracteres.EUA_PadraoEuropa);
        bool ImprimirTexto(string textoAEsquerda, string textoADireita, eFonte fonte, char charSeparador = ' ');
        int ObterNumCaracteresDaLinha(eFonte fonte);
    }

    public class RetornoImpressoraTermica
    {
        public string NomeImpressora { get; private set; }
        public bool Retorno { get; internal set; }
        public IImpressoraTermica ImpressoraTermica { get; private set; }
        public bool BuzinaAtivada { get; private set; }

        public RetornoImpressoraTermica(IImpressoraTermica impressoraTermica, string nomeImpressora, bool buzinaAtivada)
        {
            NomeImpressora = nomeImpressora;
            Retorno = true;
            ImpressoraTermica = impressoraTermica;
            BuzinaAtivada = buzinaAtivada;
        }
    }

    public static class RetornoImpressoraTermicaHelper
    {
        public static RetornoImpressoraTermica ImprimirTexto(this RetornoImpressoraTermica that, string texto)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.ImprimirTexto(texto);
            return that;
        }

        public static RetornoImpressoraTermica ImprimirTexto(this RetornoImpressoraTermica that, string textoAEsquerda, string textoADireita, eFonte fonte, char charSeparador = ' ')
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.ImprimirTexto(textoAEsquerda, textoADireita, fonte, charSeparador);
            return that;
        }

        public static RetornoImpressoraTermica SelecionarAlinhamento(this RetornoImpressoraTermica that, eAlinhamento alinhamento = eAlinhamento.Padrao)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.SelecionarAlinhamento(alinhamento);
            return that;
        }

        public static RetornoImpressoraTermica SelecionarFonte(this RetornoImpressoraTermica that, eFonte fonte = eFonte.Padrao)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.SelecionarFonte(fonte);
            return that;
        }

        public static RetornoImpressoraTermica ImprimirLinhasEmBranco(this RetornoImpressoraTermica that, int numeroDeLinhas = 1)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.ImprimirLinhasEmBranco(numeroDeLinhas);
            return that;
        }

        public static RetornoImpressoraTermica CortarPapel(this RetornoImpressoraTermica that)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.CortarPapel();
            return that;
        }

        public static eStatusImpressora ObterStatusImpressora(this RetornoImpressoraTermica that)
        {
            return that.ImpressoraTermica.ObterStatusImpressora();
        }

        public static RetornoImpressoraTermica AtivarAltoFalante(this RetornoImpressoraTermica that)
        {
            if (that.BuzinaAtivada && that.Retorno) that.Retorno = that.ImpressoraTermica.AtivarAltoFalante();
            return that;
        }

        public static RetornoImpressoraTermica ExecutarAutoTeste(this RetornoImpressoraTermica that)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.ExecutarAutoTeste();
            return that;
        }

        public static RetornoImpressoraTermica SelecionarConjuntoInternacionalDeCaracteres(this RetornoImpressoraTermica that, eLinguagem linguagem = eLinguagem.AmericaLatina)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.SelecionarConjuntoInternacionalDeCaracteres(linguagem);
            return that;
        }

        public static RetornoImpressoraTermica SelecionarTabelaDeCodigoDeCaracteres(this RetornoImpressoraTermica that, eTabelaDeCaracteres tabelaDeCaracteres = eTabelaDeCaracteres.EUA_PadraoEuropa)
        {
            if (that.Retorno) that.Retorno = that.ImpressoraTermica.SelecionarTabelaDeCodigoDeCaracteres(tabelaDeCaracteres);
            return that;
        }
    }
}