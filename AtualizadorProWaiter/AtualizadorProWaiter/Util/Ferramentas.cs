using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSharp.AtualizadorProWaiter.Util
{
    public class Ferramentas
    {
        public delegate void AoCopiarArquivo(string msg);

        public static bool PrecisaAtualizar()
        {
            string versaoAtual = GestorBancoDeDados.ObterInstancia().ObterVersaoAtualProWaiter();
            string ultimaVersao = Configuracoes.ObterInstancia().UltimaVersao;
            return versaoAtual != ultimaVersao;
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            CopyAll(source, target, null, false);
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?redirectedfrom=MSDN&view=netframework-4.8
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, AoCopiarArquivo callbackMsg, bool ignorarAppData)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (ignorarAppData && fi.Name == "App_Data")
                    continue;

                callbackMsg?.Invoke(string.Format(@"Copiando {0}\{1}", target.FullName, fi.Name));
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, callbackMsg, ignorarAppData);
            }
        }
    }
}
