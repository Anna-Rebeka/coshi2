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
        public string LineNumber => txtLineNumber.Text; // Z�skanie zadan�ho ��sla riadku

        public GoToLineDialog()
        {
            InitializeComponent();
            txtLineNumber.Focus(); // Focus na textbox pre pohodln� vstup
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Nastavenie v�sledku dial�gov�ho okna na True, �o indikuje stla�enie tla�idla OK
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Nastavenie v�sledku dial�gov�ho okna na False, �o indikuje zru�enie dial�gov�ho okna
        }
    }
}
