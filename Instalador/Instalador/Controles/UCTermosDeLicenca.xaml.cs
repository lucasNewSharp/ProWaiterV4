using System;
using System.Collections.Generic;
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

namespace Instalador.Controles
{
    /// <summary>
    /// Interaction logic for UCTermosDeLicenca.xaml
    /// </summary>
    public partial class UCTermosDeLicenca : UserControl
    {
        public delegate void AoClicarAceitoHandler(bool valor);
        public event AoClicarAceitoHandler AoClicarAceito;

        public UCTermosDeLicenca()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            txtTermos.Text = @"Bacon ipsum dolor amet frankfurter doner short loin tongue. Flank meatloaf andouille spare ribs chicken. Kielbasa spare ribs jowl swine ground round turkey tri-tip capicola. Andouille ball tip porchetta doner, jerky cow shank bresaola.

Turkey chuck venison rump picanha fatback, strip steak ground round sausage boudin spare ribs brisket. Porchetta beef ribs boudin leberkas t-bone cow ribeye, short loin andouille brisket turkey tri-tip doner shoulder venison.Spare ribs brisket fatback ground round ribeye, pork drumstick turkey beef ribs porchetta turducken ham hock.Chuck ham pig landjaeger burgdoggen.Chislic frankfurter capicola prosciutto, kevin rump porchetta picanha pork drumstick venison chicken.Filet mignon tri - tip fatback, beef andouille pork bacon biltong.Alcatra cow meatball burgdoggen.

Capicola leberkas drumstick short ribs, shankle pork chop venison pastrami sirloin spare ribs pork loin shank burgdoggen beef. Beef ribs pork belly frankfurter, swine cow salami t-bone picanha drumstick bresaola.Beef jowl capicola pork belly boudin ham hock. Chislic rump jowl tail porchetta shankle bacon, sausage short loin alcatra drumstick prosciutto picanha ground round. Doner meatball cupim, beef boudin beef ribs bacon venison ground round kevin pork loin drumstick porchetta.Venison turkey tongue hamburger beef ribs shank.

Turducken boudin burgdoggen, cupim meatball ground round kevin jerky ham tongue chicken landjaeger. Filet mignon chicken venison, buffalo picanha jowl brisket.Salami porchetta jerky frankfurter tail, leberkas jowl filet mignon. Sausage capicola chicken frankfurter t - bone tongue shankle leberkas doner porchetta chuck ball tip biltong pork belly.Pig bacon bresaola spare ribs.

Chuck pork belly tenderloin turkey kevin tri - tip, tail kielbasa andouille jowl drumstick spare ribs sausage salami prosciutto. Picanha strip steak doner tri - tip turkey jerky cupim brisket filet mignon ham venison kielbasa chuck. Cow turkey beef ground round flank buffalo venison. Filet mignon drumstick short loin ham salami prosciutto beef biltong.Cow biltong salami tongue filet mignon ball tip bresaola.";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AoClicarAceito.Invoke(ckbAceito.IsChecked.Value);
        }
    }
}
