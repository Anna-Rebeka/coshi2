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

            List<string> helpItems = new List<string>
            {
                "Klávesové skratky",
                "CTRL+ Funkcie:",
                "- CTRL + N  Nový",
                "- CTRL + O  Otvoriť",
                "- CTRL + S  Uložiť",
                "- CTRL + Shift + S  Uložiť ako",
                "- CTRL + Z  Späť",
                "- CTRL + Y  Opakovať",
                "- CTRL + X  Vystrihnuť",
                "- CTRL + C  Kopírovať",
                "- CTRL + V  Vložiť",
                "- CTRL + G  Skok na ľubovoľný riadok",
                "- CTRL + H  Skok po blokoch kódu smerom nadol",
                "- CTRL + Shift + H  Skok po blokoch kódu smerom nahor",
                "- CTRL + Medzerník  Výber z predikcie kódu",
                "",
                "Fn Funkcie:",
                "- F1  Zobraz pomoc",
                "- F2  Menu",
                "- F5  Spusti program",
                "- Shift+F5  Zastav program",
                "- F6  Prepínač medzi kódom, grafickou plochou a terminálom",
                "- F7  Rýchlejšie prehrávanie",
                "- F8  Pomalšie prehrávanie",
                "- F9  Prepínač medzi tmavým a svetlým režimom",
                "",
                "Iné:",
                "- P  Aktuálna poloha robota (ak sme na grafickej ploche)",
                "- Z  Vypíše zoznam zvukov z balíčku (ak sme na paneli s predikciou)",
                "- Alt + F4  Koniec"
            };


            commandsListView.ItemsSource = helpItems;
            if (Settings.THEME == Theme.Dark)
            {
                commandsListView.Foreground = Brushes.White;
                commandsListView.Background = Brushes.Black;
            }
            else
            {
                commandsListView.Foreground = Brushes.Black;
                commandsListView.Background = Brushes.White;
            }

            //pockaj na vykreslenie
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ListViewItem firstItem = (ListViewItem)commandsListView.ItemContainerGenerator.ContainerFromItem(commandsListView.Items[0]);
                if(firstItem != null)
                {
                    firstItem.Focus();
                }
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void AddText()
        {
            /*
            helptextbox.Text = "Klávesové skratky:\n\n" +

            "- CTL+ Funkcie:\n\n" +
            "- CTRL + N  Nový \n" +
            "- CTRL + O  Otvoriť\n" +
            "- CTRL + S  Uložiť\n" +
            "- CTRL + Shift + S  Uložiť ako\n" +
            "- CTRL + Z  Späť\n" +
            "- CTRL + Y  Opakovať\n" +
            "- CTRL + X  Vystrihnuť\n" +
            "- CTRL + C  Kopírovať\n" +
            "- CTRL + V  Vložiť\n" +
            "- CTRL + G  Skok na ľubovoľný riadok\n" +
            "- CTRL + H  Skok po blokoch kódu smerom nadol\n" +
            "- CTRL + Shift + H  Skok po blokoch kódu smerom nahor\n" +
            "- CTRL + Medzerník  Výber z predikcie kódu\n" +

            "- Fn Funkcie:\n\n" +
            "- F1  Zobraz pomoc\n" +
            "- F2  Menu\n" +
            "- F5  Spusti program\n" +
            "- Shift+F5  Zastav program\n" +
            "- F6  Prepínač medzi kódom, grafickou plochou a terminálom\n" +
            "- F7  Rýchlejšie prehrávanie\n" +
            "- F8  Pomalšie prehrávanie\n" +
            "- F9  Prepínač medzi tmavým a svetlým režimom\n" +

            "- Iné:\n\n" +
            "- P  Aktuálna poloha robota (ak sme na grafickej ploche)\n" +
            "- Z  Vypíše zoznam zvukov z balíčku (ak sme na paneli s predikciou)\n" +
            "- Alt + F4  Koniec\n\n\n";


            helptextbox.TextAlignment = TextAlignment.Center;
            */
        }

        private void Help_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
           

        }
        


    }
}
