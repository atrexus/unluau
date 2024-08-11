namespace Unluau.IR.ProtoTypes.Constants
{
    /// <summary>
    /// The type of constants available.
    /// </summary>
    public enum ConstantType : byte
    {
        /// <summary>
        /// Equivalent to Luau's `nil` keyword.
        /// </summary>
        Nil,

        /// <summary>
        /// True or false value.
        /// </summary>
        Boolean,

        /// <summary>
        /// A double. 
        /// </summary>
        Number,

        /// <summary>
        /// Contains an index to the symbol table.
        /// </summary>
        String,

        /// <summary>
        /// An environment variable. 
        /// </summary>
        Import,

        /// <summary>
        /// A precomputed table (usually constructor data).
        /// </summary>
        Table,

        /// <summary>
        /// A precomputed closure.
        /// </summary>
        Closure,

        /// <summary>
        /// A static vector value (x, y, z, w).
        /// </summary>
        Vector
    }

    /// <summary>
    /// The base class for a constant.
    /// </summary>
    public abstract class Constant(ConstantType type) : Node
    {
        /// <summary>
        /// The type of constant we have.
        /// </summary>
        public ConstantType Type => type;

        /// <summary>
        /// Converts the current <see cref="Constant"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override abstract string? ToString();
    }

    /// <summary>
    /// Creates a new constant value in the chunk.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="type">The type of constant.</param>
    /// <param name="value">The value of the constant.</param>
    public abstract class Constant<T>(ConstantType type, T value) : Constant(type)
    {
        /// <summary>
        /// The value of the constant.
        /// </summary>
        public T Value { get; private set; } = value;

        /// <summary>
        /// Returns a string representation of the constant.
        /// </summary>
        public override string? ToString() => Value is null ? "nil" : Value.ToString();
    }
}
