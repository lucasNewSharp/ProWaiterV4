using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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

namespace NewSharp.AtualizadorProWaiter.Controles
{
    /// <summary>
    /// Interaction logic for UCNotasDaVersao.xaml
    /// </summary>
    public partial class UCNotasDaVersao : UserControl
    {
        public UCNotasDaVersao()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!DesignerProperties.GetIsInDesignMode(this))
                txtNotasVersao.Text = File.ReadAllText("NotasDaVersao.txt");
        }
    }
}
