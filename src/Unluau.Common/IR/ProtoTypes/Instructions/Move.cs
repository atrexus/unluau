namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a move instruction.
    /// </summary>
    public class Move(uint value) : InstructionABC(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
