using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    int n = int.Parse(lexAnalyzer.token); //addsub()
                    check(lexAnalyzer.LOOP);
                    lexAnalyzer.scan(); // Preskočíme "krát" 
                    // Parsovanie vnútorného bloku kódu pre opakovanie
                    Block innerBlock = parse(map_size);
                    result.add(new Repeat(new Const(n), innerBlock)); // Pridáme vrchol stromu pre opakovanie
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan(); // Preskočíme "koniec" 
                }

                else if ("ku" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    string name = lexAnalyzer.token;
                    result.add(new Assign(new Variable(name), addSub()));
                }

                else if ("vypis" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    result.add(new Print(addSub()));
                }

                else
                {
                    if (lexAnalyzer.token == "koniec") { }
                    else
                    {
                        check(lexAnalyzer.WORD, "do");
                        lexAnalyzer.scan();
                        string name = lexAnalyzer.token;
                        lexAnalyzer.scan();
                        check(lexAnalyzer.WORD, "daj");
                        lexAnalyzer.scan();
                        result.add(new Assign(new Variable(name), addSub()));
                        
                        if (!VirtualMachine.variables.ContainsKey(name)) 
                        {
                            VirtualMachine.variables[name] = 2 + VirtualMachine.variables.Count;
                        }
                    }
                }
            }
            return result;
        }


        public void check(int expected_kind, string expected_token = null)
        {
            if (expected_kind == lexAnalyzer.END && lexAnalyzer.token == "koniec" ||
                expected_kind == lexAnalyzer.LOOP && new List<string> { "krát", "krat" }.Contains(lexAnalyzer.token))
            {
                return;
            }
            if (lexAnalyzer.kind != expected_kind)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind);
            }
            if (expected_token != null && lexAnalyzer.token != expected_token)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind, expected_token);
            }
        }


        
           public Syntax operand()
        {
            Syntax result;
            if (lexAnalyzer.kind == lexAnalyzer.WORD)
            {
                string name = lexAnalyzer.token;
                if (!VirtualMachine.variables.ContainsKey(name))
                {
                    MessageBox.Show("Nepoznam premennu");
                }
                result = new Variable(name);
            }
            else
            {
                check(lexAnalyzer.NUMBER);
                result = new Const(System.Convert.ToInt32(lexAnalyzer.token));
            }
            lexAnalyzer.scan();
            return result;
        }
         
        /*
        public int number()
        {
            //check(lexAnalyzer.NUMBER);
            int result = int.Parse(lexAnalyzer.token);
            lexAnalyzer.scan();
            return result;
        }
        */

        
         public Syntax addSub()
        {
            var result = operand();
            while ("+" == lexAnalyzer.token || lexAnalyzer.token == "pričítaj" || 
                   "-" == lexAnalyzer.token || lexAnalyzer.token == "odčítaj" || lexAnalyzer.token == "pricitaj" || lexAnalyzer.token == "odcitaj")
            {
                if (lexAnalyzer.token == "pričítaj" || lexAnalyzer.token == "pricitaj"  || lexAnalyzer.token == "+")
                {
                    lexAnalyzer.scan();
                    result = new Add(result, addSub());
                }
                else if (lexAnalyzer.token == "odčítaj" || lexAnalyzer.token == "odcitaj" || lexAnalyzer.token == "-")
                {
                    lexAnalyzer.scan();
                    result = new Sub(result, addSub());
                }
            }
            return result;
        }
         
        /*
        public int addsub()
        {
            int result = number();
            while (true)
            {
                if (lexAnalyzer.token == "pričítaj" || lexAnalyzer.token == "+")
                {
                    lexAnalyzer.scan();
                    result = result + number();
                }
                else if (lexAnalyzer.token == "odčítaj" || lexAnalyzer.token == "-")
                {
                    lexAnalyzer.scan();
                    result = result - number();
                }
                else { break; }
            }
            return result;
        }
        */

        public void jumpOverVariables()
        {
            int count = VirtualMachine.variables.Count;
            //Poke((int)Instruction.Jmp);
            //Poke(2 + offset);
            VirtualMachine.adr += count;
        }
   }


/*

      

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

        

    }
       
    }
*/

}
