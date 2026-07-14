using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSharp.AtualizadorProWaiter.Util
{
    public class Configuracoes
    {
        public string UlitmaVersaoNomePasta { get; private set; }
        public string UltimaVersao { get; private set; }
        public List<string> VersoesParaAtualizacao { get; private set; }

        public string VersaoAtualProWaiterNomePasta { get; private set; }
        public string VersaoAtualProWaiter { get; private set; }

        public string PastaProWaiter { get; private set; }
        public string PastaBaseBackup { get; private set; }

        private static Configuracoes _instancia = null;

        public static Configuracoes ObterInstancia()
        {
            if (_instancia == null)
                _instancia = new Configuracoes();
            return _instancia;
        }

        private Configuracoes()
        {
            string versoesDisp = ConfigurationManager.AppSettings["VersoesParaAtualizacao"];
            VersoesParaAtualizacao = versoesDisp.Split(';').ToList();

            UlitmaVersaoNomePasta = VersoesParaAtualizacao.Last();
            UltimaVersao = UlitmaVersaoNomePasta.Replace("_", ".");

            PastaProWaiter = @"C:\inetpub\wwwroot\ProWaiter";

            if (!Directory.Exists(PastaProWaiter))
                throw new ApplicationException("A pasta do sistema não existe");

            VersaoAtualProWaiter = GestorBancoDeDados.ObterInstancia().ObterVersaoAtualProWaiter();
            VersaoAtualProWaiterNomePasta = VersaoAtualProWaiter.Replace(".", "_");

            PastaBaseBackup = @"C:\NewSharp\VersoesAnteriores";
        }
    }
}
