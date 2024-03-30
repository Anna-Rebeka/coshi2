using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace coshi2
{
    public class Compiler
    {
        private LexicalAnalyzer lexAnalyzer;
        private int robot_position;
        public bool iffing = false;

        public Compiler(string code)
        {
            robot_position = 0;
            lexAnalyzer = new LexicalAnalyzer();
            lexAnalyzer.initialize(code);
        }



        /// Procedure fills memory of virtual machine with instructions.
        public Block parse()
        {
            Block result = new Block(); //... vytvorí objekt pre zoznam príkazov
            while (lexAnalyzer.kind == lexAnalyzer.WORD)
            {
                if (lexAnalyzer.token == "hore".ToLower()) //...vytvorí vrchol stromu pre príkaz dopredu
                {
                    lexAnalyzer.scan();
                    result.add(new Up());
                    robot_position -= Settings.MAP_SQRT_SIZE;
                }

                else if ("vlavo" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Lt());
                    robot_position -= 1;
                }

                else if ("vpravo" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Rt());
                    robot_position += 1;
                }
                else if ("dole" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Dw());
                    robot_position += Settings.MAP_SQRT_SIZE;
                }
                else if ("opakuj" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    check(lexAnalyzer.NUMBER);
                    int n = int.Parse(lexAnalyzer.token); //addsub()
                    int opakuj_line_number = lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position);
                    lexAnalyzer.scan(); // Preskočíme "cislo" 
                    check(lexAnalyzer.LOOP, null, opakuj_line_number);
                    lexAnalyzer.scan(); // Preskočíme "krát" 
                    // Parsovanie vnútorného bloku kódu pre opakovanie
                    Block innerBlock = parse();
                    result.add(new Repeat(new Const(n), innerBlock)); // Pridáme vrchol stromu pre opakovanie
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan(); // Preskočíme "koniec" 
                }

                else if ("ku" == lexAnalyzer.token.ToLower() || "od" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    string name = lexAnalyzer.token;
                    result.add(new Assign(new Variable(name), addSub()));
                }

                else if ("kym" == lexAnalyzer.token.ToLower() || "kým" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    Syntax test = expr();
                    result.add(new While(test, parse()));
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan();
                }

                else if ("ak" == lexAnalyzer.token.ToLower())
                {
                    iffing = true;
                    lexAnalyzer.scan();
                    Syntax test = expr();
                    check(lexAnalyzer.WORD, "tak");
                    lexAnalyzer.scan();
                    IfElse ifelse = new IfElse(test, parse(), null);
                    
                    if (lexAnalyzer.token.ToLower() == "inak")
                    {
                        lexAnalyzer.scan();
                        ifelse.bodyfalse = parse();
                    }
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan();
                    result.add(ifelse);
                    iffing = false;
                }

                else if ("zobraz" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Print(addSub()));
                }

                else if ("ticho" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Silence());
                }

                else if ("nahlas" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    result.add(new Loud());
                }

                else if ("urob" == lexAnalyzer.token.ToLower())
                {
                    lexAnalyzer.scan();
                    check(lexAnalyzer.WORD);
                    string name = lexAnalyzer.token;
                    if (VirtualMachine.subroutines.ContainsKey(name) || VirtualMachine.variables.ContainsKey(name))
                    {
                        throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), name, name);
                    }
                    lexAnalyzer.scan();
                    Subroutine subr = new Subroutine(name, null);
                    VirtualMachine.subroutines[name] = subr;
                    subr.body = parse();
                    check(lexAnalyzer.END);
                    lexAnalyzer.scan();
                    result.add(subr);
                }


                else
                {
                    if (lexAnalyzer.token.ToLower() == "koniec" || lexAnalyzer.token.ToLower() == "inak" && iffing) { return result; }
                    else
                    {
                        if (lexAnalyzer.token.ToLower() == "do")
                        {
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
                        else {
                            string name = lexAnalyzer.token;
                            if (!VirtualMachine.subroutines.ContainsKey(name))
                            {
                                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), name);
                            }
                            result.add(new Call(name));
                            lexAnalyzer.scan();
                        }
                        
                    }
                }
            }
            return result;
        }


        public void check(int expected_kind, string expected_token = null, int opakuj_pos = 0)
        {
            if (expected_token != null && lexAnalyzer.token.ToLower() != expected_token)
            {
                throw new SyntaxError(lexAnalyzer.CalculateLineNumberOfError(lexAnalyzer.position), expected_kind, expected_token);
            }
            if (expected_kind == lexAnalyzer.END && lexAnalyzer.token.ToLower() == "koniec" ||
                expected_kind == lexAnalyzer.LOOP && new List<string> { "krát", "krat" }.Contains(lexAnalyzer.token.ToLower()))
            {
                return;
            }
            if (lexAnalyzer.kind != expected_kind)
            {
                if(expected_kind == lexAnalyzer.LOOP)
                {
                    throw new SyntaxError(opakuj_pos, expected_kind);
                }
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

        public Syntax compare(bool neg) { 
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
                    if (neg)
                    {
                        result = new Greater(result, addSub());
                    }
                    else
                    {
                        result = new LowEqual(result, addSub());
                    }
                }
                else
                {
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    if (neg)
                    {
                        result = new GreatEqual(result, addSub());
                    }
                    else
                    {
                        result = new Lower(result, addSub());
                    }
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
                    if (neg)
                    {
                        result = new Lower(result, addSub());
                    }
                    else
                    {
                        result = new GreatEqual(result, addSub());
                    }
                }
                else
                {
                    check(lexAnalyzer.WORD, "ako");
                    lexAnalyzer.scan();
                    if (neg)
                    {
                        result = new LowEqual(result, addSub());
                    }
                    else
                    {
                        result = new Greater(result, addSub());
                    }
                }
            }
            else if ("rovný" == lexAnalyzer.token || "rovny" == lexAnalyzer.token)
            {
                lexAnalyzer.scan();
                if (neg)
                {
                    result = new NotEqual(result, addSub());
                }
                else
                {
                    result = new Equal(result, addSub());
                }
            }
            return result;
        }


        public Syntax expr()
        {
            var result = new Syntax();
            bool neg = false;
            if (lexAnalyzer.token.ToLower() == "nie")
            {
                neg = true;
                lexAnalyzer.scan();
            }
            lexAnalyzer.scan();

            if ("volne" == lexAnalyzer.token || "voľné" == lexAnalyzer.token)
            {
                lexAnalyzer.scan();
                string smer = lexAnalyzer.token;

                switch (smer)
                {
                    case "hore":
                        result = new FreeUp();
                        lexAnalyzer.scan();
                        break;
                    case "dole":
                        result = new FreeDown();
                        lexAnalyzer.scan();
                        break;
                    case "vlavo":
                    case "vľavo":
                        result = new FreeLeft();
                        lexAnalyzer.scan();
                        break;
                    case "vpravo":
                        result = new FreeRight();
                        lexAnalyzer.scan();
                        break;

                    default:
                        break;
                }
            }
            else if ("zvuk" == lexAnalyzer.token.ToLower()) {                
                lexAnalyzer.scan();
                string name = lexAnalyzer.token.ToLower();
                if (!SoundsHandler.sound_codes.ContainsKey(name)) {
                    if (neg)
                    {
                        result = new IsNotSound(-1);
                    }
                    else
                    {
                        result = new IsSound(-1);
                    }
                    
                }
                else {
                    if (neg)
                    {
                        result = new IsNotSound(SoundsHandler.sound_codes[name]);
                    }
                    else
                    {
                        result = new IsSound(SoundsHandler.sound_codes[name]);
                    }
                }
                lexAnalyzer.scan();
            }
            else
            {
                result = compare(neg);
            }

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
