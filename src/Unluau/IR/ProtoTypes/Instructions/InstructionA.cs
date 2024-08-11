namespace Unluau.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Represents an instruction with an A field.
    /// </summary>
    public class InstructionA : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionA"/> class.
        /// </summary>
        public InstructionA(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionA"/> class.
        /// </summary>
        public InstructionA(ulong value) : base(value)
        {
        }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
