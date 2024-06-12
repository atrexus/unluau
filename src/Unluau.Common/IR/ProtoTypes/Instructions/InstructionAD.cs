namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents an instruction with an A and D field.
    /// </summary>
    public class InstructionAD : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAD"/> class.
        /// </summary>
        public InstructionAD(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAD"/> class.
        /// </summary>
        public InstructionAD(ulong value) : base(value)
        {
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
