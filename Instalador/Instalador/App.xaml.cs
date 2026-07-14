using Instalador.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Instalador
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string guid = "9C6D4692-464C-4ADD-A88A-FD995C7E7634";

        [STAThread]
        public static void Main()
        {
            using (Mutex mutex = new Mutex(false, guid))
            {                
                if (mutex.WaitOne(0, false))
                {
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    var application = new App();
                    application.InitializeComponent();
                    application.Run();
                }
                else
                {
                    GestorMensagensComLog.ExibirMensagem("O instalador já está rodando", string.Empty, eTipoMensagem.Warning);
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Instalador.MainWindow.IgnorarExcessoes)
            {
                Exception ex = (e.ExceptionObject as Exception);
                GestorMensagensComLog.ExibirMensagemComLog("Erro crítico\n\n" + ex.ToString(), "ERRO CRITICO", eTipoMensagem.Erro, ex);
            }
        }
    }
}
