using NewSharp.AtualizadorProWaiter.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AtualizadorProWaiter
{
    public partial class App : Application
    {
        private static string guid = "37427626-555B-4DDF-B46C-75F749B48E31";

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
                    GestorMensagensComLog.ExibirMensagem("O atualizador já está rodando", string.Empty, eTipoMensagem.Warning);
                }
            }
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (e.ExceptionObject as Exception);
            GestorMensagensComLog.ExibirMensagemComLog("Erro crítico\n\n" + ex.ToString(), "ERRO CRITICO", eTipoMensagem.Erro, ex);

        }
    }
}
