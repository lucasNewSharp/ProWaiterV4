using NewSharp.AtualizadorProWaiter.Gestores;
using NewSharp.AtualizadorProWaiter.Gestores.Backup;
using NewSharp.AtualizadorProWaiter.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AtualizadorProWaiter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _threadInstalacao;
        private GestorBackupVersaoLocal _gBackupVersaoLocal = new GestorBackupVersaoLocal();
        private GestorIIS _gIIS = new GestorIIS();
        private GestorAtualizacao _gAtualizacao = null;        
        private bool _iniciou = false;
        private bool _finalizou = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            lblTituloAtualizador.Content += " para a versão " + Configuracoes.ObterInstancia().UltimaVersao;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_finalizou && _iniciou)
            {
                GestorMensagensComLog.ExibirMensagem("Aguarde a instalação finalizar...", "Aviso", eTipoMensagem.Warning);
                e.Cancel = true;
            }
            else
            {
                if (_threadInstalacao != null)
                {
                    GestorMensagensComLog.GravarLog("Encerrando thread de atualização");
                    if (_threadInstalacao.IsAlive)
                    {
                        _threadInstalacao.Abort();
                        _threadInstalacao = null;
                    }
                }
            }
        }

        private void btnAvancar_Click(object sender, RoutedEventArgs e)
        {
            if (_finalizou)
            {
                Close();
            }
            else
            {
                _iniciou = true;
                ucNotasDaVersao.Visibility = Visibility.Hidden;
                ucAtualizacao.Visibility = Visibility.Visible;
                btnAvancar.IsEnabled = false;
                _threadInstalacao = new Thread(new ThreadStart(AtualizarProWaiter));
                _threadInstalacao.Start();
            }
        }

        private void _g_AoReceberInformacao(string msg, bool ehErro)
        {
            if (ehErro)
            {
                throw new ApplicationException(msg);
            }

            AdicionarTextoJanelaAtualizacao(msg);

        }

        private void AdicionarTextoJanelaAtualizacao(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                GestorMensagensComLog.GravarLog(msg);
                ucAtualizacao.txtTexto.Text += msg + "\n";
                ucAtualizacao.txtTexto.ScrollToEnd();
            });
        }

        private void AlterarTituloJanelaAtualizacao(string titulo)
        {
            Dispatcher.Invoke(() =>
            {
                GestorMensagensComLog.GravarLog(titulo + "\n");
                ucAtualizacao.txtTitulo.Text = titulo;
            });
        }


        private void DestacarLabel(Label lbl)
        {
            Dispatcher.Invoke(() =>
            {
                lblNovaVersao.FontWeight =
                    lblBackupVersaoLocal.FontWeight =
                    lblAtualizandoSistema.FontWeight =
                    lblFim.FontWeight = FontWeights.Normal;

                lbl.FontWeight = FontWeights.Bold;
            });
        }


        private void AtualizarProWaiter()
        {
            //Verificamos se é necessário atualizar
            AlterarTituloJanelaAtualizacao("Verificando versão atual...");

            if (!Ferramentas.PrecisaAtualizar())
            {
                GestorMensagensComLog.ExibirMensagemComLog("O ProWaiter já está na última versão", "Aviso", eTipoMensagem.Warning, null);
                _finalizou = true;
                Dispatcher.Invoke(() => { Close(); });
                return;
            }

            //Paramos o IIS
            try
            {
                AlterarTituloJanelaAtualizacao("Parando o IIS...");
                _gIIS.AoReceberInformacao += _g_AoReceberInformacao;
                _gIIS.PararIIS();
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar para o IIS", "Erro", eTipoMensagem.Erro, ex);
                _finalizou = true;
                Dispatcher.Invoke(() => { Close(); });
                return;
            }

            //Criamos o backup da instalação local
            DestacarLabel(lblBackupVersaoLocal);
            try
            {
                GestorMensagensComLog.GravarLog("Iniciando o bakcup do banco de dados...");
                AlterarTituloJanelaAtualizacao("Realizando backup...");
                _gBackupVersaoLocal.AoReceberInformacao += _g_AoReceberInformacao;
                _gBackupVersaoLocal.ExecutarBackup();
                GestorMensagensComLog.GravarLog("Backup do banco concluído com sucesso.");
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar efetuar o backup do sistema", "Erro", eTipoMensagem.Erro, ex);
                _finalizou = true;
                Dispatcher.Invoke(() => { Close(); });
                return;
            }

            //Realizamos a atualizacao
            bool ok = true;
            try
            {
                AlterarTituloJanelaAtualizacao("Atualizando o sistema...");
                DestacarLabel(lblAtualizandoSistema);
                _gAtualizacao = new GestorAtualizacao();
                _gAtualizacao.AoReceberInformacao += _g_AoReceberInformacao;
                _gAtualizacao.Atualizar();                
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar atualizar o sistema, clique OK para restaurar a ultima versão.", "Erro", eTipoMensagem.Erro, ex);

                AlterarTituloJanelaAtualizacao("Efetuando rollback...");
                try
                {
                    _gAtualizacao.EfetuarRollback();
                    GestorMensagensComLog.ExibirMensagem("Rollback efetuado, verifique os erros na pasta \"Logs\" que encontra-se dentro da pasta do atualizador", "Aviso", eTipoMensagem.Warning);
                }
                catch(Exception e)
                {
                    GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar efetuar rollback, entre em contato com o administrador do sistema", "ERRO CRITICO", eTipoMensagem.Erro, e);                    
                }
                finally
                {
                    ok = false;
                }
            }
            
            //Iniciando o IIS
            try
            {
                AdicionarTextoJanelaAtualizacao("Iniciando o IIS...");
                _gIIS.IniciarIIS();
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Não foi possível reiniciar o IIS reinicie o computador", "Erro", eTipoMensagem.Warning, ex);
                _finalizou = true;
                Dispatcher.Invoke(() => { Close(); });
                return;
            }

            DestacarLabel(lblFim);
            AlterarTituloJanelaAtualizacao("Fim");

            if(ok)
                AdicionarTextoJanelaAtualizacao("Atualização encerrada com sucesso!");
            else
                AdicionarTextoJanelaAtualizacao("ATUALIZAÇÃO NÃO EFETUADA");

            _finalizou = true;
            Dispatcher.Invoke(() =>
            {
                btnAvancar.Content = "Fechar";
                btnAvancar.IsEnabled = true;
                ucAtualizacao.progressBar.IsIndeterminate = false;
            });
        }

    }
}
