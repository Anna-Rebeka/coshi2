using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace coshi2
{
    public class Compiler
    {
        private LexicalAnalyzer lexAnalyzer;
        private int robot_position = 0;

        public Compiler(string code) 
        {
            robot_position = 0;
            lexAnalyzer = new LexicalAnalyzer();
            lexAnalyzer.initialize(code);
        }

     

        /// <summary>
        /// Procedure fills memory of virtual machine with instructions.
		/// Consider using Parse() to create Syntax tree
        /// </summary>
        public Block parse(int map_size)
        {
            Block result = new Block(); //... vytvorí objekt pre zoznam príkazov
            while (lexAnalyzer.kind == lexAnalyzer.WORD)
            {
                if (lexAnalyzer.token == "hore") //...vytvorí vrchol stromu pre príkaz dopredu
                {
                    if (robot_position / map_size == 0)
                    {
                        int line = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                        throw new RobotOutOfMapException(line);   
                    }
                    lexAnalyzer.scan();
                    result.add(new Up());
                    robot_position -= map_size;
                }

                else if ("vlavo" == lexAnalyzer.token)
                {
                    if (robot_position % map_size == 0)
                    {
                        int line = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                        throw new RobotOutOfMapException(line);
                    }
                    lexAnalyzer.scan();
                    result.add(new Lt());
                    robot_position -= 1;
                }

                else if ("vpravo" == lexAnalyzer.token)
                {
                    if ((robot_position + 1) % map_size == 0)
                    {
                        int line = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                        throw new RobotOutOfMapException(line);
                    }
                    lexAnalyzer.scan();
                    result.add(new Rt());
                    robot_position += 1;
                }
                else if ("dole" == lexAnalyzer.token)
                {
                    if (robot_position / map_size == map_size - 1)
                    {
                        int line = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                        throw new RobotOutOfMapException(line);
                    }
                    lexAnalyzer.scan();
                    result.add(new Dw());
                    robot_position += map_size;
                }
                else if ("opakuj" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    int n = int.Parse(lexAnalyzer.token);
                    lexAnalyzer.scan();
                    // Parsovanie vnútorného bloku kódu pre opakovanie
                    Block innerBlock = parse(map_size);
                    result.add(new Repeat(new Const(n), innerBlock)); // Pridáme vrchol stromu pre opakovanie
                    if(lexAnalyzer.token != "koniec")
                    {
                        throw new SyntaxErrorException(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), "chýba koniec");
                    }
                    lexAnalyzer.scan(); // Preskočíme "koniec" 

                }
              
                else
                {
                    if (lexAnalyzer.token != "koniec")
                    {
                        throw new SyntaxErrorException(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), lexAnalyzer.token);
                    }
                    break;
                }
            }
            return result;
        }






        /*
         public Block parse(int map_size)
        {
            int robot_position = 0;

            Block result = new Block(); //... vytvorí objekt pre zoznam príkazov
            while (lexAnalyzer.kind == lexAnalyzer.WORD)
            {
                if (lexAnalyzer.token == "hore") //...vytvorí vrchol stromu pre príkaz dopredu
                {
    
                    lexAnalyzer.scan();
                    result.add(new Up());
                    robot_position -= map_size;
                }

                else if ("vlavo" == lexAnalyzer.token)
                {
                    ...
                }

                else if ("vpravo" == lexAnalyzer.token)
                {
                   ...
                }
                else if ("dole" == lexAnalyzer.token)
                {
                    ...
                }
                else if ("opakuj" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    int n = int.Parse(lexAnalyzer.token);
                    lexAnalyzer.scan(); 
                    result.add(new Repeat(new Const(n), parse(map_size)));
                    lexAnalyzer.scan();
                    lexAnalyzer.scan();
                    MessageBox.Show(lexAnalyzer.token);
                }

                else
                {
                    throw new SyntaxErrorException(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), lexAnalyzer.token);
                }
            }
            return result;
        }
         */





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
