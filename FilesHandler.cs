using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace coshi2
{
    public static class FilesHandler
    {
        public static void save(string code)
        {
            if (string.IsNullOrEmpty(Settings.CURRENTFILEPATH))
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Textové súbory (*.txt)|*.txt|Všetky súbory (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    Settings.CURRENTFILEPATH = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(Settings.CURRENTFILEPATH))
                {
                    writer.Write("{" + Settings.SOUND_PACKAGE.name + "," + Settings.MAP_SQRT_SIZE + "} \n");
                    writer.Write(code);

                }

                MessageBox.Show("Súbor sa uložil.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba pri ukladaní do súboru: {ex.Message}");
            }
        }

        public static string open()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Settings.CURRENTFILEPATH = openFileDialog.FileName;

                using (var reader = new StreamReader(Settings.CURRENTFILEPATH))
                {
                    string firstline = "";
                    if (!reader.EndOfStream)
                    {
                        // Ulož prvý riadok do premennej settings
                        string filesettings = reader.ReadLine().Trim();
                        if (!filesettings.Contains("{") || !filesettings.Contains("}") || !filesettings.Contains(","))
                        {
                            MessageBox.Show("Upozornenie: Nepodarili sa zistiť pôvodné nastavenia.");
                            firstline = filesettings;
                        }
                        else
                        {
                            string[] parameters = filesettings.Substring(1, filesettings.Length - 2).Split(",");
                            int size = int.Parse(parameters[1]);
                            Settings.set_sound_package(parameters[0]);
                            Settings.set_size(size);
                            /*
                            changeSize(Settings.PACKAGE_SIZE);
                            DrawLabels();
                           
                            */
                        }
                    }

                    // Zvyšok súboru sa uloží do textového poľa
                    //textBox.Text = firstline + reader.ReadToEnd();
                    return firstline + reader.ReadToEnd(); ;
                }
            }
            return "";
        }

        public static void delete() { }
    }
}
