namespace Unluau.IR.ProtoTypes
{
    /// <summary>
    /// Represents an upvalue in the function prototype.
    /// </summary>
    public class Upvalue : Node
    {
        /// <summary>
        /// The type of the upvalue.
        /// </summary>
        public Type? Type { get; set; }

        /// <summary>
        /// The name of the upvalue.
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
