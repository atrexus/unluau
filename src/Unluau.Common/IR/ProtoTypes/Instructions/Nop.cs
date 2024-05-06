namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a no-operation instruction.
    /// </summary>
    public class Nop(uint value) : Instruction(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
