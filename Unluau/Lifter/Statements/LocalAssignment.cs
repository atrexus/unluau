using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalAssignment : Statement
    {
        public LocalExpression Variable { get; private set; }

        public LocalAssignment(LocalExpression variable)
            => Variable = variable;

        public override void Write(Output output)
        {
            output.Write("local ");
            Variable.Write(output);

            output.Write(" = ");
            Variable.Expression.Write(output);
        }
    }
}
