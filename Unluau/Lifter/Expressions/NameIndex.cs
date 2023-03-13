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
        public bool IsSelf { get; set; }

        public NameIndex(Expression expression, string name, bool isSelf = false)
        {
            Expression = expression;
            Name = name;
            IsSelf = isSelf;
        }

        public override void Write(Output output)
        {
            Expression.Write(output);
            output.Write((IsSelf ? ":" : ".") + Name);
        }

        public override string[] GetNames()
        {
            return Expression.GetNames().Append(Name).ToArray();
        }
    }
}
