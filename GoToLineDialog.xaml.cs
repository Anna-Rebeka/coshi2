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
using System.Windows.Shapes;

namespace coshi2
{
    public partial class GoToLineDialog : Window
    {
        public string LineNumber => txtLineNumber.Text; // Získanie zadaného èísla riadku

        public GoToLineDialog()
        {
            InitializeComponent();
            txtLineNumber.Focus(); // Focus na textbox pre pohodlný vstup
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Nastavenie výsledku dialógového okna na True, èo indikuje stlaèenie tlaèidla OK
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Nastavenie výsledku dialógového okna na False, èo indikuje zrušenie dialógového okna
        }
    }
}
