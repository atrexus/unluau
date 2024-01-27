using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Reflection.Metadata.Ecma335;
using Unluau.IL;
using Unluau.IL.Blocks;
using Unluau.IL.Instructions;
using Unluau.IL.Values;
using Unluau.Utils;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// Represents a function in a Luau bytecode chunk.
    /// </summary>
    public class Function
    {
        private string[] _symbolTable;

        /// <summary>
        /// The total number of register slots the function requires.
        /// </summary>
        public byte SlotCount { get; private set; }

        /// <summary>
        /// The number of parameters the function requires.
        /// </summary>
        public byte ParameterCount { get; private set; }

        /// <summary>
        /// The total number of upvalues the function uses.
        /// </summary>
        public byte UpvalueCount { get; private set; }

        /// <summary>
        /// If true, the current function is variadic ('...').
        /// </summary>
        public bool IsVariadic { get; private set; }

        /// <summary>
        /// A list of instructions for the virtual machine to execute.
        /// </summary>
        public Instruction[] Instructions { get; private set; }

        /// <summary>
        /// A list of constants that the instructions reference.
        /// </summary>
        public Constant[] Constants { get; private set; }

        /// <summary>
        /// A list of indices pointing to locations in the chunk's closure array.
        /// </summary>
        public int[] ClosureTable { get; private set; }

        /// <summary>
        /// The line that the function was defined on.
        /// </summary>
        public int LineDefined { get; private set; }

        /// <summary>
        /// The index in the symbol table that constains the name of the current function.
        /// </summary>
        public int? DebugSymbolIndex { get; private set; }

        /// <summary>
        /// Constains flags..? 
        /// </summary>
        public byte? Flags { get; private set; }

        /// <summary>
        /// Contains information about all the types in the function (currently useless).
        /// </summary>
        public byte[]? TypeInformation { get; private set; }

        /// <summary>
        /// Contains information on which lines an instruction was on.
        /// </summary>
        public LineInformation? LineInformation { get; private set; }

        /// <summary>
        /// Contains information about local variables and upvalues (for debugging).
        /// </summary>
        public DebugInformation? DebugInformation { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Function"/> from the binary reader.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="version">The version of the bytecode.</param>
        public Function(BinaryReader reader, Version version, string[] symbolTable)
        {
            _symbolTable = symbolTable;

            SlotCount = reader.ReadByte();
            ParameterCount = reader.ReadByte();
            UpvalueCount = reader.ReadByte();
            IsVariadic = reader.ReadBoolean();

            // TODO: Make this a method once they finally drop their type encoding.
            if (version.HasTypeEncoding)
            {
                Flags = reader.ReadByte();

                var typesCount = reader.ReadSize();

                if (typesCount > 0 && version.TypesVersion == TypesVersionKind.Version1)
                {
                    TypeInformation = new byte[typesCount];

                    for (int i = 0; i < typesCount; i++)
                        TypeInformation[i] = reader.ReadByte();
                }
            }

            Instructions = ReadInstructions(reader);
            Constants = ReadConstants(reader);
            ClosureTable = ReadClosureTable(reader);

            LineDefined = reader.ReadSize();
            DebugSymbolIndex = reader.ReadIndex();

            if (reader.ReadBoolean())
                LineInformation = new LineInformation(reader, Instructions.Length);

            if (reader.ReadBoolean())
                DebugInformation = new DebugInformation(reader);
        }

        /// <summary>
        /// Reads a list of instructions using the binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to use.</param>
        /// <returns>A list of instructions.</returns>
        private static Instruction[] ReadInstructions(BinaryReader reader)
        {
            var instructionCount = reader.ReadSize();

            var instructions = new Instruction[instructionCount];

            for (int i = 0; i < instructionCount; i++)
                instructions[i] = new(reader.ReadInt32());

            return instructions;
        }

        /// <summary>
        /// Reads a list of constants using the binary reader.
        /// </summary>
        /// <param name="reader">The binary reader to use.</param>
        /// <returns>A list of constant values.</returns>
        private static Constant[] ReadConstants(BinaryReader reader)
        {
            var constantCount = reader.ReadSize();

            var constants = new Constant[constantCount];

            for (int i = 0; i < constantCount; i++)
            {
                byte c = reader.ReadByte();

                if (!Enum.IsDefined(typeof(ConstantType), c))
                    throw new Exception($"Constant is not defined ({c})");

                ConstantType constantType = (ConstantType)c;

                switch (constantType)
                {
                    case ConstantType.Nil:
                        constants[i] = new NilConstant();
                        break;
                    case ConstantType.Bool:
                        constants[i] = new BoolConstant(reader.ReadBoolean());
                        break;
                    case ConstantType.Number:
                        constants[i] = new NumberConstant(reader.ReadDouble());
                        break;
                    case ConstantType.String:
                        // In previous versions the string was retrieved from the symbol table for simplicity. Now that
                        // an IL exists, there is no need to do this here.
                        constants[i] = new StringConstant(reader.ReadSize());
                        break;
                    case ConstantType.Import:
                    {
                        var nameCount = reader.ReadInt32() >> 30;

                        var names = new StringConstant[nameCount];

                        // Load all of the string constants into the import constant. Luau does this differently. 
                        // I've decided to go with a loop here to conserve space.
                        for (int j = 0; j < nameCount; ++j)
                        {
                            var constantIndex = ((20 - j * 10) >> 20) & 1023;

                            names[j] = (StringConstant)constants[constantIndex];
                        }

                        constants[i] = new ImportConstant(names);
                        break;
                    }
                    case ConstantType.Table:
                    {
                        var keyCount = reader.ReadSize();

                        var keys = new Constant[keyCount];

                        for (int j = 0; j < keyCount; ++j)
                        {
                            var keyIndex = reader.ReadSize();

                            keys[j] = constants[keyIndex];
                        }

                        constants[i] = new TableConstant(keys);
                        break;
                    }
                    case ConstantType.Closure:
                        constants[i] = new ClosureConstant(reader.ReadSize());
                        break;
                    case ConstantType.Vector:
                        // This is a relatively new constant. It's most likely an optimization for the Roblox engine, since they use
                        // vectors so often.
                        constants[i] = new VectorConstant([reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()]);
                        break;
                }
            }

            return constants;
        }

        /// <summary>
        /// Reads a list of closure indeces using a binary reader.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>List of closure indeces.</returns>
        private static int[] ReadClosureTable(BinaryReader reader)
        {
            var closureCount = reader.ReadSize();

            var closureTable = new int[closureCount];

            for (int i = 0; i < closureCount; ++i)
                closureTable[i] = reader.ReadSize();

            return closureTable;
        }

        /// <summary>
        /// Lifts the current function to a new IL closure.
        /// </summary>
        /// <returns>An IL closure.</returns>
        public Closure Lift()
        {

        }

        public BasicBlock LiftBasicBlock(ref int pc)
        {
            var startPc = pc;
            var instructions = new List<IL.Instructions.Instruction>();

            // We could set up an infinate loop, but I hate them. By setting up a loop like this, we can 
            // easily break once we have processed all instructions.
            for (; pc < Instructions.Length; ++pc)
            {
                var instruction = Instructions[pc];
                var context = GetContext(pc, pc);
                var top = 0;

                switch (instruction.Code)
                {
                    case OpCode.LOADNIL:
                    case OpCode.LOADN:
                    case OpCode.LOADB:
                    case OpCode.LOADKX:
                    case OpCode.LOADK:
                    case OpCode.GETGLOBAL:
                    case OpCode.GETIMPORT:
                    {
                        // Not we update top, since this is the last register loaded.
                        var ra = top = instruction.A;

                        var value = instruction.Code switch
                        {
                            OpCode.LOADK or OpCode.GETIMPORT => ConstantToBasicValue(context, Constants[instruction.D]),

                            // Both LOADKX and GETGLOBAL have constants in the auxiliary instruction. We can group
                            // them for simplicity.
                            OpCode.LOADKX or
                            OpCode.GETGLOBAL => ConstantToBasicValue(context, Constants[Instructions[++pc].Value]),
                            OpCode.LOADB => new BasicValue<bool>(context, instruction.B == 1),
                            OpCode.LOADN => new BasicValue<int>(context, instruction.D),
                            OpCode.LOADNIL => new BasicValue<object>(context, null),

                            // We know this won't ever happen, but the C# compiler will cry if I don't add this.
                            _ => throw new NotSupportedException()
                        };

                        // Note: loading a value onto the stack can be for both for constant values and environment variables.
                        // It doesn't really matter what kind we have because when we generate our AST they are treated the same.
                        instructions.Add(new LoadValue(context, ra, value));
                        break;
                    }
                    case OpCode.FASTCALL:
                    case OpCode.FASTCALL1:
                    case OpCode.FASTCALL2:
                    case OpCode.FASTCALL2K:
                    case OpCode.CALL:
                    {
                        // Unlike the simple CALL instruction, all FASTCALL instructions contain an identifier to a builtin function
                        // as the first argument.
                        BasicValue function = instruction.Code == OpCode.CALL
                            ? new Reference(context, instruction.A)
                            : new BasicValue<string>(context, Builtin.IdToName(instruction.A));

                        List<BasicValue> arguments = [];

                        if (instruction.Code == OpCode.FASTCALL2K)
                        {
                            // The FASTCALL2K instruction is unique. Unlike the other FASTCALL instructions, you can't just jump to the call
                            // without processing the following instructions. The auxiliary instruction contains the constant index.
                            arguments.Add(new Reference(context, instruction.B));
                            arguments.Add(ConstantToBasicValue(context, Constants[Instructions[++pc].Value]));
                        }

                        // The C operand of the FASTCALL instruction is the jump index for the CALL instruction.
                        if (instruction.Code != OpCode.CALL)
                            instruction = Instructions[pc += instruction.C + 1];

                        if (instruction.Code != OpCode.FASTCALL2K)
                        {
                            // When CALL's B operand is 0, the instruction is LUA_MULTRET. This means that its arguments start 
                            // at R(A) and go up to the top of the stack.
                            var argCount = instruction.B > 0 ? instruction.B : (top - instruction.A) + 1;

                            for (int i = 1; i < argCount; ++i)
                                arguments.Add(new Reference(context, instruction.A + i));
                        }

                        int? results = instruction.B == 1 ? null : instruction.B - 1;

                        instructions.Add(new Call(context, function, arguments.ToArray(), results));
                        break;
                    }
                }
            }

            return new(GetContext(startPc, pc), [.. instructions]);
        }

        private BasicValue ConstantToBasicValue(Context context, Constant constant)
        {
            if (constant is StringConstant stringConstant)
                return new BasicValue<string>(context, _symbolTable[stringConstant.Value]);

            else if (constant is NumberConstant numberConstant)
                return new BasicValue<double>(context, numberConstant.Value);

            else if (constant is BoolConstant boolConstant)
                return new BasicValue<bool>(context, boolConstant.Value);

            else if (constant is NilConstant)
                return new BasicValue<object>(context, null);

            else if (constant is ImportConstant importConstant)
            {
                var names = new string[importConstant.Value.Length];

                for (int i = 0; i < names.Length; ++i)
                    names[i] = _symbolTable[importConstant.Value[i].Value];

                return new Global(context, names);
            }

            throw new NotImplementedException();
        }

        private BasicValue[] MakeReferenceList(Context context, int from, int to)
        {
            var ret = new List<BasicValue>();

            for (int i = from; i <= to; ++i)
                ret.Add(new Reference(context, i));

            return [.. ret];
        }

        private Context GetContext(int startPc, int endPc) => new((startPc, endPc), LineInformation?.GetLines(startPc, endPc));
    }
}
