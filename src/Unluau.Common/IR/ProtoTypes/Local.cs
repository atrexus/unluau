namespace Unluau.Common.IR.ProtoTypes
{
    /// <summary>
    /// Represents a local variable in the function prototype.
    /// </summary>
    public class Local(byte register, (int, int) scope) : Node
    {
        /// <summary>
        /// The register the local variable is stored in.
        /// </summary>
        public byte Register { get; set; } = register;

        /// <summary>
        /// The scope of the local variable (in terms of instruction indices).
        /// </summary>
        public (int, int) Scope { get; set; } = scope;

        /// <summary>
        /// The type of the local variable.
        /// </summary>
        public Type? Type { get; set; }

        /// <summary>
        /// The name of the local variable.
        /// </summary>
        public string? Name { get; set; }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            if (visitor.Visit(this))
            {
                Type?.Accept(visitor);
            }
        }
    }
}
