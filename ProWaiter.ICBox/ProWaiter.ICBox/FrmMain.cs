using IcBoxClassLibrary;
using ProWaiter.ICBox.Model;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ProWaiter.ICBox
{
    public partial class FrmMain : Form
    {
        private IcBox _icBox = new IcBox();
        private NotifyIcon _notifyIcon;
        private bool _fechar = false;
        private Thread _thread;
        private bool _inicializando = true;
        private bool _interromperLeituraCOM = false;
        private GestorConfiguracoes _gConfig;

        public FrmMain()
        {
            _gConfig = new GestorConfiguracoes();
            ConfigurarNotifyIcon();
            InitializeComponent();
            AtualizarPortas();
            SelecionarPortaConfigurada();
            _icBox.initialize("");

            _thread = new Thread(new ThreadStart(Monitorar));
            _thread.IsBackground = true;
            _thread.Start();

            Conectar();
            _inicializando = false;
        }

        private void AtualizarPortas()
        {
            cbxComPort.Text = string.Empty;
            var portas = _icBox.GetPorts();
            cbxComPort.Items.Clear();
            foreach (var porta in portas)
            {
                cbxComPort.Items.Add(porta);
            }
        }

        private void SelecionarPortaConfigurada()
        {
            Configuracao conf = _gConfig.ObterConfiguracaoPortaCOM();
            if (conf != null)
            {
                foreach (var obj in cbxComPort.Items)
                {
                    if (obj.ToString() == conf.Valor)
                    {
                        cbxComPort.SelectedItem = obj;
                    }
                }
            }
        }

        #region Notify Icon e comportamento da janela

        private void ConfigurarNotifyIcon()
        {
            ShowInTaskbar = false;
            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseDoubleClick += _notifyIcon_MouseDoubleClick;
            _notifyIcon.Icon = new Icon(AppContext.BaseDirectory + @"\Icone\ProWaiter_22x22.ico");
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "ProWaiter ICBox";            
            MenuItem menuItemSair = new MenuItem() { Text = "Sair" };
            menuItemSair.Click += MenuItemSair_Click;
            _notifyIcon.ContextMenu = new ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(menuItemSair);
        }

        private void MenuItemSair_Click(object sender, EventArgs e)
        {
            _fechar = true;
            Close();
        }

        private void _notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TopMost = true;
            Show();
            WindowState = FormWindowState.Normal;            
            BringToFront();
            TopMost = false;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {                
                Hide();
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_fechar)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                Desconectar();
            }
        }


        #endregion

        private void btnSalvarNaBaseDados_Click(object sender, EventArgs e)
        {
            try
            {                
                _gConfig.SalvarConfiguracao(cbxComPort.Text);
                MessageBox.Show("Configurações salvas!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar salvar a configuração na base de dados\r\n" + ex.Message);
            }
        }

        private void btAtualizarPortasDisponiveis_Click(object sender, EventArgs e)
        {
            AtualizarPortas();
        }

        #region Monitoramento

        private void btnConectar_Click(object sender, EventArgs e)
        {
            Conectar();
        }

        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            Desconectar();
        }

        private void Conectar()
        {
            if(_thread == null || !_thread.IsAlive)
            {
                _thread = new Thread(new ThreadStart(Monitorar));
                _thread.IsBackground = true;
                _thread.Start();
            }

            string portaCOM = cbxComPort.Text;

            if (string.IsNullOrWhiteSpace(portaCOM))
            {
                if (!_inicializando)
                    ExibirAlerta("Selecione uma porta COM");
                return;
            }

            if (_icBox.openCom(portaCOM))
            {
                if (_icBox.checkConnectionStatus())
                {
                    SetarTextoLabelMonitoramento("Monitorando...");
                    btnDesconectar.Enabled = true;
                    btnConectar.Enabled = false;
                    btnAtualizarPortasDisponiveis.Enabled = false;
                    btnSalvarNaBaseDados.Enabled = false;
                    cbxComPort.Enabled = false;
                    SetarProgresso(true);
                }
                else
                {
                    _icBox.closeCom();
                    ExibirErro("Erro ao tentar conectar com o ICBox");
                }
            }
            else
            {
                ExibirErro("Erro ao tentar conectar com o ICBox");
            }
        }

        private void Desconectar()
        {
            if (_icBox.closeCom())
                AjustarInterfaceDesconectado();
        }

        private void Monitorar()
        {
            int i = 0;

            try
            {
                while (true)
                {
                    if (_interromperLeituraCOM)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (_icBox.isConnected())
                    {
#if (!DEBUG)
                        i++;
                        if (i % 10 == 0)
                        {
                            AdicionarItemListBoxDados("Dados recebidos - " + i.ToString());
                            _gConfig.SalvarTelefoneDetectado("999664484");
                        }
#endif

                        string evento = _icBox.getEvent(500);
                        if (!string.IsNullOrWhiteSpace(evento))
                        {
                            string telefone = ExtrairNumero(evento);
                            _gConfig.SalvarTelefoneDetectado(telefone);
                        }
                    }
                    else
                        AjustarInterfaceDesconectado();
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex) 
            { 
                ExibirErro(ex.ToString()); AjustarInterfaceDesconectado(); 
            }
        }

        private string ExtrairNumero(string evento)
        {
            if (string.IsNullOrWhiteSpace(evento))
                return string.Empty;
            //telefone = "154999664484E001000000000000I000000"; //celular
            //telefone = "15434612089E001000000000000I000000"; //Fixo

            string[] partes = evento.Split(new char[] { 'E' });
            if (partes.Length > 0)
            {
                string telefone = partes[0].Trim();
                if (telefone.Length > 2)
                {
                    telefone = telefone.Substring(1);
                    return telefone;
                }
            }
            return string.Empty;
        }

    #endregion

    #region Util

        private delegate void TextoDelegate(string text);
        private delegate void IntDelegate(bool iniciar);
        private delegate void Vazio();

        private void SetarProgresso(bool iniciar)
        {
            if (InvokeRequired)
                Invoke(new IntDelegate(SetarProgresso), new object[] { iniciar });
            else
            {
                if (iniciar)
                    progressBar.Style = ProgressBarStyle.Marquee;
                else
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = 0;
                }
            }
        }

        private void SetarTextoLabelMonitoramento(string texto)
        {
            if (InvokeRequired)
                Invoke(new TextoDelegate(SetarTextoLabelMonitoramento), new object[] { texto });
            else
                lblMonitoramento.Text = texto;
        }

        private void AdicionarItemListBoxDados(string texto)
        {
            if (InvokeRequired)
                Invoke(new TextoDelegate(AdicionarItemListBoxDados), new object[] { texto });
            else
            {
                if (lbxDados.Items.Count > 20)
                {
                    lbxDados.Items.Clear();
                }
                lbxDados.Items.Add(texto);
                if (lbxDados.Items.Count > 0)
                    lbxDados.SelectedIndex = lbxDados.Items.Count - 1;
            }
        }

        private void AjustarInterfaceDesconectado()
        {
            if (btnConectar.Enabled)
                return;

            if (InvokeRequired)
            {
                Invoke(new Vazio(AjustarInterfaceDesconectado));
            }
            else
            {
                SetarProgresso(false);
                btnConectar.Enabled = true;
                btnDesconectar.Enabled = false;
                btnAtualizarPortasDisponiveis.Enabled = true;
                btnSalvarNaBaseDados.Enabled = true;
                cbxComPort.Enabled = true;
                SetarProgresso(false);
                SetarTextoLabelMonitoramento("Parado");
                if (_icBox.isConnected())
                    _icBox.closeCom();
            }
        }

        private void ExibirErro(string msg)
        {
            MessageBox.Show(msg, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ExibirAlerta(string msg)
        {
            MessageBox.Show(msg, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ExibirInformacao(string msg)
        {
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnStatusICBox_Click(object sender, EventArgs e)
        {
            _interromperLeituraCOM = true;
            Thread.Sleep(1000);
            if(_icBox.isConnected() && _icBox.checkConnectionStatus())
            {
                ExibirInformacao("ICBox OK");
            }
            else
            {
                ExibirErro("O ICBox não está respondendo");
            }
            _interromperLeituraCOM = false;
        }

        private void btnGancho_Click(object sender, EventArgs e)
        {
            if(!_icBox.isConnected())
            {
                ExibirAlerta("ICBox não está conectado");
                return;
            }

            if (_icBox.getOnHook())
            {
                ExibirInformacao("No gancho");
            }
            else
            {
                ExibirInformacao("Fora do gancho");
            }
        }

    #endregion
    }
}