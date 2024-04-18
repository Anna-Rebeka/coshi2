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

            if (Settings.THEME == Theme.Dark)
            {
                textBlock.Foreground = Brushes.White;
                textBlock.Background = Brushes.Black;
            }
            else
            {
                textBlock.Foreground = Brushes.Black;
                textBlock.Background = Brushes.White;
            }

            txtLineNumber.Focus(); 
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; 
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
        }

        private void GoToLine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
                e.Handled = true;
            }

        }
    }
}
