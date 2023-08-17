using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    public static class VirtualMachine
    {
        public static int INSTRUCTION_UP = 1;
        public static int INSTRUCTION_LT = 2;
        public static int INSTRUCTION_RT = 3;
        public static int INSTRUCTION_DW = 4;
        public static int INSTRUCTION_SET = 5;
        public static int INSTRUCTION_LOOP = 6;

        public static int[] mem = new int[100];    //pamäť – pole celých čísel 
        public static int pc;                      //adresa inštrukcie ked sa vykonava program
        public static int adr;                     //adresa pre naplnanie mem
        public static bool terminated;             //stav procesora
        public static int map_size = 9;

        public static void reset()
        {
            pc = 0;
            adr = 0;
            terminated = false;
            mem = new int[100];
        }

        public static void execute_all()
        {
            while (!terminated)
            {
                execute();
            }
        }

        public static void execute()
        {
            if (mem[pc] == INSTRUCTION_UP)
            {
                pc = pc + 1;
                Robot.up(map_size);
            }
            else if (mem[pc] == INSTRUCTION_LT)
            {
                pc = pc + 1;
                Robot.left(map_size);
            }
            else if (mem[pc] == INSTRUCTION_RT)
            {
                pc = pc + 1;
                Robot.right(map_size);
            }
            else if (mem[pc] == INSTRUCTION_DW)
            {
                pc = pc + 1;
                Robot.down(map_size);
            }
            else if (mem[pc] == INSTRUCTION_SET)
            {
                pc = pc + 1;
                int index = mem[pc];
                pc = pc + 1;
                mem[index] = mem[pc];
                pc = pc + 1;
            }
            else if (mem[pc] == INSTRUCTION_LOOP)
            {
                pc = pc + 1;
                int index = mem[pc];
                pc = pc + 1;
                mem[index] = mem[index] - 1;
                if (mem[index] > 0)
                {
                    pc = mem[pc];
                }
                else
                {
                    pc = pc + 1;
                }
            }
            else
            {
                terminated = true;
            }
        }

        public static void poke(int code)
        {
            mem[adr] = code;
            adr = adr + 1;
        }
    }
}
