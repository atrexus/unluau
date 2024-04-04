// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Unluau
{
    public enum OpMode
    {
        // ABC encoding: three 8-bit values, containing registers or small numbers
        iABC,
        // AD encoding: one 8-bit value, one signed 16-bit value
        iAD,
        // E encoding: one signed 24-bit value
        iE,
        AUX,

        // Represents an empy instruction (contans no args)
        None
    }

    public enum OpCode : byte
    {
        // NOP: noop
        NOP,

        // BREAK: debugger break
        BREAK,

        // LOADNIL: sets register to nil
        // A: target register
        LOADNIL,

        // LOADB: sets register to boolean and jumps to a given short offset (used to compile comparison results into a boolean)
        // A: target register
        // B: value (0/1)
        // C: jump offset
        LOADB,

        // LOADN: sets register to a number literal
        // A: target register
        // D: value (-32768..32767)
        LOADN,

        // LOADK: sets register to an entry from the constant table from the proto (number/string)
        // A: target register
        // D: constant table index (0..32767)
        LOADK,

        // MOVE: move (copy) value from one register to another
        // A: target register
        // B: source register
        MOVE,

        // GETGLOBAL: load value from global table using constant string as a key
        // A: target register
        // C: predicted slot index (based on hash)
        // AUX: constant table index
        GETGLOBAL,

        // SETGLOBAL: set value in global table using constant string as a key
        // A: source register
        // C: predicted slot index (based on hash)
        // AUX: constant table index
        SETGLOBAL,

        // GETUPVAL: load upvalue from the upvalue table for the current function
        // A: target register
        // B: upvalue index (0..255)
        GETUPVAL,

        // SETUPVAL: store value into the upvalue table for the current function
        // A: target register
        // B: upvalue index (0..255)
        SETUPVAL,

        // CLOSEUPVALS: close (migrate to heap) all upvalues that were captured for registers >= target
        // A: target register
        CLOSEUPVALS,

        // GETIMPORT: load imported global table global from the constant table
        // A: target register
        // D: constant table index (0..32767); we assume that imports are loaded into the constant table
        // AUX: 3 10-bit indices of constant strings that, combined, constitute an import path; length of the path is set by the top 2 bits (1,2,3)
        GETIMPORT,

        // GETTABLE: load value from table into target register using key from register
        // A: target register
        // B: table register
        // C: index register
        GETTABLE,

        // SETTABLE: store source register into table using key from register
        // A: source register
        // B: table register
        // C: index register
        SETTABLE,

        // GETTABLEKS: load value from table into target register using constant string as a key
        // A: target register
        // B: table register
        // C: predicted slot index (based on hash)
        // AUX: constant table index
        GETTABLEKS,

        // SETTABLEKS: store source register into table using constant string as a key
        // A: source register
        // B: table register
        // C: predicted slot index (based on hash)
        // AUX: constant table index
        SETTABLEKS,

        // GETTABLEN: load value from table into target register using small integer index as a key
        // A: target register
        // B: table register
        // C: index-1 (index is 1..256)
        GETTABLEN,

        // SETTABLEN: store source register into table using small integer index as a key
        // A: source register
        // B: table register
        // C: index-1 (index is 1..256)
        SETTABLEN,

        // NEWCLOSURE: create closure from a child proto; followed by a CAPTURE instruction for each upvalue
        // A: target register
        // D: child proto index (0..32767)
        NEWCLOSURE,

        // NAMECALL: prepare to call specified method by name by loading function from source register using constant index into target register and copying source register into target register + 1
        // A: target register
        // B: source register
        // C: predicted slot index (based on hash)
        // AUX: constant table index
        // Note that this instruction must be followed directly by CALL; it prepares the arguments
        // This instruction is roughly equivalent to GETTABLEKS + MOVE pair, but we need a special instruction to support custom __namecall metamethod
        NAMECALL,

        // CALL: call specified function
        // A: register where the function object lives, followed by arguments; results are placed starting from the same register
        // B: argument count + 1, or 0 to preserve all arguments up to top (MULTRET)
        // C: result count + 1, or 0 to preserve all values and adjust top (MULTRET)
        CALL,

        // RETURN: returns specified values from the function
        // A: register where the returned values start
        // B: number of returned values + 1, or 0 to return all values up to top (MULTRET)
        RETURN,

        // JUMP: jumps to target offset
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        JUMP,

        // JUMPBACK: jumps to target offset; this is equivalent to JUMP but is used as a safepoint to be able to interrupt while/repeat loops
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        JUMPBACK,

        // JUMPIF: jumps to target offset if register is not nil/false
        // A: source register
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        JUMPIF,

        // JUMPIFNOT: jumps to target offset if register is nil/false
        // A: source register
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        JUMPIFNOT,

        // JUMPIFEQ, JUMPIFLE, JUMPIFLT, JUMPIFNOTEQ, JUMPIFNOTLE, JUMPIFNOTLT: jumps to target offset if the comparison is true (or false, for NOT variants)
        // A: source register 1
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        // AUX: source register 2
        JUMPIFEQ,
        JUMPIFLE,
        JUMPIFLT,
        JUMPIFNOTEQ,
        JUMPIFNOTLE,
        JUMPIFNOTLT,

        // ADD, SUB, MUL, DIV, MOD, POW: compute arithmetic operation between two source registers and put the result into target register
        // A: target register
        // B: source register 1
        // C: source register 2
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        POW,

        // ADDK, SUBK, MULK, DIVK, MODK, POWK: compute arithmetic operation between the source register and a constant and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255)
        ADDK,
        SUBK,
        MULK,
        DIVK,
        MODK,
        POWK,

        // AND, OR: perform `and` or `or` operation (selecting first or second register based on whether the first one is truthy) and put the result into target register
        // A: target register
        // B: source register 1
        // C: source register 2
        AND,
        OR,

        // ANDK, ORK: perform `and` or `or` operation (selecting source register or constant based on whether the source register is truthy) and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255)
        ANDK,
        ORK,

        // CONCAT: concatenate all strings between B and C (inclusive) and put the result into A
        // A: target register
        // B: source register start
        // C: source register end
        CONCAT,

        // NOT, MINUS, LENGTH: compute unary operation for source register and put the result into target register
        // A: target register
        // B: source register
        NOT,
        MINUS,
        LENGTH,

        // NEWTABLE: create table in target register
        // A: target register
        // B: table size, stored as 0 for v=0 and ceil(log2(v))+1 for v!=0
        // AUX: array size
        NEWTABLE,

        // DUPTABLE: duplicate table using the constant table template to target register
        // A: target register
        // D: constant table index (0..32767)
        DUPTABLE,

        // SETLIST: set a list of values to table in target register
        // A: target register
        // B: source register start
        // C: value count + 1, or 0 to use all values up to top (MULTRET)
        // AUX: table index to start from
        SETLIST,

        // FORNPREP: prepare a numeric for loop, jump over the loop if first iteration doesn't need to run
        // A: target register; numeric for loops assume a register layout [limit, step, index, variable]
        // D: jump offset (-32768..32767)
        // limit/step are immutable, index isn't visible to user code since it's copied into variable
        FORNPREP,

        // FORNLOOP: adjust loop variables for one iteration, jump back to the loop header if loop needs to continue
        // A: target register; see FORNPREP for register layout
        // D: jump offset (-32768..32767)
        FORNLOOP,

        // FORGLOOP: adjust loop variables for one iteration of a generic for loop, jump back to the loop header if loop needs to continue
        // A: target register; generic for loops assume a register layout [generator, state, index, variables...]
        // D: jump offset (-32768..32767)
        // AUX: variable count (1..255) in the low 8 bits, high bit indicates whether to use ipairs-style traversal in the fast path
        // loop variables are adjusted by calling generator(state, index) and expecting it to return a tuple that's copied to the user variables
        // the first variable is then copied into index; generator/state are immutable, index isn't visible to user code
        FORGLOOP,

        // FORGPREP_INEXT: prepare FORGLOOP with 2 output variables (no AUX encoding), assuming generator is luaB_inext, and jump to FORGLOOP
        // A: target register (see FORGLOOP for register layout)
        FORGPREP_INEXT,

        // removed in v3
        DEP_FORGLOOP_INEXT,

        // FORGPREP_NEXT: prepare FORGLOOP with 2 output variables (no AUX encoding), assuming generator is luaB_next, and jump to FORGLOOP
        // A: target register (see FORGLOOP for register layout)
        FORGPREP_NEXT,

        // removed in v3
        DEP_FORGLOOP_NEXT,

        // GETVARARGS: copy variables into the target register from vararg storage for current function
        // A: target register
        // B: variable count + 1, or 0 to copy all variables and adjust top (MULTRET)
        GETVARARGS,

        // DUPCLOSURE: create closure from a pre-created function object (reusing it unless environments diverge)
        // A: target register
        // D: constant table index (0..32767)
        DUPCLOSURE,

        // PREPVARARGS: prepare stack for variadic functions so that GETVARARGS works correctly
        // A: number of fixed arguments
        PREPVARARGS,

        // LOADKX: sets register to an entry from the constant table from the proto (number/string)
        // A: target register
        // AUX: constant table index
        LOADKX,

        // JUMPX: jumps to the target offset; like JUMPBACK, supports interruption
        // E: jump offset (-2^23..2^23; 0 means "next instruction" aka "don't jump")
        JUMPX,

        // FASTCALL: perform a fast call of a built-in function
        // A: builtin function id (see LuauBuiltinFunction)
        // C: jump offset to get to following CALL
        // FASTCALL is followed by one of (GETIMPORT, MOVE, GETUPVAL) instructions and by CALL instruction
        // This is necessary so that if FASTCALL can't perform the call inline, it can continue normal execution
        // If FASTCALL *can* perform the call, it jumps over the instructions *and* over the next CALL
        // Note that FASTCALL will read the actual call arguments, such as argument/result registers and counts, from the CALL instruction
        FASTCALL,

        // COVERAGE: update coverage information stored in the instruction
        // E: hit count for the instruction (0..2^23-1)
        // The hit count is incremented by VM every time the instruction is executed, and saturates at 2^23-1
        COVERAGE,

        // CAPTURE: capture a local or an upvalue as an upvalue into a newly created closure; only valid after NEWCLOSURE
        // A: capture type, see LuauCaptureType
        // B: source register (for VAL/REF) or upvalue index (for UPVAL/UPREF)
        CAPTURE,

        // SUBRK, DIVRK: compute arithmetic operation between the constant and a source register and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255); must refer to a number
        SUBRK,
        DIVRK,

        // FASTCALL1: perform a fast call of a built-in function using 1 register argument
        // A: builtin function id (see LuauBuiltinFunction)
        // B: source argument register
        // C: jump offset to get to following CALL
        FASTCALL1,

        // FASTCALL2: perform a fast call of a built-in function using 2 register arguments
        // A: builtin function id (see LuauBuiltinFunction)
        // B: source argument register
        // C: jump offset to get to following CALL
        // AUX: source register 2 in least-significant byte
        FASTCALL2,

        // FASTCALL2K: perform a fast call of a built-in function using 1 register argument and 1 constant argument
        // A: builtin function id (see LuauBuiltinFunction)
        // B: source argument register
        // C: jump offset to get to following CALL
        // AUX: constant index
        FASTCALL2K,

        // FORGPREP: prepare loop variables for a generic for loop, jump to the loop backedge unconditionally
        // A: target register; generic for loops assume a register layout [generator, state, index, variables...]
        // D: jump offset (-32768..32767)
        FORGPREP,

        // JUMPXEQKNIL, JUMPXEQKB: jumps to target offset if the comparison with constant is true (or false, see AUX)
        // A: source register 1
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        // AUX: constant value (for boolean) in low bit, NOT flag (that flips comparison result) in high bit
        JUMPXEQKNIL,
        JUMPXEQKB,

        // JUMPXEQKN, JUMPXEQKS: jumps to target offset if the comparison with constant is true (or false, see AUX)
        // A: source register 1
        // D: jump offset (-32768..32767; 0 means "next instruction" aka "don't jump")
        // AUX: constant table index in low 24 bits, NOT flag (that flips comparison result) in high bit
        JUMPXEQKN,
        JUMPXEQKS,

        // IDIV: compute floor division between two source registers and put the result into target register
        // A: target register
        // B: source register 1
        // C: source register 2
        IDIV,

        // IDIVK compute floor division between the source register and a constant and put the result into target register
        // A: target register
        // B: source register
        // C: constant table index (0..255)
        IDIVK,

        // Enum entry for number of opcodes, not a valid opcode by itself!
        COUNT
    }

    public enum OpCodeEncoding
    {
        None,
        Client,
        Studio
    }

    public class OpProperties
    {
        public OpCode Code { get; private set; }
        public OpMode Mode { get; private set; }
        public bool HasAux { get; private set; }

        public OpProperties(OpCode code, OpMode mode, bool hasAux = false)
        {
            Code = code;
            Mode = mode;
            HasAux = hasAux;
        }

        public static IDictionary<OpCode, OpProperties> Map = new Dictionary<OpCode, OpProperties>()
        {
            { OpCode.NOP, new OpProperties(OpCode.NOP, OpMode.None) },
            { OpCode.BREAK, new OpProperties(OpCode.BREAK, OpMode.None) },
            { OpCode.LOADNIL, new OpProperties(OpCode.LOADNIL, OpMode.iABC) },
            { OpCode.LOADB, new OpProperties(OpCode.LOADB, OpMode.iABC) },
            { OpCode.LOADN, new OpProperties(OpCode.LOADN, OpMode.iAD) },
            { OpCode.LOADK, new OpProperties(OpCode.LOADK, OpMode.iAD) },
            { OpCode.MOVE, new OpProperties(OpCode.MOVE, OpMode.iABC) },
            { OpCode.GETGLOBAL, new OpProperties(OpCode.GETGLOBAL, OpMode.iABC, true) },
            { OpCode.SETGLOBAL, new OpProperties(OpCode.SETGLOBAL, OpMode.iABC, true) },
            { OpCode.GETUPVAL, new OpProperties(OpCode.GETUPVAL, OpMode.iABC) },
            { OpCode.SETUPVAL, new OpProperties(OpCode.SETUPVAL, OpMode.iABC) },
            { OpCode.CLOSEUPVALS, new OpProperties(OpCode.CLOSEUPVALS, OpMode.iABC) },
            { OpCode.GETIMPORT, new OpProperties(OpCode.GETIMPORT, OpMode.iAD, true) },
            { OpCode.GETTABLE, new OpProperties(OpCode.GETTABLE, OpMode.iABC) },
            { OpCode.SETTABLE, new OpProperties(OpCode.SETTABLE, OpMode.iABC) },
            { OpCode.GETTABLEKS, new OpProperties(OpCode.GETTABLEKS, OpMode.iABC, true) },
            { OpCode.SETTABLEKS, new OpProperties(OpCode.SETTABLEKS, OpMode.iABC, true) },
            { OpCode.GETTABLEN, new OpProperties(OpCode.GETTABLEN, OpMode.iABC) },
            { OpCode.SETTABLEN, new OpProperties(OpCode.SETTABLEN, OpMode.iABC) },
            { OpCode.NEWCLOSURE, new OpProperties(OpCode.NEWCLOSURE, OpMode.iAD) },
            { OpCode.NAMECALL, new OpProperties(OpCode.NAMECALL, OpMode.iABC, true) },
            { OpCode.CALL, new OpProperties(OpCode.CALL, OpMode.iABC) },
            { OpCode.RETURN, new OpProperties(OpCode.RETURN, OpMode.iABC) },
            { OpCode.JUMP, new OpProperties(OpCode.JUMP, OpMode.iAD) },
            { OpCode.JUMPBACK, new OpProperties(OpCode.JUMPBACK, OpMode.iAD) },
            { OpCode.JUMPIF, new OpProperties(OpCode.JUMPIF, OpMode.iAD) },
            { OpCode.JUMPIFNOT, new OpProperties(OpCode.JUMPIFNOT, OpMode.iAD) },
            { OpCode.JUMPIFEQ, new OpProperties(OpCode.JUMPIFEQ, OpMode.iAD, true) },
            { OpCode.JUMPIFLE, new OpProperties(OpCode.JUMPIFLE, OpMode.iAD, true) },
            { OpCode.JUMPIFLT, new OpProperties(OpCode.JUMPIFLT, OpMode.iAD, true) },
            { OpCode.JUMPIFNOTEQ, new OpProperties(OpCode.JUMPIFNOTEQ, OpMode.iAD, true) },
            { OpCode.JUMPIFNOTLE, new OpProperties(OpCode.JUMPIFNOTLE, OpMode.iAD, true) },
            { OpCode.JUMPIFNOTLT, new OpProperties(OpCode.JUMPIFNOTLT, OpMode.iAD, true) },
            { OpCode.ADD, new OpProperties(OpCode.ADD, OpMode.iABC) },
            { OpCode.SUB, new OpProperties(OpCode.SUB, OpMode.iABC) },
            { OpCode.MUL, new OpProperties(OpCode.MUL, OpMode.iABC) },
            { OpCode.DIV, new OpProperties(OpCode.DIV, OpMode.iABC) },
            { OpCode.MOD, new OpProperties(OpCode.MOD, OpMode.iABC) },
            { OpCode.POW, new OpProperties(OpCode.POW, OpMode.iABC) },
            { OpCode.ADDK, new OpProperties(OpCode.ADDK, OpMode.iABC) },
            { OpCode.SUBK, new OpProperties(OpCode.SUBK, OpMode.iABC) },
            { OpCode.MULK, new OpProperties(OpCode.MULK, OpMode.iABC) },
            { OpCode.DIVK, new OpProperties(OpCode.DIVK, OpMode.iABC) },
            { OpCode.MODK, new OpProperties(OpCode.MODK, OpMode.iABC) },
            { OpCode.POWK, new OpProperties(OpCode.POWK, OpMode.iABC) },
            { OpCode.AND, new OpProperties(OpCode.AND, OpMode.iABC) },
            { OpCode.OR, new OpProperties(OpCode.OR, OpMode.iABC) },
            { OpCode.ANDK, new OpProperties(OpCode.ANDK, OpMode.iABC) },
            { OpCode.ORK, new OpProperties(OpCode.ORK, OpMode.iABC) },
            { OpCode.CONCAT, new OpProperties(OpCode.CONCAT, OpMode.iABC) },
            { OpCode.NOT, new OpProperties(OpCode.NOT, OpMode.iABC) },
            { OpCode.MINUS, new OpProperties(OpCode.MINUS, OpMode.iABC) },
            { OpCode.LENGTH, new OpProperties(OpCode.LENGTH, OpMode.iABC) },
            { OpCode.NEWTABLE, new OpProperties(OpCode.NEWTABLE, OpMode.iABC, true) },
            { OpCode.DUPTABLE, new OpProperties(OpCode.DUPTABLE, OpMode.iAD) },
            { OpCode.SETLIST, new OpProperties(OpCode.SETLIST, OpMode.iABC, true) },
            { OpCode.FORNPREP, new OpProperties(OpCode.FORNPREP, OpMode.iAD) },
            { OpCode.FORNLOOP, new OpProperties(OpCode.FORNLOOP, OpMode.iAD) },
            { OpCode.FORGLOOP, new OpProperties(OpCode.FORGLOOP, OpMode.iAD, true) },
            { OpCode.FORGPREP_INEXT, new OpProperties(OpCode.FORGPREP_INEXT, OpMode.iAD) },
            { OpCode.DEP_FORGLOOP_INEXT, new OpProperties(OpCode.DEP_FORGLOOP_INEXT, OpMode.iAD) },
            { OpCode.FORGPREP_NEXT, new OpProperties(OpCode.FORGPREP_NEXT, OpMode.iAD) },
            { OpCode.DEP_FORGLOOP_NEXT, new OpProperties(OpCode.DEP_FORGLOOP_NEXT, OpMode.iAD) },
            { OpCode.GETVARARGS, new OpProperties(OpCode.GETVARARGS, OpMode.iABC) },
            { OpCode.DUPCLOSURE, new OpProperties(OpCode.DUPCLOSURE, OpMode.iAD) },
            { OpCode.PREPVARARGS, new OpProperties(OpCode.PREPVARARGS, OpMode.iABC) },
            { OpCode.LOADKX, new OpProperties(OpCode.LOADKX, OpMode.iABC, true) },
            { OpCode.JUMPX, new OpProperties(OpCode.JUMPX, OpMode.iE) },
            { OpCode.FASTCALL, new OpProperties(OpCode.FASTCALL, OpMode.iABC) },
            { OpCode.COVERAGE, new OpProperties(OpCode.COVERAGE, OpMode.iE) },
            { OpCode.CAPTURE, new OpProperties(OpCode.CAPTURE, OpMode.iABC) },
            { OpCode.SUBRK, new OpProperties(OpCode.SUBRK, OpMode.iABC, false) },
            { OpCode.DIVRK, new OpProperties(OpCode.DIVRK, OpMode.iABC, false) },
            { OpCode.FASTCALL1, new OpProperties(OpCode.FASTCALL1, OpMode.iABC) },
            { OpCode.FASTCALL2, new OpProperties(OpCode.FASTCALL2, OpMode.iABC, true) },
            { OpCode.FASTCALL2K, new OpProperties(OpCode.FASTCALL2K, OpMode.iABC, true) },
            { OpCode.FORGPREP, new OpProperties(OpCode.FORGPREP, OpMode.iAD) },
            { OpCode.JUMPXEQKNIL, new OpProperties(OpCode.JUMPXEQKNIL, OpMode.iAD, true) },
            { OpCode.JUMPXEQKB, new OpProperties(OpCode.JUMPXEQKB, OpMode.iAD, true) },
            { OpCode.JUMPXEQKN, new OpProperties(OpCode.JUMPXEQKN, OpMode.iAD, true) },
            { OpCode.JUMPXEQKS, new OpProperties(OpCode.JUMPXEQKS, OpMode.iAD, true) },
            { OpCode.IDIV, new OpProperties(OpCode.IDIV, OpMode.iABC, false) },
            { OpCode.IDIVK, new OpProperties(OpCode.IDIVK, OpMode.iABC, false) },
        };
    }
}
