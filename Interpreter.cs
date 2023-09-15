



using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace coshi2
{
    public class Interpreter
    {
        private string input;
        private int index;
        private char look;
        private char KONIEC = '\0';
        private int NOTHING = 0;
        private int NUMBER = 1;
        private int WORD = 2;
        private int SYMBOL = 3;
        private string token;
        private int kind;
        private int position;
        
        private int lineNumber;
        private TextBox terminal;
        private int map_size;

        public int[] robot_pos = new int[2];
        public List<int[]> positions;

        public Interpreter(TextBox textbox, int map_size) { 
            this.terminal = textbox;
            this.map_size = map_size;
            this.reset();
        }

        public List<int[]> get_positions()
        {
            return positions;
        }

        public void load(string usercode)
        {
            this.reset();
            this.input = usercode;
            this.next();
            this.scan();
            this.interpret();
        }

        public void reset()
        {
            this.index = 0;
            this.look = '\0';
            this.token = "";
            this.kind = this.WORD;
            this.position = 0;
            this.robot_pos = new int[] { 0, 0 };
            this.positions = new List<int[]> { new int[] { 0, 0 } };
            this.lineNumber = 1;
        }

        public void setMap_size(int map_size) {
            this.map_size = map_size;
        }

        public void next()
        {
            if (this.index >= this.input.Length)
            {
                this.look = this.KONIEC;
            }
            else
            {
                this.look = this.input[this.index];
                this.index++;
            }
        }

        public void vypis_prikaz()
        {
            while (this.look != this.KONIEC)
            {
                Console.WriteLine(this.look);
                this.next();
            }
            this.reset();
        }

        public void scan()
        {

          

            while(char.IsWhiteSpace(look))
            {
                if (look == '\n') {
                    this.lineNumber += 1;
                }
                next();
            }

            //while (this.look == ' ' || this.look == '\n')
            //{
            //    this.next();
            //}
            this.token = "";
            this.position = this.index - 1;
            if (Char.IsNumber(this.look))
            {
                while (Char.IsNumber(this.look))
                {
                    this.token += this.look;
                    this.next();
                }
                this.kind = this.NUMBER;
            }
            else if (Char.IsLetter(this.look))
            {
                while (Char.IsLetter(this.look))
                {
                    this.token += this.look;
                    this.next();
                }
                this.kind = this.WORD;
            }
            else if (this.look != this.KONIEC)
            {
                this.token = this.look.ToString();
                this.next();
                this.kind = this.SYMBOL;
            }
            else
            {
                this.kind = this.NOTHING;
            }
        }

        public void vypis_lexemy()
        {
            while (this.kind != this.NOTHING)
            {
                Console.WriteLine($"pos= {this.position}, kind= {this.kind}, token= {this.token}");
                this.scan();
            }
            this.reset();
        }

        public void interpret()
        {
            //this.handler.soundsHandler.play_sound(0,0);
            //Thread.Sleep(1000);
            while (this.kind == this.WORD)
            {
                if (this.token == "hore" || this.token == "ho")
                {
                    //pohni robota ak padlo skonci
                    //this.handler.moveUpRobot();
                    if (this.robot_pos[0] - 1 < 0) {
                        terminal.Text = "Chyba na riadku [" + lineNumber.ToString() + "]. Robot mimo plochy.";
                    }
                    this.robot_pos[0] -= 1;
                    this.positions.Add(new int[] { this.robot_pos[0], this.robot_pos[1] });
                    this.scan();
                }
                else if (this.token == "vlavo" || this.token == "vl")
                {
                    //pohni robota ak padlo skonci
                    if (this.robot_pos[1] - 1 < 0)
                    {
                        terminal.Text = "Chyba na riadku [" + lineNumber.ToString() + "]. Robot mimo plochy.";
                    }
                    this.robot_pos[1] -= 1;
                    this.positions.Add(new int[] { this.robot_pos[0], this.robot_pos[1] });
                    this.scan();
                }
                else if (this.token == "vpravo" || this.token == "vp")
                {
                    if (this.robot_pos[1] + 1 >= map_size)
                    {
                        terminal.Text = "Chyba na riadku [" + lineNumber.ToString() + "]. Robot mimo plochy.";
                    }
                    this.robot_pos[1] += 1;
                    this.positions.Add(new int[] { this.robot_pos[0], this.robot_pos[1] });
                    this.scan();
                }
                else if (this.token == "dole" || this.token == "do")
                {
                    //pohni robota ak padlo skonci
                    if (this.robot_pos[0] + 1 >= map_size)
                    {
                        terminal.Text = "Chyba na riadku [" + lineNumber.ToString() + "]. Robot mimo plochy.";
                    }
                    this.robot_pos[0] += 1;
                    this.positions.Add(new int[] { this.robot_pos[0], this.robot_pos[1] });
                    this.scan();
                }
                else if (token == "opakuj" || token == "op")
                {
                    this.scan();
                    float count = float.Parse(token);
                    this.scan();
                    if (token == "krát")
                    {
                        this.scan();
                        int start = position;
                        while (count > 0)
                        {
                            index = start;
                            this.next();
                            this.scan();
                            this.interpret();
                            count--;
                        }
                    }
                    if (token == "koniec")
                    {
                        this.scan();
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
