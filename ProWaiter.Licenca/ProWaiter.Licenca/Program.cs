using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProWaiter.Licenca
{
    static class Program
    {
        private static string guid = "562C8B70-A825-4A8A-B21A-941C5C518277";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, guid))
            {
                if (mutex.WaitOne(0, false))
                {
                    Application.ThreadException += Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrmMain());
                }
                else
                {
                    MessageBox.Show("O ProWaiter - Licencas já está rodando", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }            
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = (e.ExceptionObject as Exception).ToString();
            MessageBox.Show(msg, "ProWaiter Licenca - ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);

            File.AppendAllText(AppContext.BaseDirectory + "erro.txt", msg + "\r\n\r\n");
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string msg = e.Exception.ToString();
            MessageBox.Show(msg, "ProWaiter Licenca - ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            File.AppendAllText(AppContext.BaseDirectory + "erro.txt", msg + "\r\n\r\n");
        }
    }
}
