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
        public bool iffing = false;

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
                    lexAnalyzer.scan(); // Preskočíme "cislo" 
                    check(lexAnalyzer.LOOP);
                    lexAnalyzer.scan(); // Preskočíme "krát" 
                    // Parsovanie vnútorného bloku kódu pre opakovanie
                    Block innerBlock = parse(map_size);
                    result.add(new Repeat(new Const(n), innerBlock)); // Pridáme vrchol stromu pre opakovanie
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan(); // Preskočíme "koniec" 
                }

                else if ("ku" == lexAnalyzer.token || "od" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    string name = lexAnalyzer.token;
                    result.add(new Assign(new Variable(name), addSub()));
                }

                else if ("kym" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    //skontroluj ci ide "je volne", potom zavolaj free(), inak ries inak
                    Syntax test = expr();
                    result.add(new While(test, parse(map_size)));
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan();
                }

                else if ("ak" == lexAnalyzer.token)
                {
                    iffing = true;
                    lexAnalyzer.scan();
                    Syntax test = expr();
                    check(lexAnalyzer.WORD, "tak");
                    lexAnalyzer.scan();
                    IfElse ifelse = new IfElse(test, parse(map_size), null);
                    
                    if (lexAnalyzer.token == "inak")
                    {
                        lexAnalyzer.scan();
                        ifelse.bodyfalse = parse(map_size);
                    }
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan();
                    result.add(ifelse);
                    iffing = false;
                }

                else if ("vypis" == lexAnalyzer.token)
                {
                    lexAnalyzer.scan();
                    result.add(new Print(addSub()));
                }


                else
                {
                    if (lexAnalyzer.token == "koniec" || lexAnalyzer.token == "inak" && iffing) { return result; }
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
            if (expected_token != null && lexAnalyzer.token != expected_token)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind, expected_token);
            }
            if (expected_kind == lexAnalyzer.END && lexAnalyzer.token == "koniec" ||
                expected_kind == lexAnalyzer.LOOP && new List<string> { "krát", "krat" }.Contains(lexAnalyzer.token))
            {
                return;
            }
            if (lexAnalyzer.kind != expected_kind)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind);
            }
        }

        public void check(int expected_kind, string[] expected_tokens)
        {
            if (!expected_tokens.Contains(lexAnalyzer.token))
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind, expected_tokens[0]);
            }
            if (lexAnalyzer.kind != expected_kind)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind);
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
                    throw new VariableNotFoundException(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), name);
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

        public Syntax compare() { 
            var result = addSub();

            if ("menší" == lexAnalyzer.token || "mensi" == lexAnalyzer.token) {
                lexAnalyzer.scan();
                if (lexAnalyzer.token == "alebo")
                {
                    lexAnalyzer.scan();
                    check(lexAnalyzer.WORD, new string[] { "rovný", "rovny" });
                    lexAnalyzer.scan();
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    result = new LowEqual(result, addSub());
                }
                else
                {
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    result = new Lower(result, addSub());
                }
            }
            else if ("väčší" == lexAnalyzer.token || "vacsi" == lexAnalyzer.token)
            {
                lexAnalyzer.scan();
                if (lexAnalyzer.token == "alebo")
                {
                    lexAnalyzer.scan();
                    check(lexAnalyzer.WORD, new string[] { "rovný", "rovny" });
                    lexAnalyzer.scan();
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    result = new GreatEqual(result, addSub());
                }
                else
                {
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    result = new Greater(result, addSub());
                }
            }
            else if ("rovný" == lexAnalyzer.token || "rovny" == lexAnalyzer.token)
            {
                lexAnalyzer.scan();
                result = new Equal(result, addSub());
            }



            return result;
        }


    public Syntax expr()
        {
            var result = compare();
            
            return result;
        }


        public void jumpOverVariables()
        {
            int count = VirtualMachine.variables.Count;
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMP);
            VirtualMachine.poke(count + 2);
            VirtualMachine.adr += count;
        }
   }
}
