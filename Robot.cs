using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    public class Robot
    {
        public static int position = 1;

        public static void down(int map_size)
        {
            position += (int) Math.Sqrt(map_size);
        }
        public static void left()
        {

        }
    }
}
