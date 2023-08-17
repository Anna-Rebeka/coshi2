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
        public static List<int[]>positions = new List<int[]> { new int[] { 0, 0 }};

    public static void reset()
        {
            position = 1;
            positions = new List<int[]>
            {
                new int[] { 0, 0 }
            };
        }

    public static void up(int map_size)
        {           
            position -= (int)Math.Sqrt(map_size);
            int pos = position - 1;
            positions.Add(new int[] { pos / 3 , pos % 3 });

        }

        public static void down(int map_size)
        {
            int sqrt_map_size = (int)Math.Sqrt(map_size);
            if (position + sqrt_map_size > map_size)
            {
                throw new RobotOutOfMapException();
            }
            position += sqrt_map_size;
            int pos = position - 1;
            positions.Add(new int[] { pos / 3, pos % 3 });
        }

        public static void left(int map_size)
        {
            int sqrt_map_size = (int)Math.Sqrt(map_size);
            position -= 1;
            int pos = position - 1;
            positions.Add(new int[] { pos / 3, pos % 3 });
        }

        public static void right(int map_size)
        {
            int sqrt_map_size = (int)Math.Sqrt(map_size);
            position += 1;
            int pos = position - 1;
            positions.Add(new int[] { pos / 3, pos % 3 });
        }

    }
}
