using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    public class SoundPackage
    {
        public string name;
        public Dictionary<(int, int), SoundItem> SoundItems { get; set; }

        public SoundPackage(string name)
        {
            SoundItems = new Dictionary<(int, int), SoundItem>();
            this.name = name;
        }

        public string GetSoundFilePath(int x, int y)
        {
            return SoundItems[(x, y)].Path;
        }




    }
}
