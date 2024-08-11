namespace Unluau.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents an instruction with an A and B field.
    /// </summary>
    public class InstructionAB : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAB"/> class.
        /// </summary>
        public InstructionAB(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAB"/> class.
        /// </summary>
        public InstructionAB(ulong value) : base(value)
        {
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
