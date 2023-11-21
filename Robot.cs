using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

    public static void up()
        {
            if ((position-1) / Settings.MAP_SQRT_SIZE == 0)
            {
                throw new RobotOutOfMapException();
            }
                position -= Settings.MAP_SQRT_SIZE;
            int pos = position - 1;
            positions.Add(new int[] { pos / Settings.MAP_SQRT_SIZE, pos % Settings.MAP_SQRT_SIZE });

        }

        public static void down()
        {
            if (position + Settings.MAP_SQRT_SIZE > Settings.MAP_SIZE)
            {
                throw new RobotOutOfMapException();
            }
            position += Settings.MAP_SQRT_SIZE;
            int pos = position - 1;
            positions.Add(new int[] { pos / Settings.MAP_SQRT_SIZE, pos % Settings.MAP_SQRT_SIZE });
        }

        public static void left()
        {
            if((position-1) % Settings.MAP_SQRT_SIZE == 0)
            {
                throw new RobotOutOfMapException();
            }
            position -= 1;
            int pos = position - 1;
            positions.Add(new int[] { pos / Settings.MAP_SQRT_SIZE, pos % Settings.MAP_SQRT_SIZE });
        }

        public static void right()
        {
            if ((position) % Settings.MAP_SQRT_SIZE == 0)
            {
                throw new RobotOutOfMapException();
            }
            position += 1;
            int pos = position - 1;
            positions.Add(new int[] { pos / Settings.MAP_SQRT_SIZE, pos % Settings.MAP_SQRT_SIZE });
        }


        public static void silence()
        {
            positions.Add(new int[] { -100, -100 });
        }


        public static void loud()
        {
            positions.Add(new int[] { 100, 100 });
        }
    }
}
