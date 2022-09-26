using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Assignment : Statement
    {
        public Expression Variable { get; private set; }
        public Expression Value { get; private set; }

        public Assignment(Expression variable, Expression value)
        {
            Variable = variable;
            Value = value;
        }

        public override void Write(Output output)
        {
            Variable.Write(output);
            output.Write(" = ");
            Value.Write(output);
        }
    }
}
