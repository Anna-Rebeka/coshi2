using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    public class VirtualMachine
    {
        public int INSTRUCTION_UP = 1;
        public int INSTRUCTION_LT = 2;
        public int INSTRUCTION_RT = 3;
        public int INSTRUCTION_DW = 4;
        public int INSTRUCTION_SET = 5;
        public int INSTRUCTION_LOOP = 6;

        public int[] mem = new int[100];    //pamäť – pole celých čísel 
        public int pc;                      //adresa inštrukcie ked sa vykonava program
        public int adr;                     //adresa pre naplnanie mem
        public bool terminated;             //stav procesora
        public int map_size = 9;
        public VirtualMachine() { }

        public void reset() {
            pc = 0;
            terminated = false;
        }

        public void execute_all()
        {

            /*
            
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += execute;
             */
            while (!terminated)
            {
                execute();
            }
        }


        public void execute()
        {
            if (mem[pc] == INSTRUCTION_UP)
            {
                pc = pc + 1;
                
                //forward(mem[pc]);
                pc = pc + 1;
            }
            else if (mem[pc] == INSTRUCTION_LT)
            {
                pc = pc + 1;
                //left(mem[pc]);
                pc = pc + 1;
            }
            else if (mem[pc] == INSTRUCTION_RT)
            {
                pc = pc + 1;
                //right(mem[pc]);
                pc = pc + 1;
            }
            else if (mem[pc] == INSTRUCTION_DW)
            {
                pc = pc + 1;
                //Robot.down(map_size);
                //positions.add(Robot.down(map_size))
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

        public void poke(int code)
        {
            mem[adr] = code;
            adr = adr + 1;
        }
    }
}
