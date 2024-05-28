namespace Unluau.Decompile.IL.Values.Unaries
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
    /// Applies a unary operation to a value.
    /// </summary>
    /// <param name="context">Information on the instruction.</param>
    /// <param name="type">The type of unary operation.</param>
    /// <param name="value">The value.</param>
    public abstract class BasicUnary(Context context, UnaryType type, BasicValue value) : BasicValue(context)
    {
        /// <summary>
        /// The type of unary operation.
        /// </summary>
        public UnaryType Type { get; set; } = type;

        /// <summary>
        /// The value that the unary operation is applied to.
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
        /// Creates a string representation of the <see cref="BasicUnary"/>.
        /// </summary>
        /// <returns>String representation.</returns>
        public override abstract string? ToString();
    }
}
