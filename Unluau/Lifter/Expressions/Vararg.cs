using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Vararg : Expression
    {
        public override void Write(Output output)
        {
            output.Write("...");
        }
    }
}
