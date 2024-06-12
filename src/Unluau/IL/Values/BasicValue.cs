using System.Text;

namespace Unluau.Decompile.IL.Values
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

        /// <summary>
        /// Returns a string representation of a typed value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The string representation.</returns>
        public static string ToString<T>(T? value)
        {
            if (typeof(T).IsArray && (value is Array array))
            {
                StringBuilder stringBuilder = new();

                stringBuilder.Append('[');

                for (int i = 0; i < array.Length; ++i)
                {
                    var item = array.GetValue(i);

                    if (i > 0)
                        stringBuilder.Append(", ");

                    stringBuilder.Append(ToString(item));
                }

                stringBuilder.Append(']');

                return stringBuilder.ToString();
            }

            if (value is string str)
                return $"\"{str}\"";

            if (value is null)
                return "nil";

            return value.ToString()!;
        }
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
        public override string? ToString() => $"{ToString(value)}";

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
