namespace Unluau.Common.IR.ProtoTypes.Instructions
{
    /// <summary>
    /// The different kinds of opcodes in the function prototype.
    /// </summary>
    public enum OpCode
    {
        /// <summary>
        /// No operation.
        /// </summary>
        Nop,

        /// <summary>
        /// Debugger break.
        /// </summary>
        Break,

        /// <summary>
        /// Loads a nil value into a register.
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// </remarks>
        LoadNil,
    }

    /// <summary>
    /// Represents an instruction in the function prototype.
    /// </summary>
    public class Instruction(uint value) : Node
    {
        /// <summary>
        /// Contains the raw value of the instruction.
        /// </summary>
        public uint Value { get; private set; } = value;

        /// <summary>
        /// Gets the operation code of the instruction.
        /// </summary>
        public OpCode Code => (OpCode)(Value & 0xFF);

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Represents an instruction with an A, B, and C field. May also have an auxiliary instruction.
    /// </summary>
    public abstract class InstructionABC(uint value) : Instruction(value)
    {
        /// <summary>
        /// Gets the A operand of the instruction.
        /// </summary>
        public byte A => (byte)((Value >> 8) & 0xFF);

        /// <summary>
        /// Gets the B operand of the instruction.
        /// </summary>
        public byte B => (byte)((Value >> 16) & 0xFF);

        /// <summary>
        /// Gets the C operand of the instruction.
        /// </summary>
        public byte C => (byte)((Value >> 24) & 0xFF);

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Represents an instruction with an A and D field.
    /// </summary>
    public abstract class InstructionAD(uint value) : Instruction(value)
    {
        /// <summary>
        /// Gets the A operand of the instruction.
        /// </summary>
        public byte A => (byte)((Value >> 8) & 0xFF);

        /// <summary>
        /// Gets the D operand of the instruction.
        /// </summary>
        public int D => (int)(Value >> 16);

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Represents an instruction with an E field.
    /// </summary>
    public abstract class InstructionE(uint value) : Instruction(value)
    {
        /// <summary>
        /// Gets the E operand of the instruction.
        /// </summary>
        public int E => (int)(Value >> 8);

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
