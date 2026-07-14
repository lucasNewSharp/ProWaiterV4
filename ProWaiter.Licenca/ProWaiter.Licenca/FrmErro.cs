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
    public partial class FrmErro : Form
    {
        public FrmErro(string erro)
        {
            InitializeComponent();
            txtErro.Text = erro;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
