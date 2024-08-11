namespace Unluau.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents an instruction with an E field.
    /// </summary>
    public class InstructionE(uint value) : Instruction(value)
    {
        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
