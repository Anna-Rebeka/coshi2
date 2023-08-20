using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    internal class SyntaxErrorException : Exception
    {

        public SyntaxErrorException(int line, string code) 
            : base("Riadok " + line + " neznámy príkaz " + code) { }
    }
}
