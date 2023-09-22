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
        public Dictionary<string, SoundItem> SoundItems { get; set; }

        public SoundPackage(string name)
        {
            SoundItems = new Dictionary<string, SoundItem>();
            this.name = name;
        }

        public string GetSoundFilePath(string soundName)
        {
            return SoundItems[soundName].Path;
        }




    }
}
