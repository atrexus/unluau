namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a break instruction.
    /// </summary>
    public class Break(uint value) : Instruction(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
