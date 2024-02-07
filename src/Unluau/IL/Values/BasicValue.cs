using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// A basic value template in the IL. Is only to be inherited.
    /// </summary>
    /// <param name="context">Additional information about the value.</param>
    public abstract class BasicValue(Context context) : Node(context)
    {
        /// <summary>
        /// Converts the current <see cref="BasicValue"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override abstract string? ToString();
    }

    /// <summary>
    /// A basic value in the IL. Loaded by instructions.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="context">Additional context about the value.</param>
    /// <param name="value">The value.</param>
    public class BasicValue<T>(Context context, T? value) : BasicValue(context)
    {
        /// <summary>
        /// The value. If its empty, then its equivalent to a null value.
        /// </summary>
        public T? Value { get; private set; } = value;

        /// <summary>
        /// Converts the current <see cref="BasicValue{T}"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string? ToString() => $"Val<{typeof(T).Name}>({TypeExtentions.ToString(Value)})";

        /// <summary>
        /// Implements the recursive visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
