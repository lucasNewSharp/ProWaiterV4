using System;
using System.Net.Sockets;
using System.Text;
using NewSharp.Ferramentas.Impressoras.Termicas;

namespace ProWaiter.Web.Util
{
    public class TcpEscPosPrinter : IImpressoraTermica
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public bool ConectarImpressora(string nomeImpressora)
        {
            return false; // Not used for TCP
        }

        public bool ConectarImpressora(string ip, int porta)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ip, porta);
                _stream = _client.GetStream();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DesconectarImpressora()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void IniciarImpressora()
        {
            WriteBytes(new byte[] { 27, 64 }); // ESC @
        }

        public eStatusImpressora ObterStatusImpressora()
        {
            return eStatusImpressora.Normal; // Mocking normal status since bidirectional TCP is complex
        }

        public bool ImprimirTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return true;
            
            try
            {
                // .NET Core usually requires provider for specific codepages, but ISO-8859-1 is often available
                byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(texto);
                WriteBytes(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SelecionarAlinhamento(eAlinhamento alinhamento = eAlinhamento.Padrao)
        {
            byte val = 0;
            if (alinhamento == eAlinhamento.Centralizado) val = 1;
            else if (alinhamento == eAlinhamento.Direita) val = 2;
            WriteBytes(new byte[] { 27, 97, val }); // ESC a
            return true;
        }

        public bool SelecionarFonte(eFonte fonte = eFonte.Padrao)
        {
            byte val = (byte)fonte;
            WriteBytes(new byte[] { 27, 33, val }); // ESC !
            return true;
        }

        public bool ImprimirLinhasEmBranco(int numeroDeLinhas = 1)
        {
            for (int i = 0; i < numeroDeLinhas; i++)
            {
                WriteBytes(new byte[] { 10 });
            }
            return true;
        }

        public bool CortarPapel()
        {
            WriteBytes(new byte[] { 29, 86, 66, 0 }); // GS V 66 0
            return true;
        }

        public bool AtivarAltoFalante()
        {
            WriteBytes(new byte[] { 27, 66, 3, 2 }); // ESC B 3 2 (Bematech/Elgin Beep)
            return true;
        }

        public bool ExecutarAutoTeste()
        {
            return true;
        }

        public bool SelecionarConjuntoInternacionalDeCaracteres(eLinguagem linguagem = eLinguagem.AmericaLatina)
        {
            return true;
        }

        public bool SelecionarTabelaDeCodigoDeCaracteres(eTabelaDeCaracteres tabelaDeCaracteres = eTabelaDeCaracteres.EUA_PadraoEuropa)
        {
            return true;
        }

        public bool ImprimirTexto(string textoAEsquerda, string textoADireita, eFonte fonte, char charSeparador = ' ')
        {
            int maxChars = ObterNumCaracteresDaLinha(fonte);
            int lenEsq = textoAEsquerda?.Length ?? 0;
            int lenDir = textoADireita?.Length ?? 0;

            if (lenEsq + lenDir > maxChars)
            {
                if (lenDir < maxChars)
                {
                    textoAEsquerda = textoAEsquerda.Substring(0, maxChars - lenDir - 1);
                }
            }

            int numSeparators = maxChars - (textoAEsquerda?.Length ?? 0) - (textoADireita?.Length ?? 0);
            if (numSeparators < 0) numSeparators = 0;

            string final = (textoAEsquerda ?? "") + new string(charSeparador, numSeparators) + (textoADireita ?? "");
            return ImprimirTexto(final);
        }

        public int ObterNumCaracteresDaLinha(eFonte fonte)
        {
            if (fonte.HasFlag(eFonte.LarguraDupla))
            {
                return 24;
            }
            return 48; // Standard 80mm
        }

        private void WriteBytes(byte[] data)
        {
            if (_stream != null && _stream.CanWrite)
            {
                _stream.Write(data, 0, data.Length);
            }
        }
    }
}
