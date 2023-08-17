using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coshi2
{
    internal class RobotOutOfMapException : Exception
    {
       public RobotOutOfMapException(string message = "Robot mimo mapy.") : base(message) { }
    }
}
