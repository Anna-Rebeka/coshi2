using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Media;
using System.IO;
using System.Windows;
using System.Threading;
using System.Security.Policy;

namespace coshi2
{
    public static class SoundsHandler
    {
        public static SoundItem[,] sounds_map = new SoundItem[Settings.MAP_SQRT_SIZE, Settings.MAP_SQRT_SIZE]; //z tohto pustame zvuky
        public static SoundPlayer player;
        public static string mainDirectory = "../../../sounds";


        public static void restart()
        {
            sounds_map = new SoundItem[Settings.MAP_SQRT_SIZE, Settings.MAP_SQRT_SIZE]; 
        }


        public static SoundPackage load_sound_package(string name)
        {
            restart();
            string directory = Path.Combine(mainDirectory, name);
            SoundPackage soundPackage = new SoundPackage(name);
            try
            {
                string[] packageSoundFiles = Directory.GetFiles(directory);
                string definitionFile = Path.Combine(directory, "definition.txt");

                if (File.Exists(definitionFile))
                {
                    string[] lines = File.ReadAllLines(definitionFile);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length == 4)
                        {
                            int x = int.Parse(parts[0]);
                            int y = int.Parse(parts[1]);
                            string soundName = parts[2];
                            string filePath = Path.Combine(directory, parts[3]);

                            // SoundItem pre každý zvuk v balíčku
                            SoundItem soundItem = new SoundItem(x, y, soundName, filePath);

                            // SoundItem do SoundPackage
                            soundPackage.SoundItems[soundName] = soundItem;
                        }
                    }
                }
            }
            catch { }

            return soundPackage;
        }

        public static void fill_sound_map()
        {
            
            foreach(SoundItem sound in Settings.SOUND_PACKAGE.SoundItems.Values)
            {
                int x = sound.X;
                int y = sound.Y;

                // názov zvuku
                string soundName = sound.Name;

                // na príslušné miesto v sounds_map
                if (x >= 0 && x < sounds_map.GetLength(0) && y >= 0 && y < sounds_map.GetLength(1))
                {
                    sounds_map[x, y] = sound;
                }
            }
           
        }

        public static void play_sound(int x, int y)
        {
            //with open... zahodime player
            if (y >= Settings.MAP_SQRT_SIZE || x >= Settings.MAP_SQRT_SIZE || x < 0 || y < 0) { return; }
            if (x >= Math.Sqrt(sounds_map.Length)|| y >= Math.Sqrt(sounds_map.Length) || ( sounds_map[x, y]  == null)) { return; }
            using (player = new SoundPlayer(sounds_map[x, y].Path))
            {
                player.Play();
            };

        }
    }
}
