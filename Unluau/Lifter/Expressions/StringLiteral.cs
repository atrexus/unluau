using System;

namespace Unluau
{
    public class StringLiteral : Expression, IComparable<string>
    {
        public string Value { get; protected set; }

        public StringLiteral(string value)
            => Value = value;

        public override void Write(Output output)
        {
            output.Write($"\"{Value}\"");
        }

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }
    }
}
