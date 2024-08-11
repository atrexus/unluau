namespace Unluau.IR.Versions
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

    public class TypedVersion(byte number, TypedVersionKind kind) : Version(number)
    {
        /// <summary>
        /// The kind of the typed version.
        /// </summary>
        public TypedVersionKind Kind { get; set; } = kind;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
