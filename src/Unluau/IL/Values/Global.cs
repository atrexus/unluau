using System.Reflection;

namespace Unluau.Decompile.IL.Values
{
    /// <summary>
    /// Represents a global/imported value.
    /// </summary>
    /// <param name="context">Information about the value.</param>
    /// <param name="value">The name of the global.</param>
    public class Global(Context context, string[] value) : BasicValue<string[]>(context, value)
    {
        /// <summary>
        /// Implements the recursive visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Converts the current <see cref="Global"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() => $"global({ToString(Value)})";
    }
}
