namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents the `settable` instruction.
    /// </summary>
    public class SetTable(uint value) : InstructionABC(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
