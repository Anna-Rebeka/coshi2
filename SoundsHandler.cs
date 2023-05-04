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

namespace coshi2
{
    public class SoundsHandler
    {
        public Canvas[,] map;
        public String[,] audioArray;
        SoundPlayer player;

        public SoundsHandler(Canvas[,] map)
        {
            this.map = map;
            this.audioArray = new String[this.map.GetLength(0), this.map.GetLength(0)];
            this.load_sounds();
        }

        public void load_sounds() {
            this.audioArray[0, 0] = "../../../sounds/zvierata/kacka.wav";
            this.audioArray[0, 1] = "../../../sounds/zvierata/kon.wav";
            this.audioArray[0, 2] = "../../../sounds/zvierata/koza.wav";
            this.audioArray[1, 0] = "../../../sounds/zvierata/krava.wav";
            this.audioArray[1, 1] = "../../../sounds/zvierata/macka.wav";
            this.audioArray[1, 2] = "../../../sounds/zvierata/mys.wav";
            this.audioArray[2, 0] = "../../../sounds/zvierata/pes.wav";
            this.audioArray[2, 1] = "../../../sounds/zvierata/prasa.wav";
            this.audioArray[2, 2] = "../../../sounds/zvierata/sliepka.wav";
        }

        public void play_sound(int riadok, int stlpec) {
            //MessageBox.Show("Hram " + riadok.ToString() + " : " + stlpec.ToString());
            //with open... zahodime player
            if (stlpec >= this.audioArray.GetLength(0) || riadok >= this.audioArray.GetLength(0) || riadok < 0 || stlpec < 0) { return;  }
            using (this.player = new SoundPlayer(this.audioArray[riadok, stlpec])) {
                this.player.Play();
            };
          
        }

    }
}
