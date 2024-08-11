namespace Unluau.IR.ProtoTypes
{
    /// <summary>
    /// The different kinds of types in Luau.
    /// </summary>
    public enum TypeKind : byte
    {
        /// <summary>
        /// A nil type.
        /// </summary>
        Nil,

        /// <summary>
        /// A boolean type.
        /// </summary>
        Boolean,

        /// <summary>
        /// A number type.
        /// </summary>
        Number,

        /// <summary>
        /// A string type.
        /// </summary>
        String,

        /// <summary>
        /// A table type.
        /// </summary>
        Table,

        /// <summary>
        /// A function type.
        /// </summary>
        Function,

        /// <summary>
        /// A thread type.
        /// </summary>
        Thread,

        /// <summary>
        /// A user data type.
        /// </summary>
        UserData,

        /// <summary>
        /// A vector type.
        /// </summary>
        Vector,

        /// <summary>
        /// A buffer type.
        /// </summary>
        Buffer,

        Any = 15,
    }

    /// <summary>
    /// Represents a type in Luau bytecode.
    /// </summary>
    public class Type(TypeKind kind, bool optional) : Node
    {
        /// <summary>
        /// The kind of type this is.
        /// </summary>
        public TypeKind Kind { get; set; } = kind;

        /// <summary>
        /// Whether the type is optional.
        /// </summary>
        public bool IsOptional { get; set; } = optional;

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
