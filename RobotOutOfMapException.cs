using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    internal class RobotOutOfMapException : Exception
    {
        public RobotOutOfMapException(int line)
            : base("Riadok " + line + " robot mimo mapy.") { }
        public RobotOutOfMapException(string message = "Robot mimo mapy.") : base(message) { }
    }
}
