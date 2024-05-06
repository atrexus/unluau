namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a load nil instruction.
    /// </summary>
    public class LoadNil(uint value) : InstructionABC(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
