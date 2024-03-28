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
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
            AddText();
            if (Settings.THEME == Theme.Dark)
            {
                helptextbox.Foreground = Brushes.White;
                helptextbox.Background = Brushes.Black;
            }
            else
            {
                helptextbox.Foreground = Brushes.Black;
                helptextbox.Background = Brushes.White;
            }
            helptextbox.Focus();

        }

        private void AddText()
        {
            helptextbox.Text = "Klávesové skratky:\n\n" +
            "- CTRL + N  Nový \n" +
            "- CTRL + S  Uložiť\n" +
            "- CTRL + O  Otvoriť\n" +
            "- CTRL + G  Skok na ľubovoľný riadok\n" +
            "- CTRL + H  Skok po blokoch kódu smerom nadol\n" +
            "- CTRL + Shift + H  Skok po blokoch kódu smerom nahor\n" +
            "- CTRL + Medzerník  Výber z predikcie kódu\n" +
            "- P Aktuálna poloha robota (ak sme na grafickej ploche)\n" +
            "- Alt + F4  Koniec\n\n\n" +
            "- Fn Funkcie:\n\n" +
            "- F1  Zobraz pomoc\n" +
            "- F2  Menu\n" +
            "- F5  Spusti program\n" +
            "- Shift+F5  Zastav program\n" +
            "- F6  Prepínač medzi kódom, grafickou plochou a terminálom\n" +
            "- F7  Rýchlejšie prehrávanie\n" +
            "- F8  Pomalšie prehrávanie\n" +
            "- F9  Prepínač medzi tmavým a svetlým režimom\n";
            helptextbox.TextAlignment = TextAlignment.Center;
            
        }

        private void Help_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else
            {
                e.Handled = true;
            }
            
        }
        


    }
}
