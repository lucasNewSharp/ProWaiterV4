using ProWaiter.Licenca.Entidades;
using ProWaiter.Licenca.Gestores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProWaiter.Licenca
{
    public partial class FrmMain : Form
    {
        private const string TextoAtivandoLicenca = "Ativando licenca, aguarde...";
        private const string TextoValidandoLicenca = "Validando licenca, aguarde...";
        private const string TextoLicencaAtivada = "LICENÇA ATIVADA!";
        private const string TextoLicencaInativa = "LICENCA INATIVA!";
        private string _erroAtual = string.Empty;

        private Timer _timer = null;
        private NotifyIcon _notifyIcon;
        private bool _fechar = false;

        public FrmMain()
        {
            InitializeComponent();
            ConfiguracoesIniciais();
        }

        private void ConfiguracoesIniciais()
        {
            ConfigurarNotifyIcon();
            txtNome.MaxLength = LicencaProWaiter.TamMaxNome;
            txtCidade.MaxLength = LicencaProWaiter.TamMaxCidade;
            txtEndereco.MaxLength = LicencaProWaiter.TamMaxEndereco;
            txtUF.MaxLength = LicencaProWaiter.TamUf;

            GestorLicencas gLic = new GestorLicencas(new LicencasContext());
            if (!gLic.LicencaExiste())
            {
                AtivarControles(true);
            }
            else
            {
                LicencaProWaiter licenca = gLic.ObterLicenca();
                txtNome.Text = licenca.Nome;
                txtEndereco.Text = licenca.Endereco;
                txtUF.Text = licenca.UF;
                txtCidade.Text = licenca.Cidade;
                txtChave.Text = licenca.Segredo;
                txtDataAtivacao.Text = licenca.DataAtivacao.ToShortDateString();
                txtUltimaValidacao.Text = DateTime.FromFileTimeUtc(licenca.Validacao).Date.ToShortDateString();

                if (licenca.Ativo)
                    lblStatus.Text = TextoLicencaAtivada;
                else
                    lblStatus.Text = TextoLicencaInativa;
                IniciarTimer();
            }
        }

        private void ConfigurarNotifyIcon()
        {
            ShowInTaskbar = false;
            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseDoubleClick += _notifyIcon_MouseDoubleClick;
            _notifyIcon.Icon = new Icon(AppContext.BaseDirectory + @"\Imagens\ProWaiterLicenca.ico");
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "ProWaiter Licença";
            MenuItem menuItemSair = new MenuItem() { Text = "Sair" };
            menuItemSair.Click += MenuItemSair_Click;
            _notifyIcon.ContextMenu = new ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(menuItemSair);
        }

        private void MenuItemSair_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza que deseja fechar o validador da sua licença?", "ProWaiter Licença", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _fechar = true;
                Close();
            }
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
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }


        #region Timer

        private void IniciarTimer()
        {
            if(_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            _timer = new Timer();
            _timer.Interval = 600000; //10 minutos
            _timer.Tick += ExecutarValidacao;
            _timer.Start();
        }

        private void ExecutarValidacao(object sender, EventArgs e)
        {
            string msg = null;
            GestorLicencas gLic = new GestorLicencas(new LicencasContext());
            try
            {

                if (gLic.LicencaExiste())
                {
                    lblStatus.Text = TextoValidandoLicenca;
                    Application.DoEvents();
                    gLic.ValidarLicenca(out msg);
                    lblStatus.Text = TextoLicencaAtivada;
                }
                _erroAtual = string.Empty;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "ERRO";
                _erroAtual = ex.ToString();
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                lblStatus.Text = msg;
            }
        }

        #endregion

        private bool FormularioEhValido()
        {
            if (TxtEstaEmBaraco(txtNome))
            {
                ExibirMsgErro("Digite o nome do estabelecimento");
                return false;
            }
            if (TxtEstaEmBaraco(txtEndereco))
            {
                ExibirMsgErro("Digite o endereço do estabelecimento");
                return false;
            }
            if (TxtEstaEmBaraco(txtCidade))
            {
                ExibirMsgErro("Digite a cidade do estabelecimento");
                return false;
            }
            if (TxtEstaEmBaraco(txtUF))
            {
                ExibirMsgErro("Digite a UF do estabelecimento");
                return false;
            }
            if (txtUF.Text.Length != LicencaProWaiter.TamUf)
            {
                ExibirMsgErro("Digite a UF com 2 caracteres, exemplo (RS)");
                return false;
            }
            if (TxtEstaEmBaraco(txtChave))
            {
                ExibirMsgErro("Digite a chave do produto");
                return false;
            }
            if(txtChave.Text.Trim().Length != LicencaProWaiter.TamSegredo)
            {
                ExibirMsgErro("A chave deve conter 32 caracteres");
                return false;
            }

            return true;
        }

        private bool TxtEstaEmBaraco(TextBox txt)
        {
            return string.IsNullOrWhiteSpace(txt.Text.Trim());
        }

        private void ExibirMsgErro(string msg)
        {
            MessageBox.Show(msg, "ProWaiter Licenca - ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AtivarControles(bool v)
        {
            txtNome.ReadOnly =
                txtEndereco.ReadOnly =
                txtCidade.ReadOnly =
                txtUF.ReadOnly =
                txtChave.ReadOnly = !v;
            btnAtivar.Enabled = v;
        }

        private void resetarAtivaçãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza que deseja liberar o formulário para efetuar uma nova ativação?", "CUIDADO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                AtivarControles(true);
                if (_timer != null)
                    _timer.Stop();
                txtNome.Text =
                    txtEndereco.Text =
                    txtCidade.Text =
                    txtDataAtivacao.Text =
                    txtUF.Text = string.Empty;
            }
        }

        private void btnAtivar_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormularioEhValido())
                {
                    lblStatus.Text = TextoAtivandoLicenca;
                    Application.DoEvents();
                    GestorLicencas gLic = new GestorLicencas(new LicencasContext());
                    string msg;
                    gLic.AtivarLicenca(new LicencaDTO()
                    {
                        Nome = txtNome.Text.Trim(),
                        Cidade = txtCidade.Text.Trim(),
                        Endereco = txtEndereco.Text.Trim(),
                        Segredo = txtChave.Text.Trim(),
                        UF = txtUF.Text.Trim()
                    }, out msg);

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        ExibirMsgErro(msg);
                        lblStatus.Text = TextoLicencaInativa;
                    }
                    else
                    {
                        lblStatus.Text = TextoLicencaAtivada;
                        txtDataAtivacao.Text = DateTime.Today.ToShortDateString();
                        IniciarTimer();
                        AtivarControles(false);
                    }
                }
                _erroAtual = string.Empty;
            }
            catch (Exception ex)
            {
                _erroAtual = ex.ToString();
                lblStatus.Text = "ERRO";
            }
        }

        private void forcarValidacaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GestorLicencas gLic = new GestorLicencas(new LicencasContext());
                if (!gLic.LicencaExiste())
                {
                    ExibirMsgErro("Não existe licença configurada!");
                    return;
                }
                string msg;
                lblStatus.Text = TextoValidandoLicenca;
                Application.DoEvents();
                gLic.ValidarLicenca(out msg);
                if (!string.IsNullOrWhiteSpace(msg))
                    lblStatus.Text = msg;
                else
                    lblStatus.Text = TextoLicencaAtivada;
                _erroAtual = string.Empty;
            }
            catch (Exception ex)
            {
                _erroAtual = ex.ToString();
                lblStatus.Text = "ERRO";
            }
        }

        private void verUltimoErroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_erroAtual))
            {
                FrmErro frmErro = new FrmErro(_erroAtual);
                frmErro.Show();
            }
            else
            {
                MessageBox.Show("Não existem erros para exibir!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}