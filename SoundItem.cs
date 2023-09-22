using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    public class SoundItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public SoundItem(int x, int y, string name, string filePath)
        {
            X = x;
            Y = y;
            Name = name;
            Path = filePath;
        }
    }
}
