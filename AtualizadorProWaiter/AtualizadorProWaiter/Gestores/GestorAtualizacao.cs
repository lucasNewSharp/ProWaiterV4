using NewSharp.AtualizadorProWaiter.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSharp.AtualizadorProWaiter.Gestores
{
    internal class GestorAtualizacao
    {
        public delegate void AoReceberInformacaoHandler(string msg, bool ehErro);
        public event AoReceberInformacaoHandler AoReceberInformacao;

        private List<DirectoryInfo> _versoes = new List<DirectoryInfo>();
        private List<string> _versoesDisponiveisAtualizacao = new List<string>();
        private List<string> _versoesAAtualizar = new List<string>();
        private string _versaoAtual = string.Empty;

        public GestorAtualizacao()
        {
            _versaoAtual = GestorBancoDeDados.ObterInstancia().ObterVersaoAtualProWaiter();
            _versoesDisponiveisAtualizacao = ConfigurationManager.AppSettings["VersoesParaAtualizacao"].Split(new char[] { ';' }).ToList();
        }

        public void Atualizar()
        {
            try
            {
                Configuracoes config = Configuracoes.ObterInstancia();

                DispararEvento("Atualizando tabelas do banco de dados...", false);
                if (_versaoAtual == "1_0")
                {
                    _versoesAAtualizar = _versoesDisponiveisAtualizacao;
                }
                else
                {
                    int indice = _versoesDisponiveisAtualizacao.IndexOf(_versaoAtual.Replace(".", "_")) + 1;
                    for (int i = indice; i < _versoesDisponiveisAtualizacao.Count; i++)
                    {
                        _versoesAAtualizar.Add(_versoesDisponiveisAtualizacao[i]);
                    }
                }

                //Atualização das tabelas de banco de dados
                GestorBancoDeDados.ObterInstancia().AtualizarBancoDeDados(_versoesAAtualizar);
                DispararEvento("Banco de dados atualizado", false);

                //Remoção de arquivos antigos do sistema
                DispararEvento("Iniciando atualização do sistema", false);                
                DeletarArquivosPastaProWaiter();

                //Copia de novos arquivos do sistema
                DispararEvento("Copiando novos arquivos do sistema...", false);
                string pastaUltimaVersao = "Versoes\\" + config.UlitmaVersaoNomePasta;
                string zipUltimaVersao = pastaUltimaVersao + "\\ProWaiter.zip";
                string destinoUltimaVersao = config.PastaProWaiter + "\\ProWaiter.zip";
                File.Copy(zipUltimaVersao, destinoUltimaVersao);
                ZipFile.ExtractToDirectory(destinoUltimaVersao, config.PastaProWaiter);
                Ferramentas.CopyAll(new DirectoryInfo(config.PastaProWaiter + "\\ProWaiter\\"), new DirectoryInfo(config.PastaProWaiter), null, true);
                File.Delete(destinoUltimaVersao);
                Directory.Delete(config.PastaProWaiter + "\\ProWaiter", true);
                DispararEvento("Sistema atualizado", false);
            }
            catch (Exception ex)
            {
                DispararEvento(ex.ToString(), true);
            }
        }

        public void EfetuarRollback()
        {
            var config = Configuracoes.ObterInstancia();
            var gBanco = GestorBancoDeDados.ObterInstancia();

            DispararEvento("Restaurando a base de dados...", false);
            gBanco.FazerRollBackUltimaVersaoLocal();
            DispararEvento("Base de dados restaurada", false);
            
            DispararEvento("Restaurando arquivos do sistema", false);
            string pastaBackup = config.PastaBaseBackup + "\\" + config.VersaoAtualProWaiterNomePasta + @"\ProWaiter\";            
            DeletarArquivosPastaProWaiter();
            Ferramentas.CopyAll(new DirectoryInfo(pastaBackup), new DirectoryInfo(config.PastaProWaiter), null, true);
            DispararEvento("Sistema restaurado", false);
        }

        private static void DeletarArquivosPastaProWaiter()
        {
            DirectoryInfo directoryInfoPW = new DirectoryInfo(Configuracoes.ObterInstancia().PastaProWaiter);
            FileInfo[] arquivos = directoryInfoPW.GetFiles();
            foreach (var arq in arquivos)
            {
                if (arq.Name == "ProWaiter.ico")
                    continue;
                File.Delete(arq.FullName);
            }

            foreach (DirectoryInfo dir in directoryInfoPW.GetDirectories())
            {
                if (dir.Name == "App_Data")
                    continue;
                Directory.Delete(dir.FullName, true);
            }
        }

        private void DispararEvento(string msg, bool ehErro)
        {
            AoReceberInformacao?.Invoke(msg, ehErro);
        }
    }
}
