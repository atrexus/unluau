namespace Unluau.IL.Values.Binaries
{
    /// <summary>
    /// Describes the type of binary operation.
    /// </summary>
    public enum BinaryType
    {
        /// <summary>
        /// Represents the addition operation.
        /// </summary>
        Add,

        /// <summary>
        /// Represents the subtraction operation.
        /// </summary>
        Subtract,

        /// <summary>
        /// Represents the multiplication operation.
        /// </summary>
        Multiply,

        /// <summary>
        /// Represents the division operation.
        /// </summary>
        Divide,

        /// <summary>
        /// Represents the modulus operation.
        /// </summary>
        Modulus,

        /// <summary>
        /// Represents the exponentiation operation.
        /// </summary>
        Power,

        /// <summary>
        /// The logical AND operation.
        /// </summary>
        And,

        /// <summary>
        /// The logical OR operation.
        /// </summary>
        Or,

        /// <summary>
        /// The logical floor division (//) operation.
        /// </summary>
        FloorDivide,
    }

    /// <summary>
    /// Represents a binary operation in the IL.
    /// </summary>
    /// <param name="context">Information about the instruction.</param>
    /// <param name="left">The left value.</param>
    /// <param name="type">The type of binary operation.</param>
    /// <param name="right">The right value.</param>
    public abstract class BasicBinary(Context context, BasicValue left, BinaryType type, BasicValue right) : BasicValue(context)
    {
        /// <summary>
        /// The left operand in the binary operation.
        /// </summary>
        public BasicValue Left { get; set; } = left;

        /// <summary>
        /// The type of binary operation.
        /// </summary>
        public BinaryType Type { get; set; } = type;

        /// <summary>
        /// The right operand in the binary operation.
        /// </summary>
        public BasicValue Right { get; set; } = right;

        /// <summary>
        /// Implements the visitor pattern.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Visit(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Left.Visit(visitor);
                Right.Visit(visitor);
            }
        }

        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override abstract string? ToString();
    }   
}
