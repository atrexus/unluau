using Unluau.Utils;

namespace Unluau.IL.Values
{
    /// <summary>
    /// The types of unary operations.
    /// </summary>
    public enum UnaryType
    { 
        /// <summary>
        /// Equivalent to Luau's `not` keyword.
        /// </summary>
        Not,
        /// <summary>
        /// Represents `-` operator
        /// </summary>
        Minus,
        /// <summary>
        /// Represents the `#` operator.
        /// </summary>
        Length
    }

    /// <summary>
    /// Applies unary operation to a register.
    /// </summary>
    /// <param name="context">Information about the location.</param>
    /// <param name="type">The type of unary operation.</param>
    /// <param name="value">The register that the unary operation is applied to.</param>
    public class Unary(Context context, UnaryType type, BasicValue value) : BasicValue(context)
    {
        /// <summary>
        /// The type of unary operation.
        /// </summary>
        public UnaryType Type { get; set; } = type;

        /// <summary>
        /// The register that the unary operation is applied to.
        /// </summary>
        public BasicValue Value { get; set; } = value;

        /// <summary>
        /// Implements the visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Value.Visit(visitor);
            }
        }

        /// <summary>
        /// Converts the current <see cref="Unary"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString() => $"{Enum.GetName(typeof(UnaryType), Type)}({TypeExtensions.ToString(Value)})";
    }
}
