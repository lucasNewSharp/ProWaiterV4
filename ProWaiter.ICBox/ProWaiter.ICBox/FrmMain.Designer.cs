namespace ProWaiter.ICBox
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSalvarNaBaseDados = new System.Windows.Forms.Button();
            this.btnAtualizarPortasDisponiveis = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.cbxComPort = new System.Windows.Forms.ComboBox();
            this.btnConectar = new System.Windows.Forms.Button();
            this.btnDesconectar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lbxDados = new System.Windows.Forms.ListBox();
            this.lblMonitoramento = new System.Windows.Forms.Label();
            this.btnStatusICBox = new System.Windows.Forms.Button();
            this.btnGancho = new System.Windows.Forms.Button();
            this.GroupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.btnSalvarNaBaseDados);
            this.GroupBox2.Controls.Add(this.btnAtualizarPortasDisponiveis);
            this.GroupBox2.Controls.Add(this.Label1);
            this.GroupBox2.Controls.Add(this.cbxComPort);
            this.GroupBox2.Controls.Add(this.btnConectar);
            this.GroupBox2.Controls.Add(this.btnDesconectar);
            this.GroupBox2.Location = new System.Drawing.Point(12, 12);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(291, 143);
            this.GroupBox2.TabIndex = 13;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Conexão";
            // 
            // btnSalvarNaBaseDados
            // 
            this.btnSalvarNaBaseDados.Location = new System.Drawing.Point(14, 108);
            this.btnSalvarNaBaseDados.Name = "btnSalvarNaBaseDados";
            this.btnSalvarNaBaseDados.Size = new System.Drawing.Size(261, 23);
            this.btnSalvarNaBaseDados.TabIndex = 7;
            this.btnSalvarNaBaseDados.Text = "Salvar configuração na base de dados";
            this.btnSalvarNaBaseDados.UseVisualStyleBackColor = true;
            this.btnSalvarNaBaseDados.Click += new System.EventHandler(this.btnSalvarNaBaseDados_Click);
            // 
            // btnAtualizarPortasDisponiveis
            // 
            this.btnAtualizarPortasDisponiveis.Location = new System.Drawing.Point(12, 67);
            this.btnAtualizarPortasDisponiveis.Name = "btnAtualizarPortasDisponiveis";
            this.btnAtualizarPortasDisponiveis.Size = new System.Drawing.Size(120, 23);
            this.btnAtualizarPortasDisponiveis.TabIndex = 6;
            this.btnAtualizarPortasDisponiveis.Text = "Atualizar Portas";
            this.btnAtualizarPortasDisponiveis.UseVisualStyleBackColor = true;
            this.btnAtualizarPortasDisponiveis.Click += new System.EventHandler(this.btAtualizarPortasDisponiveis_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(11, 21);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(59, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Porta COM";
            // 
            // cbxComPort
            // 
            this.cbxComPort.FormattingEnabled = true;
            this.cbxComPort.Location = new System.Drawing.Point(11, 36);
            this.cbxComPort.Name = "cbxComPort";
            this.cbxComPort.Size = new System.Drawing.Size(121, 21);
            this.cbxComPort.TabIndex = 2;
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(183, 31);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(92, 23);
            this.btnConectar.TabIndex = 4;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // btnDesconectar
            // 
            this.btnDesconectar.Location = new System.Drawing.Point(183, 65);
            this.btnDesconectar.Name = "btnDesconectar";
            this.btnDesconectar.Size = new System.Drawing.Size(92, 23);
            this.btnDesconectar.TabIndex = 5;
            this.btnDesconectar.Text = "Desconectar";
            this.btnDesconectar.UseVisualStyleBackColor = true;
            this.btnDesconectar.Click += new System.EventHandler(this.btnDesconectar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGancho);
            this.groupBox1.Controls.Add(this.btnStatusICBox);
            this.groupBox1.Controls.Add(this.lblMonitoramento);
            this.groupBox1.Controls.Add(this.lbxDados);
            this.groupBox1.Controls.Add(this.progressBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 161);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 237);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status do monitoramento porta COM";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(11, 44);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(268, 23);
            this.progressBar.TabIndex = 0;
            // 
            // lbxDados
            // 
            this.lbxDados.FormattingEnabled = true;
            this.lbxDados.Location = new System.Drawing.Point(10, 73);
            this.lbxDados.Name = "lbxDados";
            this.lbxDados.Size = new System.Drawing.Size(269, 121);
            this.lbxDados.TabIndex = 1;
            // 
            // lblMonitoramento
            // 
            this.lblMonitoramento.AutoSize = true;
            this.lblMonitoramento.Location = new System.Drawing.Point(12, 21);
            this.lblMonitoramento.Name = "lblMonitoramento";
            this.lblMonitoramento.Size = new System.Drawing.Size(41, 13);
            this.lblMonitoramento.TabIndex = 2;
            this.lblMonitoramento.Text = "Parado";
            // 
            // btnStatusICBox
            // 
            this.btnStatusICBox.Location = new System.Drawing.Point(37, 200);
            this.btnStatusICBox.Name = "btnStatusICBox";
            this.btnStatusICBox.Size = new System.Drawing.Size(92, 23);
            this.btnStatusICBox.TabIndex = 3;
            this.btnStatusICBox.Text = "Status ICBox";
            this.btnStatusICBox.UseVisualStyleBackColor = true;
            this.btnStatusICBox.Click += new System.EventHandler(this.btnStatusICBox_Click);
            // 
            // btnGancho
            // 
            this.btnGancho.Location = new System.Drawing.Point(136, 200);
            this.btnGancho.Name = "btnGancho";
            this.btnGancho.Size = new System.Drawing.Size(110, 23);
            this.btnGancho.TabIndex = 4;
            this.btnGancho.Text = "Está no gancho?";
            this.btnGancho.UseVisualStyleBackColor = true;
            this.btnGancho.Click += new System.EventHandler(this.btnGancho_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 408);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GroupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProWaiter ICBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.Button btnAtualizarPortasDisponiveis;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.ComboBox cbxComPort;
        internal System.Windows.Forms.Button btnConectar;
        internal System.Windows.Forms.Button btnDesconectar;
        private System.Windows.Forms.Button btnSalvarNaBaseDados;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMonitoramento;
        private System.Windows.Forms.ListBox lbxDados;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnStatusICBox;
        private System.Windows.Forms.Button btnGancho;
    }
}

