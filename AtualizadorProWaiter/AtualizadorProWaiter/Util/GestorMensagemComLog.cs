using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewSharp.AtualizadorProWaiter.Util
{
    public enum eTipoMensagem { Info, Warning, Erro }

    public static class GestorMensagensComLog
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void ExibirMensagem(string msg, string titulo, eTipoMensagem tipoMensagem)
        {
            ExibirMensagemComLogHelper(msg, titulo, tipoMensagem, true, false, null);
        }

        public static void GravarLog(string msg)
        {
            ExibirMensagemComLogHelper(msg, null, eTipoMensagem.Info, false, true, null);
        }

        public static void GravarLog(string msg, eTipoMensagem tipoMensagem, Exception ex = null)
        {
            ExibirMensagemComLogHelper(msg, null, tipoMensagem, false, true, ex);
        }

        public static void ExibirMensagemComLog(string msg, string titulo, eTipoMensagem tipoMensagem, Exception ex)
        {
            ExibirMensagemComLogHelper(msg, titulo, tipoMensagem, true, true, ex);
        }

        private static void ExibirMensagemComLogHelper(string msg, string titulo, eTipoMensagem tipoMensagem, bool exibirMensagem, bool gravarLog, Exception ex)
        {
            switch (tipoMensagem)
            {
                case eTipoMensagem.Info:
                    if (exibirMensagem)
                        MessageBox.Show(msg, titulo, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (gravarLog)
                        _logger.Info(msg);
                    break;
                case eTipoMensagem.Warning:
                    if (exibirMensagem)
                        MessageBox.Show(msg, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (gravarLog)
                        _logger.Warn(msg);
                    break;
                case eTipoMensagem.Erro:
                    if (exibirMensagem)
                        MessageBox.Show(msg, titulo, MessageBoxButton.OK, MessageBoxImage.Error);
                    if (gravarLog)
                        _logger.Error(ex, msg);
                    break;
            }
        }
    }
}
