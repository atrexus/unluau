namespace Unluau.Common.IR.Versions
{
    /// <summary>
    /// Describes the version of the type encoding.
    /// </summary>
    public enum TypedVersionKind : byte
    {
        /// <summary>
        /// The first version of the type encoding.
        /// </summary>
        Deprecated = 1,

        /// <summary>
        /// The second version of the type encoding.
        /// </summary>
        Latest = 2,
    }

    public abstract class TypedVersion(TypedVersionKind kind) : Version
    {
        /// <summary>
        /// The kind of the typed version.
        /// </summary>
        public TypedVersionKind Kind { get; set; } = kind;

        /// <summary>
        /// Reads the type information. Must be implemented by the derived class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The type info.</returns>
        public abstract TypeInfo Read(IRReader reader);

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
