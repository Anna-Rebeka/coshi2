using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    internal class SyntaxError : Exception
    {

        public SyntaxError(int line, string code) 
            : base("Riadok " + line + " neznámy príkaz alebo premenná " + code) { }

        public SyntaxError(int line, int expected_kind) 
            : base("Riadok " + line + " očakával som " + castKind(expected_kind)) { }

        public SyntaxError(int line, int expected_kind, string expected_token = null) 
            : base("Riadok " + line + " očakával som " + expected_token) { }

        public static string castKind(int kind)
        {
            switch (kind)
            {
                case 1:
                    return "číslo";
                case 2:
                    return "slovo";
                case 3:
                    return "symbol";
                case 4:
                    return "koniec";
                case 5:
                    return "krát";
                default:
                    return "";
            }
        }
    }

}
