using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    internal class VariableNotFoundException : Exception
    {

        public VariableNotFoundException(int line, string variableName) 
            : base("Riadok " + line + " neznáma premenná " + variableName) { }


    }

}
