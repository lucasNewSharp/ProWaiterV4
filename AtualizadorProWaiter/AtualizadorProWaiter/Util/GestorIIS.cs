using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSharp.AtualizadorProWaiter.Util
{
    internal class GestorIIS
    {
        public delegate void AoReceberInformacaoHandler(string msg, bool ehErro);
        public event AoReceberInformacaoHandler AoReceberInformacao;

        enum Acao { Parar, Iniciar }

        public void PararIIS()
        {
            Executar(Acao.Parar);
        }

        public void IniciarIIS()
        {
            Executar(Acao.Iniciar);
        }

        private void Executar(Acao acao)
        {
            string comando = string.Empty;
            switch (acao)
            {
                case Acao.Parar:
                    comando = "iisreset /stop";
                    break;
                case Acao.Iniciar:
                    comando = "iisreset /start";
                    break;
            }

            string script = @"/C  " + comando;

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd.exe", script);
            procStartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            procStartInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardInput = true;

            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            var proc = new Process();
            proc.EnableRaisingEvents = true;
            proc.StartInfo = procStartInfo;
            proc.OutputDataReceived += Proc_OutputDataReceived; ;
            proc.ErrorDataReceived += Proc_ErrorDataReceived;
            proc.Exited += Proc_Exited;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit();

            if (!proc.HasExited)
            {
                proc.Kill();
            }

            if (proc != null)
            {
                proc.Dispose();
                proc = null;
            }
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispararEvento(e.Data, false);
        }

        private void Proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            DispararEvento(e.Data, true);
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            DispararEvento("", false);
        }

        private void DispararEvento(string msg, bool ehErro)
        {
            AoReceberInformacao?.Invoke(msg, ehErro);
        }
    }
}
