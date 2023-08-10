using System;
using System.Windows.Controls;

namespace coshi2
{
    internal class LexicalAnalyzer
    {
        public char KONIEC = '\0';

        public int NOTHING = 0;
        public int NUMBER = 1;
        public int WORD = 2;
        public int SYMBOL = 3;

        public string token;
        public int kind;
        public int position;

        public string input;
        public int index;
        public char look;


        //prebrane z Interpreter.cs
        private int lineNumber;
        private TextBox terminal;
        private int map_size;

        public void initialize(string code)
        {
            input = code;
            index = 0;
            next();
            scan();
        }

        public void scan()
        {
            while (char.IsWhiteSpace(look))
            {
                if (look == '\n')
                {
                    lineNumber += 1;
                }
                next();
            }

            token = "";
            position = index - 1;
            if (Char.IsNumber(look))
            {
                while (Char.IsNumber(look))
                {
                    token += look;
                    next();
                }
                kind = NUMBER;
            }
            else if (Char.IsLetter(look))
            {
                while (Char.IsLetter(look))
                {
                    token += look;
                    next();
                }
                kind = WORD;
            }
            else if (look != KONIEC)
            {
                token = look.ToString();
                next();
                kind = SYMBOL;
            }
            else
            {
                kind = NOTHING;
            }
        }


        public void next()
        {
            if (index >= input.Length)
            {
                look = KONIEC;
            }
            else
            {
                look = input[index];
                index++;
            }
        }


    }
}
