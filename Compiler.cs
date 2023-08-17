using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace coshi2
{
    public class Compiler
    {
        private LexicalAnalyzer lexAnalyzer;

        public Compiler(string code) 
        {
            lexAnalyzer = new LexicalAnalyzer();
            lexAnalyzer.initialize(code);
        }

     

        /// <summary>
        /// Procedure fills memory of virtual machine with instructions.
		/// Consider using Parse() to create Syntax tree
        /// </summary>
        public Block parse(int map_size)
        {
            int robot_position = 1;

            Block result = new Block(); //... vytvorí objekt pre zoznam príkazov
            while (lexAnalyzer.kind == lexAnalyzer.WORD)
            {
                if (lexAnalyzer.token == "hore") //...vytvorí vrchol stromu pre príkaz dopredu
                {
                    if (robot_position / map_size == 0)
                    {
                        int line = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                        throw new RobotOutOfMapException("Robot mimo mapy na riadku " + line);   
                    }
                    lexAnalyzer.scan();
                    result.add(new Up());
                    robot_position += map_size;
                }

                else if ("vlavo" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    result.add(new Lt());
                }

                else if ("vpravo" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    result.add(new Rt());
                }
                else if ("dole" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    result.add(new Dw());
                }
                else if ("opakuj" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    int n = int.Parse(lexAnalyzer.token);
                    lexAnalyzer.scan(); 
                    result.add(new Repeat(new Const(n), parse(map_size)));
                }
              
                else
                {
                    break;
                }
            }
            return result;
        }

       
        /*
        
        public void JumpToProgramBody()
        {
            int offset = VirtualMachine.Variables.Count;
            Poke((int)Instruction.Jmp);
            Poke(2 + offset);
            VirtualMachine.adr += offset;
        }

        public Syntax Operand()
        {
            Syntax result;
            if (lexAnalyzer.kind == Kind.WORD)
            {
                string name = lexAnalyzer.ToString();
                if (!VirtualMachine.Variables.ContainsKey(name))
                {
                    throw new System.Collections.Generic.KeyNotFoundException($"{name} nie je zadeklarovane");
                }
                result = new Variable(name);
            }
            else
            {
                lexAnalyzer.Check(Kind.NUMBER);
                result = new Const(System.Convert.ToInt32(lexAnalyzer.ToString()));
            }
            Scan();
            return result;
        }

        public Syntax MulDiv()
        {
            var result = Operand();
            while ("*" == lexAnalyzer.ToString() || "/" == lexAnalyzer.ToString())
            {
                if ("*" == lexAnalyzer.ToString())
                {
                    Scan();
                    result = new Mul(result, AddSub());
                }
                else if ("/" == lexAnalyzer.ToString())
                {
                    Scan();
                    result = new Div(result, AddSub());
                }
            }
            return result;
        }

        public Syntax AddSub()
        {
            var result = MulDiv();
            while ("+" == lexAnalyzer.ToString() || "-" == lexAnalyzer.ToString())
            {
                if ("+" == lexAnalyzer.ToString())
                {
                    Scan();
                    result = new Add(result, AddSub());
                }
                else if ("-" == lexAnalyzer.ToString())
                {
                    Scan();
                    result = new Sub(result, AddSub());
                }
            }
            return result;
        }

    }
        */
    }


}
