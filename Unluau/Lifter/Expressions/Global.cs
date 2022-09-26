using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class Global : Expression, IComparable<string>
    {
        public string Value { get; protected set; }

        public Global(string value)
            => Value = value;

        public Global(IList<string> value)
            => Value = string.Join(".", value.ToArray());

        public override void Write(Output output)
        {
            output.Write(Value);
        }

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }
    }
}
