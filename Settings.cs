using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;

namespace coshi2
{
    public static class Settings
    {
        public static Canvas[,] MAP;
        public static int MAP_SIZE = 9;
        public static int MAP_SQRT_SIZE = 3;
        public static int PACKAGE_SIZE = 3;
        public static SoundPackage SOUND_PACKAGE = new SoundPackage("zvierata");
        public static bool SILENCE = false;
        public static string CURRENTFILEPATH = null;

        public static void set_size(int size)
        {
            MAP_SIZE = size * size;
            MAP_SQRT_SIZE = size;
        }

        public static void set_sound_package(string name)
        {
            SoundPackage trypackage = SoundsHandler.load_sound_package(name);
            if (trypackage.SoundItems.Count == 0) {
                MessageBox.Show("Chyba: Nepodarilo sa nájsť zvukový balíček: " + name);
                return;
            }
            SOUND_PACKAGE = trypackage;
            PACKAGE_SIZE = (int) Math.Sqrt(SOUND_PACKAGE.SoundItems.Count());
            set_size(PACKAGE_SIZE);
            SoundsHandler.fill_sound_map();   
        }
    }
}
