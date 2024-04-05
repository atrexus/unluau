using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// Concatenates values in registers.
    /// </summary>
    /// <param name="context">Information about the location.</param>
    /// <param name="values">The values that are concatenated together.</param>
    public class Concat(Context context, BasicValue[] values) : BasicValue(context)
    {
        /// <summary>
        /// The registers that are concatenated together.
        /// </summary>
        public BasicValue[] Values { get; set; } = values;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                foreach (var value in Values)
                {
                    value.Visit(visitor);
                }
            }
        }

        /// <summary>
        /// Converts the current <see cref="Concat"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() => $"Concat({TypeExtensions.ToString(Values)})";
    }
}
