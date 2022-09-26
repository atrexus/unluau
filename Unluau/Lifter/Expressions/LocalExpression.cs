using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalExpression : Expression
    {
        public Expression Expression { get; private set; }
        public Decleration Decleration { get; set; }

        public LocalExpression(Expression expression, Decleration decleration)
        {
            Expression = expression;
            Decleration = decleration;
        }

        public override void Write(Output output)
        {
            if (Decleration.Referenced >= 1)
                output.Write(Decleration.Name);
            else
                Expression.Write(output);
        }
    }
}
