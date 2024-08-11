namespace Unluau.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// Contains additional information about the instruction.
    /// </summary>
    public record Context
    {
        /// <summary>
        /// The line the instruction is defined on.
        /// </summary>
        public int? LineDefined { get; set; }

        /// <summary>
        /// The program counter of the instruction.
        /// </summary>
        public int Pc { get; set; }
    }

    /// <summary>
    /// Represents an instruction in the function prototype.
    /// </summary>
    public class Instruction(uint value) : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        public Instruction(ulong value) : this((uint)(value & 0xFFFFFFFF)) => Aux = new Instruction((uint)(value >> 32));

        /// <summary>
        /// Gets or sets the context of the instruction.
        /// </summary>
        public Context Context { get; set; } = new();

        /// <summary>
        /// Contains the raw value of the instruction.
        /// </summary>
        public uint Value { get; private set; } = value;

        /// <summary>
        /// Gets the operation code of the instruction.
        /// </summary>
        public OpCode Code => (OpCode)(Value & 0xFF);

        /// <summary>
        /// Gets the A operand of the instruction.
        /// </summary>
        public byte A => (byte)(Value >> 8 & 0xFF);

        /// <summary>
        /// Gets the B operand of the instruction.
        /// </summary>
        public byte B => (byte)(Value >> 16 & 0xFF);

        /// <summary>
        /// Gets the C operand of the instruction.
        /// </summary>
        public byte C => (byte)(Value >> 24 & 0xFF);

        /// <summary>
        /// Gets the D operand of the instruction.
        /// </summary>
        public int D => (int)(Value >> 16);

        /// <summary>
        /// Gets the E operand of the instruction.
        /// </summary>
        public int E => (int)(Value >> 8);

        /// <summary>
        /// Gets or sets the auxiliary instruction.
        /// </summary>
        public Instruction? Aux { get; set; }

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
