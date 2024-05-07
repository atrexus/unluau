namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a load boolean instruction.
    /// </summary>
    public class LoadBoolean(uint value) : InstructionABC(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
