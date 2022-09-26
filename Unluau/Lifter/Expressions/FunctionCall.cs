using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class FunctionCall : Expression
    {
        public Expression Function { get; protected set; }
        public IList<Expression> Arguments { get; protected set; }

        public FunctionCall(Expression function, IList<Expression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public override void Write(Output output)
        {
            Function.Write(output);
            output.Write("(");

            bool first = true;
            foreach (Expression argument in Arguments)
            {
                if (!first)
                    output.Write(",");
                else
                    first = false;

                if (argument != null)
                    argument.Write(output);
            }

            output.Write(")");
        }
    }
}
