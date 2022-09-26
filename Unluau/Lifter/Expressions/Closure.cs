using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Closure : Expression
    {
        public IList<Local> Arguments { get; protected set; }
        public bool HasVararg { get; protected set; }
        public Block Block { get; protected set; }

        public Closure(IList<Local> arguments, bool hasVararg, Block block)
        {
            Arguments = arguments;
            HasVararg = hasVararg;
            Block = block;
        }

        public override void Write(Output output)
        {
            output.Write("(");

            bool first = true;
            foreach (Local argument in Arguments)
            {
                if (!first)
                    output.Write(", ");
                else
                    first = false;

                output.Write(argument.Name);
            }

            if (HasVararg)
                output.Write(", ...");

            output.Write(")");

            Block.Write(output);

            output.WriteLine("end");
        }
    }
}
