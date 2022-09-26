using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class NameIndex : Expression
    {
        public Expression Expression { get; set; }
        public string Name { get; set; }

        public NameIndex(Expression expression, string name)
        {
            Expression = expression;
            Name = name;
        }

        public override void Write(Output output)
        {
            Expression.Write(output);
            output.Write("." + Name);
        }
    }
}
