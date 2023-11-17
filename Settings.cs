﻿using System;
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
        public static int PACKAGE_SIZE = 3;
        public static SoundPackage SOUND_PACKAGE = new SoundPackage("zvierata");
        public static bool SILENCE = false;

        public static void set_size(int size)
        {
            MAP_SIZE = size * size;
            MAP_SQRT_SIZE = size;
        }

        public static void set_sound_package(string name)
        {
            SOUND_PACKAGE = SoundsHandler.load_sound_package(name);
            PACKAGE_SIZE = (int) Math.Sqrt(SOUND_PACKAGE.SoundItems.Count());
            set_size(PACKAGE_SIZE);
            SoundsHandler.fill_sound_map();   
        }
    }
}
