using Instalador.Util;
using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Instalador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _threadInstalacao;
        private Process _procIIS;
        private Process _procSQLServer;
        private Process _procConfSQL;
        private Process _procAgendamentoBackup;
        private bool _erroInstalacao;
        private bool _processoEmAndamento;
        private bool _cancelarInstalacao;
        private bool _finalizou;
        private bool _reinicializacaoAceita;

        private const string _pastaBackup = "C:\\NewSharp\\BancoDeDados\\Backup";
        public const string ArquivoSinalizadorInstalacaoSetup = "ContinuarInstalacao.txt";

        public static bool IgnorarExcessoes { get; private set; }


        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Title = "ProWaiter 2.2";
        }

        private void ucTermosELicenca_AoClicarAceito(bool valor)
        {
            btnAvancar.IsEnabled = valor;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBoxResult.Yes;

                if (!_reinicializacaoAceita && !_erroInstalacao)
                {
                    result = MessageBox.Show("Tem certeza que deseja sair da instalação?", "Instalação do ProWaiter", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                if (result == MessageBoxResult.No)
                    e.Cancel = true;
                else
                {
                    GestorMensagensComLog.GravarLog("Instalador encerrado");
                    IgnorarExcessoes = true;
                    if (_threadInstalacao != null)
                    {
                        GestorMensagensComLog.GravarLog("Encerrando thread de instalação");
                        if (_threadInstalacao.IsAlive)
                        {
                            _threadInstalacao.Abort();
                            _threadInstalacao = null;
                        }
                    }
                    if (_procIIS != null && !_procIIS.HasExited)
                    {
                        GestorMensagensComLog.GravarLog("Encerrando processo de instalação do IIS");
                        _procIIS.Kill();
                    }
                    if (_procSQLServer != null && !_procSQLServer.HasExited)
                    {
                        GestorMensagensComLog.GravarLog("Encerrando processo de instalação SQL Server");
                        _procSQLServer.Kill();
                    }
                    if (_procConfSQL != null && !_procConfSQL.HasExited)
                    {
                        GestorMensagensComLog.GravarLog("Encerrando processo de configuração do SQL server");
                        _procConfSQL.Kill();
                    }
                    if (_procAgendamentoBackup != null && !_procAgendamentoBackup.HasExited)
                    {
                        GestorMensagensComLog.GravarLog("Encerrando processo configuração do agendamento de tarefas");
                        _procAgendamentoBackup.Kill();
                    }
                    GestorMensagensComLog.GravarLog("Instalador encerrado");
                }
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Erro no encerramento do instalador", "Erro", eTipoMensagem.Erro, ex);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (_processoEmAndamento)
            {
                MessageBoxResult res = MessageBox.Show("Tem certeza que deseja parar a instalação?", "Cancelamento de instalação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                {
                    GestorMensagensComLog.GravarLog("Processo de instalação encerrado pelo usuário", eTipoMensagem.Warning);
                    _cancelarInstalacao = true;
                    btnCancelar.IsEnabled = false;
                    SetarTituloInstalacao("Aguardando término da operação atual, aguarde....");
                }
            }
            else
            {
                Close();
            }
        }

        private void btnAvancar_Click(object sender, RoutedEventArgs e)
        {
            if (_finalizou)
            {
                string msg = "É necessaria a reinicialização do sistema, após a reinicialização o sistema irá continuar a instalação, deseja reiniciar agora?";
                MessageBoxResult result = MessageBox.Show(msg, "Reinicialização do sistema", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    GestorMensagensComLog.GravarLog("Usuário reiniciou o sistema após a preparação do ambiente");
                    _reinicializacaoAceita = true;
                    Process.Start("shutdown.exe", "-r -t 0");
                }
                else
                {
                    GestorMensagensComLog.GravarLog("Usuário NÃO reiniciou o sistema após a preparação do ambiente", eTipoMensagem.Warning);
                }
                Close();
            }
            else
            {
                GestorMensagensComLog.GravarLog("INSTALAÇÃO INICIADA");
                ucTermosELicenca.Visibility = Visibility.Hidden;
                DestacarLabel(lblPreRequisitos);
                SetarTituloInstalacao("Verificando pré-requisitos");
                ucInstalacao.Visibility = Visibility.Visible;
                btnAvancar.IsEnabled = false;
                _threadInstalacao = new Thread(new ThreadStart(InstalarProWaiter));
                _threadInstalacao.Start();
            }
        }

        private void DestacarLabel(Label lbl)
        {
            Dispatcher.Invoke(() =>
            {
                lblComponentesDoWindows.FontWeight =
                    lblSqlServer.FontWeight =
                    lblPreRequisitos.FontWeight =
                    lblConfiguracaoSQL.FontWeight =
                    lblTermosLicenca.FontWeight = FontWeights.Normal;

                lbl.FontWeight = FontWeights.Bold;
            });
        }

        private void SetarTituloInstalacao(string titulo)
        {
            Dispatcher.Invoke(() =>
            {
                GestorMensagensComLog.GravarLog(titulo);
                ucInstalacao.txtTitulo.Text = titulo;
            });
        }

        private void ConcatenarTextoInstalacao(string texto)
        {
            Dispatcher.Invoke(() =>
            {
                GestorMensagensComLog.GravarLog(texto);
                ucInstalacao.txtTexto.Text += texto;
                ucInstalacao.txtTexto.ScrollToEnd();
            });
        }

        private void InstalarProWaiter()
        {
            _processoEmAndamento = true;
            SetarTituloInstalacao("Verificando pré-requisitos");

            if (!ValidacaoPreRequisitos())
            {
                Dispatcher.Invoke(() => { ucInstalacao.progressBar.Visibility = Visibility.Hidden; });
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }
            }
            else
            {
                InstalarIIS();
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }
                InstalarBancoDeDados();
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }
                ConfigurarBancoDeDados();
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }
                CriarAgendamentosBackup();
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }
                _processoEmAndamento = false;
                CriarInicializacaoParaSetupProWaiter();
                if (_erroInstalacao || _cancelarInstalacao)
                {
                    _processoEmAndamento = false;
                    Dispatcher.Invoke(() => { Close(); });
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    ucInstalacao.Visibility = Visibility.Hidden;
                    ucFinalizar.Visibility = Visibility.Visible;
                    btnAvancar.Content = "Finalizar";
                    btnAvancar.IsEnabled = true;
                    btnCancelar.IsEnabled = false;
                    _finalizou = true;
                });
            }
        }

        #region PreRequisitos

        private bool ValidacaoPreRequisitos()
        {
            try
            {
                bool ok = true;
                ComputerInfo ci = new ComputerInfo();

                //Versão do windows
                if (Environment.OSVersion.Version.Major < 10)
                {
                    ConcatenarTextoInstalacao("A versão do windows não é compativel com o ProWaiter - Falha\n");
                    ok = false;
                }
                else
                {
                    ConcatenarTextoInstalacao($"Versão do windows {Environment.OSVersion.Version.Major} - OK\n");
                }

                //Arquitetura do windows
                if (!Environment.Is64BitOperatingSystem)
                {
                    ConcatenarTextoInstalacao("A arquitetura do windows deve ser x64 - Falha\n");
                    ok = false;
                }
                else
                {
                    ConcatenarTextoInstalacao($"Arquitetura do windows x64 - OK\n");
                }

                //Espaço em disco            
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.Name == "C:\\")
                    {
                        if (drive.TotalFreeSpace < 10737418240) //10GB
                        {
                            ConcatenarTextoInstalacao("A unidade C deve ter pelo menos 10GB de espaço livre - Falha\n");
                            ok = false;
                        }
                        else
                        {
                            ConcatenarTextoInstalacao($"Espaço disponível {(((drive.TotalFreeSpace / 1024) / 1024) / 1024)} GB - OK\n");
                        }
                    }
                }

                if (ci.TotalPhysicalMemory < 7516192768) //7GB
                {
                    ConcatenarTextoInstalacao("Memória RAM insuficiente, mínimo 8GB requerido - Falha\n");
                    ok = false;
                }
                else
                {
                    ConcatenarTextoInstalacao($"Memória disponível {(((ci.TotalPhysicalMemory / 1024) / 1024) / 1024)} - OK\n");
                }

                return ok;
            }
            catch (Exception ex)
            {
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar avaliar os pré-requisitos", "Erro", eTipoMensagem.Erro, ex);
                _erroInstalacao = true;
                return false;
            }
        }

        #endregion

        #region IIS

        private void InstalarIIS()
        {
            try
            {
                DestacarLabel(lblComponentesDoWindows);
                SetarTituloInstalacao("Instalando o IIS");
                _procIIS = GestorExecutaScript.ExecutaScript("IIS", Directory.GetCurrentDirectory() + "\\IIS", ProgressoInstalacaoIIS, ErroInstalacaoIIS, FinalizacaoInstalacaoIIS);
            }
            catch (Exception ex)
            {
                _erroInstalacao = true;
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar instlar IIS", "ERRO", eTipoMensagem.Erro, ex);
            }
        }

        private void ProgressoInstalacaoIIS(object sender, DataReceivedEventArgs e)
        {
            string txt = e.Data;

            if (!string.IsNullOrWhiteSpace(txt))
            {
                if (txt.Contains("[") || txt.Contains("]"))
                {
                    txt = txt.Replace("[", string.Empty).Replace("]", string.Empty).Replace("=", string.Empty).Trim() + "...";
                }
                else
                {
                    txt = "\n\n" + txt;
                }

                ConcatenarTextoInstalacao(txt);
            }
        }

        private void ErroInstalacaoIIS(object sender, DataReceivedEventArgs e)
        {
            _erroInstalacao = true;
            ConcatenarTextoInstalacao(e.Data + "\n");
        }

        private void FinalizacaoInstalacaoIIS(object sender, EventArgs e)
        {
            if (_erroInstalacao)
                ConcatenarTextoInstalacao("\n\nERRO NA INSTLAÇÃO");
            else
            {
                ConcatenarTextoInstalacao("\n\nInstalação do IIS concluída com SUCESSO");
            }
        }

        #endregion

        #region Banco de Dados

        private void InstalarBancoDeDados()
        {
            try
            {
                DestacarLabel(lblSqlServer);
                SetarTituloInstalacao("Instalando o SQL Server");
                _procSQLServer = GestorExecutaScript.ExecutaScript("SqlServer", Directory.GetCurrentDirectory() + "\\SQLEXPR_x64_PTB", ProgressoInstalacaoSqlServer, ErroInstalacaoSqlServer, FinalizacaoInstalacaoSqlServer);
            }
            catch (Exception ex)
            {
                _erroInstalacao = true;
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar instalar o SQL server", "Erro", eTipoMensagem.Erro, ex);
            }
        }

        private void ProgressoInstalacaoSqlServer(object sender, DataReceivedEventArgs e)
        {
            string txt = e.Data;

            if (!string.IsNullOrWhiteSpace(txt))
            {
                ConcatenarTextoInstalacao("\n" + txt);
            }
        }


        private void ErroInstalacaoSqlServer(object sender, DataReceivedEventArgs e)
        {
            _erroInstalacao = true;
            ConcatenarTextoInstalacao("\n" + e.Data);
        }

        private void FinalizacaoInstalacaoSqlServer(object sender, EventArgs e)
        {
            if (_erroInstalacao)
                ConcatenarTextoInstalacao("\n\nERRO NA INSTLAÇÃO");
            else
            {
                ConcatenarTextoInstalacao("\n\nInstalação do SqlServer concluída com SUCESSO");
            }
        }

        #endregion

        #region Configurar Banco de Dados

        private void ConfigurarBancoDeDados()
        {
            try
            {
                SetarTituloInstalacao("Configurando banco de dados");

                //Aguardar o SQL Server iniciar
                ServiceController[] serviceControllers;
                serviceControllers = ServiceController.GetServices("localhost");
                foreach (ServiceController serviceController in serviceControllers)
                {
                    if (serviceController.DisplayName.Contains("SQL Server") && serviceController.ServiceName == "MSSQL$SQLEXPRESS")
                    {
                        while (serviceController.Status != ServiceControllerStatus.Running)
                        {
                            ConcatenarTextoInstalacao("\nAguardando SQL Server..." + Enum.GetName(typeof(ServiceControllerStatus), serviceController.Status).ToString());
                            Thread.Sleep(1000);
                        }
                    }
                }

                DestacarLabel(lblConfiguracaoSQL);
                try
                {
                    Directory.CreateDirectory(_pastaBackup);
                    ConcatenarTextoInstalacao("\nPasta C:\\NewSharp Criada");
                }
                catch (Exception ex)
                {
                    _erroInstalacao = true;
                    GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar criar a pasta c:\\Newsharp", "Erro", eTipoMensagem.Erro, ex);
                    return;
                }

                try
                {
                    //Cópia do backup para o destino
                    Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "ConfigSQL"))
                        .ToList()
                        .ForEach(f => File.Copy(f, Path.Combine(_pastaBackup, System.IO.Path.GetFileName(f)), true));
                    ConcatenarTextoInstalacao("\nArquivos copiados...");
                }
                catch (Exception ex)
                {
                    _erroInstalacao = true;
                    GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar copiar os arquivos necessários para a pasta c:\\NewSharp", "Erro", eTipoMensagem.Erro, ex);
                    return;
                }

                ConcatenarTextoInstalacao("\nExecutando restore e configuração do banco de dados ProWaiter...");

                try
                {
                    _procConfSQL = GestorExecutaScript.ExecutaScript("ExecutarSQLRestauracao", _pastaBackup, "RestaurarBanco.sql", ProgressoConfigSQL, ErroConfigSQL, FimConfigSQL);
                }
                catch (Exception ex)
                {
                    _erroInstalacao = true;
                    GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar restaurar o banco de dados", "Erro", eTipoMensagem.Erro, ex);
                    return;
                }

                try
                {
                    File.Delete(Path.Combine(_pastaBackup, "ProWaiter.bkp"));
                    File.Delete(Path.Combine(_pastaBackup, "ExecutarSQLRestauracao.bat"));
                    File.Delete(Path.Combine(_pastaBackup, "RestaurarBanco.sql"));
                }
                catch (Exception exDelecao)
                {
                    GestorMensagensComLog.GravarLog("ERRO ao tentar remover arquivos temporários", eTipoMensagem.Warning, exDelecao);
                }
            }
            catch (Exception ex)
            {
                _erroInstalacao = true;
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar configurar o SQL server", "Erro", eTipoMensagem.Erro, ex);
            }
        }

        private void ProgressoConfigSQL(object sender, DataReceivedEventArgs e)
        {
            string txt = e.Data;

            if (!string.IsNullOrWhiteSpace(txt))
            {
                ConcatenarTextoInstalacao("\n" + txt);
            }
        }

        private void ErroConfigSQL(object sender, DataReceivedEventArgs e)
        {
            _erroInstalacao = true;
            ConcatenarTextoInstalacao("\n" + e.Data);
        }

        private void FimConfigSQL(object sender, EventArgs e)
        {
            if (_erroInstalacao)
                ConcatenarTextoInstalacao("\n\nERRO NA INSTLAÇÃO");
            else
            {
                ConcatenarTextoInstalacao("\n\nConfiguração do Banco de dados concluída com SUCESSO");
            }
        }

        #endregion

        #region Agendamentos backup

        private void CriarAgendamentosBackup()
        {
            ConcatenarTextoInstalacao("\n\nCriando agendamentos de backup");
            try
            {
                _procAgendamentoBackup = GestorExecutaScript.ExecutaScript("CriarTasks", _pastaBackup, ProgressoCriacaoAgendamentoBackup, ErroCriacaoAgendamentoBackup, FimCriacaoAgendamentoBackup);
            }
            catch (Exception ex)
            {
                _erroInstalacao = true;
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar criar agendamentos de backup", "Erro", eTipoMensagem.Erro, ex);
            }
        }

        private void ProgressoCriacaoAgendamentoBackup(object sender, DataReceivedEventArgs e)
        {
            string txt = e.Data;

            if (!string.IsNullOrWhiteSpace(txt))
            {
                ConcatenarTextoInstalacao("\n" + txt);
            }
        }

        private void ErroCriacaoAgendamentoBackup(object sender, DataReceivedEventArgs e)
        {
            _erroInstalacao = true;
            ConcatenarTextoInstalacao("\n" + e.Data);
        }

        private void FimCriacaoAgendamentoBackup(object sender, EventArgs e)
        {
            if (_erroInstalacao)
                ConcatenarTextoInstalacao("\n\nERRO NA INSTLAÇÃO");
            else
            {
                ConcatenarTextoInstalacao("\n\nAgendamentos criados com sucesso");
            }
        }

        #endregion

        #region Agendar instalação setup prowaiter depois de reiniciar

        //Criar Inicialização para o ProWaiter
        private void CriarInicializacaoParaSetupProWaiter()
        {
            try
            {
                string arqScript = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), "IniciarInstaladorProWaiter.bat");
                string comando = "start \"\" \"" + Directory.GetCurrentDirectory() + "\\SetupProWaiter\\setup.exe\"\ndel %0";
                File.WriteAllText(arqScript, comando);
            }
            catch (Exception ex)
            {
                _erroInstalacao = true;
                GestorMensagensComLog.ExibirMensagemComLog("Erro ao tentar configurar a inicialização do setup apos a reinicialização", "Erro", eTipoMensagem.Erro, ex);
                return;
            }
        }

        #endregion
    }
}
