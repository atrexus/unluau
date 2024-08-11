namespace Unluau.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents an instruction with an A, B, and C field.
    /// </summary>
    public class InstructionABC : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        public InstructionABC(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        public InstructionABC(ulong value) : base(value)
        {
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
