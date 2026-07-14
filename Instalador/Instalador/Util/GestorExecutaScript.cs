using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instalador.Util
{
    public class GestorExecutaScript
    {
        public static Process ExecutaScript(string nomeScript, string pastaScript, DataReceivedEventHandler callBackProgresso, DataReceivedEventHandler callBackErro, EventHandler callBackExited)
        {
            return ExecutaScript(nomeScript, pastaScript, string.Empty, callBackProgresso, callBackErro, callBackExited);
        }

        public static Process ExecutaScript(string nomeScript, string pastaScript, string argumentos, DataReceivedEventHandler callBackProgresso, DataReceivedEventHandler callBackErro, EventHandler callBackExited)
        {
            string args = @"/C " + nomeScript + ".bat";
            if (!string.IsNullOrWhiteSpace(argumentos))
                args += " " + argumentos;

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd.exe", args);            
            procStartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            procStartInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            procStartInfo.WorkingDirectory = pastaScript;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.RedirectStandardInput = true;
            
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            var proc = new Process();
            proc.EnableRaisingEvents = true;
            proc.StartInfo = procStartInfo;            
            proc.OutputDataReceived += callBackProgresso;
            proc.ErrorDataReceived += callBackErro;
            proc.Exited += callBackExited;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit();
            return proc;
        }
    }
}
