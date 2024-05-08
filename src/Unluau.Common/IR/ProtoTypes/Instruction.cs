namespace Unluau.Common.IR.ProtoTypes
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

        /// <summary>
        /// Store source register into table using key from register
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// B: table register.
        /// C: index register.
        /// </remarks>
        SetTable,

        /// <summary>
        /// Load value from table into target register using constant string as a key
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: table register.
        /// C: predicted slot index (based on hash).
        /// AUX: constant table index.
        /// </remarks>
        GetTableKS,

        /// <summary>
        /// Store source register into table using constant string as a key
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// B: table register.
        /// C: predicted slot index (based on hash).
        /// AUX: constant table index.
        /// </remarks>
        SetTableKS,

        /// <summary>
        /// Load value from table into target register using small integer index as a key
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: table register.
        /// C: index-1 (index is 1..256).
        /// </remarks>
        GetTableN,

        /// <summary>
        /// Store source register into table using small integer index as a key
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// B: table register.
        /// C: index-1 (index is 1..256).
        /// </remarks>
        SetTableN,

        /// <summary>
        /// Create closure from a child proto; followed by a CAPTURE instruction for each upvalue
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// D: child proto index.
        /// </remarks>
        NewClosure,

        /// <summary>
        /// Prepare to call specified method by name by loading function from source register using constant index into target register and copying source register into target register + 1
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: source register.
        /// C: predicted slot index (based on hash).
        /// AUX: constant table index.
        /// Note that this instruction must be followed directly by CALL; it prepares the arguments
        /// This instruction is roughly equivalent to GETTABLEKS + MOVE pair, but we need a special instruction to support custom __namecall metamethod
        /// </remarks>
        NameCall,

        /// <summary>
        /// Call specified function
        /// </summary>
        /// <remarks>
        /// A: register where the function object lives, followed by arguments; results are placed starting from the same register
        /// B: argument count + 1, or 0 to preserve all arguments up to top (MULTRET)
        /// C: result count + 1, or 0 to preserve all values and adjust top (MULTRET)
        /// </remarks>
        Call,

        /// <summary>
        /// Returns specified values from the function
        /// </summary>
        /// <remarks>
        /// A: register where the function object lives, followed by arguments; results are placed starting from the same register
        /// B: argument count + 1, or 0 to preserve all arguments up to top (MULTRET)
        /// C: result count + 1, or 0 to preserve all values and adjust top (MULTRET)
        /// </remarks>
        Return,

        /// <summary>
        /// Jumps to target offset
        /// </summary>
        /// <remarks>
        /// D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump") 
        /// </remarks>
        Jump,

        /// <summary>
        /// Jumps to target offset; this is equivalent to JUMP but is used as a safepoint to be able to interrupt while/repeat loops
        /// </summary>
        /// <remarks>
        /// D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        /// </remarks>
        JumpBack,

        /// <summary>
        /// Jumps to target offset if register is not nil/false
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        /// </remarks>
        JumpIf,

        /// <summary>
        /// Jumps to target offset if register is nil/false
        /// </summary>
        /// <remarks>
        /// A: source register.
        /// D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        /// </remarks>
        JumpIfNot,

        // JUMPIFEQ, JUMPIFLE, JUMPIFLT, JUMPIFNOTEQ, JUMPIFNOTLE, JUMPIFNOTLT: jumps to target offset if the comparison is true (or false, for NOT variants)
        // A: source register 1
        // D: jump offset (-32768..32767; 1 means "next instruction" aka "don't jump")
        // AUX: source register 2
        JumpIfEq,
        JumpIfLe,
        JumpIfLt,
        JumpIfNotEq,
        JumpIfNotLe,
        JumpIfNotLt,

        // ADD, SUB, MUL, DIV, MOD, POW: compute arithmetic operation between two source registers and put the result into target register
        // A: target register
        // B: source register 1
        // C: source register 2
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Pow,

        // ADDK, SUBK, MULK, DIVK, MODK, POWK: compute arithmetic operation between the source register and a constant and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255); must refer to a number
        AddK,
        SubK,
        MulK,
        DivK,
        ModK,
        PowK,

        // AND, OR: perform `and` or `or` operation (selecting first or second register based on whether the first one is truthy) and put the result into target register
        // A: target register
        // B: source register 1
        // C: source register 2
        And,
        Or,

        // ANDK, ORK: perform `and` or `or` operation (selecting source register or constant based on whether the source register is truthy) and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255)
        AndK,
        OrK,

        /// <summary>
        /// Concatenate all strings between B and C (inclusive) and put the result into A
        /// </summary>
        /// <remarks>
        /// A: target register
        /// B: source register start
        /// C: source register end
        /// </remarks>
        Concat,

        // NOT, MINUS, LENGTH: compute unary operation for source register and put the result into target register
        // A: target register
        // B: source register
        Not,
        Minus,
        Len,

        /// <summary>
        /// Create table in target register
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: table size, stored as 0 for v=0 and ceil(log2(v))+1 for v!=0
        /// AUX: array size
        /// </remarks>
        NewTable,
        DupTable,
        SetList,
        ForNPrep,
        ForNLoop,
        ForGPrep,
        ForGLoop,

        ForGPrepINext,

        // Deprecated, removed in v3
        _ForGLoopINext,

        ForGPrepNext,
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

        /// <inheritdoc/>
        public override void Accept(Visitor visitor)
        {
            visitor.Visit(this);
        }
    }

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

        /// <summary>
        /// Gets the A operand of the instruction.
        /// </summary>
        public byte A => (byte)(Value >> 8 & 0xFF);

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
    public class InstructionE(uint value) : Instruction(value)
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
