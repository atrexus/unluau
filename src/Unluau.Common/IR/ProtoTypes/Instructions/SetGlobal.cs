namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents a <see cref="OpCode.GetGlobal"/> instruction.
    /// </summary>
    public class SetGlobal(ulong value) : InstructionABC(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
