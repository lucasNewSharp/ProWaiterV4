using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSharp.AtualizadorProWaiter.Util
{
    //Singleton
    public class GestorBancoDeDados
    {
        private static GestorBancoDeDados _instancia = null;
        private static string _stringConexao = string.Empty;
        private static string _versaoAtualProWaiter = string.Empty;

        public static GestorBancoDeDados ObterInstancia()
        {
            if (_instancia == null)
                _instancia = new GestorBancoDeDados();

            return _instancia;
        }

        private GestorBancoDeDados()
        {
            _stringConexao = ConfigurationManager.ConnectionStrings["ProWaiterConnectionString"].ConnectionString;
        }

        public string ObterVersaoAtualProWaiter()
        {
            if (!string.IsNullOrEmpty(_versaoAtualProWaiter))
                return _versaoAtualProWaiter;

            SqlConnection conn = new SqlConnection(_stringConexao);
            string versao = string.Empty;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select valor from TBConfiguracoes where Codigo = 'Versao'", conn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        versao = (string)dr["valor"];
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            _versaoAtualProWaiter = versao;
            return _versaoAtualProWaiter;
        }

        internal void AtualizarBancoDeDados(List<string> versoesAAtualizar)
        {
            Encoding encodingISO = Encoding.GetEncoding("ISO-8859-1");
            string[] splitter = new string[] { "\r\nGO\r\n" };

            SqlConnection conn = new SqlConnection(_stringConexao);
            SqlTransaction trans = null;
            bool setarRollback = false;
            try
            {
                conn.Open();
                trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                foreach (var versao in versoesAAtualizar)
                {
                    string pastaVer = "Versoes\\" + versao + "\\SQLs";
                    FileInfo[] sqls = new DirectoryInfo(pastaVer).GetFiles();
                    foreach (var sql in sqls)
                    {
                        string textoSQL = File.ReadAllText(sql.FullName, encodingISO);

                        //Precisamos separa o script pelos "GO", pois o "GO" é reconecido apenas pelo managment studo e pelo command que tu executa diretamente
                        string[] scripts = textoSQL.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var script in scripts)
                        {
                            SqlCommand cmd = new SqlCommand(script, conn, trans);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                SqlCommand cmdUpdateVersao = new SqlCommand("update TBConfiguracoes set valor = '" + Configuracoes.ObterInstancia().UltimaVersao + "' where codigo = 'Versao'", conn, trans);
                cmdUpdateVersao.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                setarRollback = true;
                throw ex;
            }
            finally
            {
                if (trans != null)
                {
                    if (setarRollback)
                        trans.Rollback();
                    else
                        trans.Commit();
                }
                conn.Close();
            }
        }

        internal void FazerRollBackUltimaVersaoLocal()
        {
            var config = Configuracoes.ObterInstancia();
            //Restauramos o banco de dados
            string pastaBackUp = config.PastaBaseBackup + "\\" + config.VersaoAtualProWaiterNomePasta + @"\BancoDeDados\Backup\ProWaiter.bak";

            string scriptRestore = "USE [master]\n" +
                "ALTER DATABASE[ProWaiter] SET SINGLE_USER WITH ROLLBACK IMMEDIATE\n" +
                "RESTORE DATABASE[ProWaiter] FROM DISK = N'" + pastaBackUp + "' WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5\n" +
                "ALTER DATABASE[ProWaiter] SET MULTI_USER\n" +
                "USE [ProWaiter]\n" +
                "ALTER USER ProWaiterDB with login = ProWaiterDB";

            SqlConnection conn = new SqlConnection(_stringConexao);
            try
            {
                conn.Open();                
                SqlCommand cmd = new SqlCommand(scriptRestore, conn);
                cmd.ExecuteNonQuery();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {                
                conn.Close();
            }
        }
    }
}
