using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace coshi2
{
    public static class Settings
    {
        public static Canvas[,] MAP;
        public static int MAP_SIZE = 9;
        public static int MAP_SQRT_SIZE = 3;
        public static SoundPackage SOUND_PACKAGE = new SoundPackage("zvierata");

        public static void set_size(int size)
        {
            MAP_SIZE = size;
            MAP_SQRT_SIZE = (int)Math.Sqrt(size);
        }

        public static void set_sound_package(string name)
        {
            SOUND_PACKAGE = SoundsHandler.load_sound_package(name);
            SoundsHandler.fill_sound_map();
        }
    }
}
