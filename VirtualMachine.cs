using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace coshi2
{
    public static class VirtualMachine
    {
        public static int INSTRUCTION_UP = 1;
        public static int INSTRUCTION_DW = 2;
        public static int INSTRUCTION_LT = 3;
        public static int INSTRUCTION_RT = 4;
        public static int INSTRUCTION_SET = 5;
        public static int INSTRUCTION_GET = 6;
        public static int INSTRUCTION_LOOP = 7;

        public static int INSTRUCTION_PRINT = 8;   //+ parameter na vrchole zásobníka
        public static int INSTRUCTION_JUMP = 9;

        public static int INSTRUCTION_ADD = 10;    //+ dva operandy na vrchole zásobníka
        public static int INSTRUCTION_SUB = 11;    //+ dva operandy na vrchole zásobníka

        public static int INSTRUCTION_MINUS = 12;  //+ jeden operand na vrchole zásobníka
        public static int INSTRUCTION_PUSH = 13;

        public static int INSTRUCTION_JUMPIFFALSE = 14;
        public static int INSTRUCTION_CALL = 15;
        public static int INSTRUCTION_RETURN = 16;

        public static int INSTRUCTION_LOW = 17;
        public static int INSTRUCTION_GREAT = 18;
        public static int INSTRUCTION_EQUAL = 19;
        public static int INSTRUCTION_LOWEQUAL = 20;
        public static int INSTRUCTION_GREATEQUAL = 21;

        public static int INSTRUCTION_FREEUP = 22;
        public static int INSTRUCTION_FREEDOWN = 23;
        public static int INSTRUCTION_FREELEFT = 24;
        public static int INSTRUCTION_FREERIGHT = 25;

        public static int[] mem = new int[100];    //pamäť – pole celých čísel 
        public static int counter_adr = mem.Length - 1;
        public static int pc;                      //adresa inštrukcie ked sa vykonava program
        public static int top;                     //index vrcholu zásobníka – celočíselná premenná 
        public static int adr;                     //adresa pre naplnanie mem
        public static bool terminated;             //stav procesora
        public static Dictionary<string, int> variables = new Dictionary<string, int>();
        public static Dictionary<string, Subroutine> subroutines = new Dictionary<string, Subroutine>();


        private static TextBox _textBox; // Store the reference to the TextBox control


        // Method to set the reference to the TextBox control
        public static void SetTextBoxReference(TextBox textBox)
        {
            _textBox = textBox;
        }

        public static void AppendTextToTextBox(string text)
        {
            if (_textBox != null)
            {
                // Append text to the TextBox using the AppendText method
                _textBox.AppendText("\n" + text);
            }
        }


        public static void reset()
        {
            pc = 0;
            top = mem.Length;
            adr = 0;
            terminated = false;
            mem = new int[100];
            variables = new Dictionary<string, int>();
            subroutines = new Dictionary<string, Subroutine>();
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
            if (mem[pc] == INSTRUCTION_PUSH)
            {
                pc = pc + 1;
                top = top - 1;
                mem[top] = mem[pc];
                pc = pc + 1;
            }
            else if (mem[pc] == INSTRUCTION_MINUS)
            {
                pc = pc + 1;
                mem[top] = -mem[top];
            }
            else if (mem[pc] == INSTRUCTION_ADD)
            {
                pc = pc + 1;
                mem[top + 1] = mem[top + 1] + mem[top];
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_SUB)
            {
                pc = pc + 1;
                mem[top + 1] = mem[top + 1] - mem[top];
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_GET)
            {
                pc = pc + 1;
                int index = mem[pc];
                pc = pc + 1;
                top = top - 1;
                mem[top] = mem[index];
            }
            else if (mem[pc] == INSTRUCTION_SET)
            {
                pc = pc + 1;
                int index = mem[pc];
                pc = pc + 1;
                mem[index] = mem[top];
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_PRINT)
            {
                pc = pc + 1;
                AppendTextToTextBox(mem[top].ToString());
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_JUMP)
            {
                pc = mem[pc + 1];
            }

            else if (mem[pc] == INSTRUCTION_JUMPIFFALSE)
            {
                pc = pc + 1;
                if (mem[top] == 0)
                {
                    pc = mem[pc];
                }
                else
                {
                    pc = pc + 1;
                }
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_CALL)
            {
                pc = pc + 1;
                top = top - 1;
                mem[top] = pc + 1;
                pc = mem[pc];
            }
            else if (mem[pc] == INSTRUCTION_RETURN)
            {
                pc = mem[top];
                top = top + 1;
            }

            else if (mem[pc] == INSTRUCTION_UP)
            {
                pc = pc + 1;
                Robot.up();
            }
            else if (mem[pc] == INSTRUCTION_LT)
            {
                pc = pc + 1;
                Robot.left();
            }
            else if (mem[pc] == INSTRUCTION_RT)
            {
                pc = pc + 1;
                Robot.right();
            }
            else if (mem[pc] == INSTRUCTION_DW)
            {
                pc = pc + 1;
                Robot.down();
            }
            else if (mem[pc] == INSTRUCTION_LOW)
            {
                pc = pc + 1;
                int a = mem[top];
                int b = mem[top + 1];
                int answ = 0;
                if(b < a)
                {
                    answ = 1;
                }
                mem[top + 1] = answ;
                top = top + 1;
            }
            else if (mem[pc] == INSTRUCTION_GREAT)
            {
                pc = pc + 1;
                int a = mem[top];
                int b = mem[top + 1];
                int answ = 0;
                if (b > a)
                {
                    answ = 1;
                }
                mem[top + 1] = answ;
                top = top + 1;
            }

            else if (mem[pc] == INSTRUCTION_LOWEQUAL)
            {
                pc = pc + 1;
                int a = mem[top];
                int b = mem[top + 1];
                int answ = 0;
                if (b <= a)
                {
                    answ = 1;
                }
                mem[top + 1] = answ;
                top = top + 1;
            }

            else if (mem[pc] == INSTRUCTION_GREATEQUAL)
            {
                pc = pc + 1;
                int a = mem[top];
                int b = mem[top + 1];
                int answ = 0;
                if (b >= a)
                {
                    answ = 1;
                }
                mem[top + 1] = answ;
                top = top + 1;
            }

            else if (mem[pc] == INSTRUCTION_FREEUP)
            {
                pc = pc + 1;
                int lol = mem[top];
                int answ = ((1.0 * (Robot.position - 1) - Settings.MAP_SQRT_SIZE) / Settings.MAP_SQRT_SIZE) >= 0.0 ? 1 : 0;
                mem[top] = answ;
            }

            else if (mem[pc] == INSTRUCTION_FREEDOWN)
            {
                pc = pc + 1;
                int lol = mem[top];
                int answ = ((1.0 * (Robot.position - 1) + Settings.MAP_SQRT_SIZE) / Settings.MAP_SQRT_SIZE) < 1.0 * (Settings.MAP_SQRT_SIZE) ? 1 : 0;
                mem[top] = answ;
            }

            else if (mem[pc] == INSTRUCTION_FREELEFT)
            {
                pc = pc + 1;
                int lol = mem[top];
                int answ = ((Robot.position - 1) % Settings.MAP_SQRT_SIZE) != 0 ? 1 : 0;
                mem[top] = answ;
            }

            else if (mem[pc] == INSTRUCTION_FREERIGHT)
            {
                pc = pc + 1;
                int lol = mem[top];
                int answ = (Robot.position) % (Settings.MAP_SQRT_SIZE) != 0 ? 1 : 0;
                mem[top] = answ;
            }

            else if (mem[pc] == INSTRUCTION_LOOP)
            {
                pc = pc + 1;
                mem[top]--;
                if (mem[top] > 0)
                {
                    pc = mem[pc];
                }
                else
                {
                    top++;
                    pc++;
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
