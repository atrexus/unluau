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
        LoadB,

        /// <summary>
        /// Loads a number into a register.
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// D: value (-32768..32767).
        /// </remarks>
        LoadN,

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
        GetUpval,

        /// <summary>
        /// Store value into the upvalue table for the current function
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: upvalue index.
        /// </remarks>
        SetUpval,

        /// <summary>
        /// Close (migrate to heap) all upvalues that were captured for registers >= target
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// </remarks>
        CloseUpvals,

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
        Length,

        /// <summary>
        /// Create table in target register
        /// </summary>
        /// <remarks>
        /// A: target register.
        /// B: table size, stored as 0 for v=0 and ceil(log2(v))+1 for v!=0
        /// AUX: array size
        /// </remarks>
        NewTable,

        /// <summary>
        /// Duplicate table using the constant table template to target register
        /// </summary>
        /// <remarks>
        /// A: target register
        /// D: constant table index (0..32767)
        /// </remarks>
        DupTable,
        SetList,
        ForNPrep,
        ForNLoop,
        ForGLoop,

        /// <summary>
        /// Prepare FORGLOOP with 2 output variables (no AUX encoding), assuming generator is luaB_inext, and jump to FORGLOOP
        /// </summary>
        /// <remarks>
        /// A: target register (see FORGLOOP for register layout)
        /// </remarks>
        ForGPrepINext,

        // Deprecated, removed in v3
        _ForGLoopINext,

        /// <summary>
        /// Prepare FORGLOOP with 2 output variables (no AUX encoding), assuming generator is luaB_next, and jump to FORGLOOP
        /// </summary>
        /// <remarks>
        /// A: target register (see FORGLOOP for register layout)
        /// </remarks>
        ForGPrepNext,

        /// <summary>
        /// Start executing new function in native code
        /// </summary>
        /// <remarks>
        /// This is a pseudo-instruction that is never emitted by bytecode compiler, but can be constructed at runtime to accelerate native code dispatch
        /// </remarks>
        NativeCall,

        /// <summary>
        /// Copy variables into the target register from vararg storage for current function.
        /// </summary>
        /// <remarks>
        /// A: target register
        /// B: variable count + 1, or 0 to copy all variables and adjust top (MULTRET)
        /// </remarks>
        GetVarArgs,

        /// <summary>
        /// Create closure from a pre-created function object (reusing it unless environments diverge).
        /// </summary>
        /// <remarks>
        /// A: target register
        /// D: constant table index (0..32767)
        /// </remarks>
        DupClosure,

        /// <summary>
        /// Prepare stack for variadic functions so that GETVARARGS works correctly.
        /// </summary>
        /// <remarks>
        /// A: number of fixed arguments
        /// </remarks>
        PrepVarArgs,

        /// <summary>
        /// Sets register to an entry from the constant table from the proto (number/string).
        /// </summary>
        /// <remarks>
        /// A: target register
        /// AUX: constant table index
        /// </remarks>
        LoadKX,

        /// <summary>
        /// Jumps to the target offset; like JUMPBACK, supports interruption.
        /// </summary>  
        /// <remarks>
        /// E: jump offset (-2^23..2^23; 0 means "next instruction" aka "don't jump")
        /// </remarks>
        JumpX,

        /// <summary>
        /// Perform a fast call of a built-in function.
        /// </summary>
        /// <remarks>
        /// A: built-in function id (see LuauBuiltinFunction)
        /// C: jump offset to get to following CALL
        /// FASTCALL is followed by one of (GETIMPORT, MOVE, GETUPVAL) instructions and by CALL instruction
        /// This is necessary so that if FASTCALL can't perform the call inline, it can continue normal execution
        /// If FASTCALL *can* perform the call, it jumps over the instructions *and* over the next CALL
        /// Note that FASTCALL will read the actual call arguments, such as argument/result registers and counts, from the CALL instruction
        /// </remarks>
        FastCall,

        /// <summary>
        /// Update coverage information stored in the instruction.
        /// </summary>
        /// <remarks>
        /// E: hit count for the instruction (0..2^23-1)
        /// The hit count is incremented by VM every time the instruction is executed, and saturates at 2^23-1
        /// </remarks>
        Coverage,

        /// <summary>
        /// Capture a local or an upvalue as an upvalue into a newly created closure; only valid after NEWCLOSURE.
        /// </summary>
        /// <remarks>
        /// A: capture type, see LuauCaptureType
        /// B: source register (for VAL/REF) or upvalue index (for UPVAL/UPREF)
        /// </remarks>
        Capture,

        // SUBRK, DIVRK: compute arithmetic operation between the constant and a source register and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255); must refer to a number
        SubRK,
        DivRK,

        /// <summary>
        /// Perform a fast call of a built-in function using 1 register argument.
        /// </summary>
        /// <remarks>
        /// A: builtin function id (see LuauBuiltinFunction)
        /// B: source argument register
        /// C: jump offset to get to following CALL
        /// </remarks>
        FastCall1,

        /// <summary>
        /// Perform a fast call of a built-in function using 2 register argument.
        /// </summary>
        /// <remarks>
        /// A: builtin function id (see LuauBuiltinFunction)
        /// B: source argument register
        /// C: jump offset to get to following CALL
        /// AUX: source register 2 in least-significant byte
        /// </remarks>
        FastCall2,

        /// <summary>
        /// Perform a fast call of a built-in function using 1 register argument and 1 constant argument
        /// </summary>
        /// <remarks>
        /// A: builtin function id (see LuauBuiltinFunction)
        /// B: source argument register
        /// C: jump offset to get to following CALL
        /// AUX: constant index
        /// </remarks>
        FastCall2K,

        /// <summary>
        /// Prepare loop variables for a generic for loop, jump to the loop backedge unconditionally.
        /// </summary>
        /// <remarks>
        /// A: target register; generic for loops assume a register layout [generator, state, index, variables...]
        /// D: jump offset (-32768..32767)
        /// </remarks>
        ForGPrep,

        /// <summary>
        /// Jumps to target offset if the comparison with constant is true (or false, see AUX)
        /// </summary>
        /// <remarks>
        /// A: source register 1
        /// D: jump offset (-32768..32767; 1 means "next instruction" aka "don't jump")
        /// AUX: constant value (for boolean) in low bit, NOT flag (that flips comparison result) in high bit
        /// </remarks>
        JumpXEqKNil,
        JumpXEqKB,

        // JUMPXEQKN, JUMPXEQKS: jumps to target offset if the comparison with constant is true (or false, see AUX)
        // A: source register 1
        // D: jump offset (-32768..32767; 1 means "next instruction" aka "don't jump")
        // AUX: constant table index in low 24 bits, NOT flag (that flips comparison result) in high bit
        JumpXEqKN,
        JumpXEqKS,

        /// <summary>
        /// Compute floor division between two source registers and put the result into target register
        /// </summary>
        /// <remarks>
        /// A: target register
        /// B: source register 1
        /// C: source register 2
        /// </remarks>
        IDiv,

        /// <summary>
        /// Compute floor division between the source register and a constant and put the result into target register.
        /// </summary>
        /// <remarks>
        /// A: target register
        /// B: source register
        /// C: constant table index (0..255)
        /// </remarks>
        IDivK,
    }
}
