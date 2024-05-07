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

        /// <summary>
        /// Loads a boolean value into a register.
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: value (1/0).
        /// C: jump offset.
        /// </remarks>
        LoadBoolean,

        /// <summary>
        /// Loads a number into a register.
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// D: value (-32768..32767).
        /// </remarks>
        LoadNumber,

        /// <summary>
        /// Sets register to an entry from the constant table from the proto (number/vector/string)
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// D: constant index (0..32767).
        /// </remarks>
        LoadK,

        /// <summary>
        /// Moves a value from one register to another.
        /// </summary>
        /// <remarks>
        /// A: target register. B: source register.
        /// </remarks>
        Move,

        /// <summary>
        /// Load value from global table using constant string as a key
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// C: predicted slot index (based on hash).
        /// AUX: constant table index.
        /// </remarks>
        GetGlobal,

        /// <summary>
        /// Load value from global table using constant string as a key
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// C: predicted slot index (based on hash).
        /// AUX: constant table index.
        /// </remarks>
        SetGlobal,

        /// <summary>
        /// Load upvalue from the upvalue table for the current function
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: upvalue index.
        /// </remarks>
        GetUpvalue,

        /// <summary>
        /// Store value into the upvalue table for the current function
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: upvalue index.
        /// </remarks>
        SetUpvalue,

        /// <summary>
        /// Close (migrate to heap) all upvalues that were captured for registers >= target
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// </remarks>
        CloseUpvalues,

        /// <summary>
        /// Load imported global table global from the constant table
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: constant table index (0..32767); we assume that imports are loaded into the constant table
        /// AUX: 3 10-bit indices of constant strings that, combined, constitute an import path; length of the path is set by the top 2 bits (1,2,3)
        /// </remarks>
        GetImport,

        /// <summary>
        /// Load value from table into target register using key from register
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: table register.
        /// C: index register.
        /// </remarks>
        GetTable,
    }

    /// <summary>
    /// Represents an instruction in the function prototype.
    /// </summary>
    public class Instruction(uint value) : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        public Instruction(ulong value) : this((uint)(value >> 32)) => Aux = new Instruction((uint)value);

        /// <summary>
        /// Contains the raw value of the instruction.
        /// </summary>
        public uint Value { get; private set; } = value;

        /// <summary>
        /// Gets the operation code of the instruction.
        /// </summary>
        public OpCode Code => (OpCode)(Value & 0xFF);

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

    /// <summary>
    /// Represents an instruction with an A, B, and C field.
    /// </summary>
    public abstract class InstructionABC : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        protected InstructionABC(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionABC"/> class.
        /// </summary>
        protected InstructionABC(ulong value) : base(value)
        {
        }

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
    public abstract class InstructionAD : Instruction
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAD"/> class.
        /// </summary>
        protected InstructionAD(uint value) : base(value)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstructionAD"/> class.
        /// </summary>
        protected InstructionAD(ulong value) : base(value)
        {
        }

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
