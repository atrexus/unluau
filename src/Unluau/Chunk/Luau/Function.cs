using System.Text;
using Unluau.IL;
using Unluau.IL.Statements.Instructions;
using Unluau.IL.Statements;
using Unluau.IL.Statements.Blocks;
using Unluau.IL.Values;
using Unluau.IL.Values.Conditions;
using Unluau.Utils;
using Index = Unluau.IL.Values.Index;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// Represents a function in a Luau bytecode chunk.
    /// </summary>
    public class Function
    {
        private readonly string[] _symbolTable;

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
        /// The index in the symbol table that contains the name of the current function.
        /// </summary>
        public int? DebugSymbolIndex { get; private set; }

        /// <summary>
        /// Contains flags..? 
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
                        var id = reader.ReadInt32();
                        var nameCount = id >> 30;

                        var names = new StringConstant[nameCount];

                        // Load all of the string constants into the import constant. Luau does this differently. 
                        // I've decided to go with a loop here to conserve space.
                        for (int j = 0; j < nameCount; ++j)
                        {
                            var constantIndex = (id >> (20 - j * 10)) & 1023;

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
        /// Reads a list of closure indices using a binary reader.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>List of closure indices.</returns>
        private static int[] ReadClosureTable(BinaryReader reader)
        {
            var closureCount = reader.ReadSize();

            var closureTable = new int[closureCount];

            for (int i = 0; i < closureCount; ++i)
                closureTable[i] = reader.ReadSize();

            return closureTable;
        }

        public override string ToString()
        {
            StringBuilder builder = new();

            // Write function header.
            builder.AppendLine($"{ParameterCount}{(IsVariadic ? "+" : string.Empty)} param(s), {SlotCount} slot(s), {UpvalueCount} upvalue(s), {Constants.Length} constant(s), {ClosureTable.Length} function(s)");

            // Write function name
            builder.Append($"function {(DebugSymbolIndex is null ? "main" : _symbolTable[(int)DebugSymbolIndex])}(");

            // Write function parameters
            for (int i = 0; i < ParameterCount; ++i)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append($"v{i + 1}");
            }

            // Write vararg "..." if provided
            if (IsVariadic)
                builder.Append($"{(ParameterCount > 0 ? ", " : string.Empty)}...");

            // Write line information if available
            builder.AppendLine($"){(LineInformation is null ? string.Empty : $" -- line {LineInformation.GetLine(0)} through {LineInformation.GetLine(Instructions.Length - 1)}")}");

            var fmt = new string('0', (int)Math.Floor(Math.Log10(Instructions.Length - 1) + 1) + 1);

            for (int i = 0; i < Instructions.Length; ++i)
            {
                var instruction = Instructions[i];
                var properties = OpProperties.Map[instruction.Code];

                builder.Append(i.ToString(fmt));

                switch (properties.Mode)
                {
                    case OpMode.iABC:
                        builder.Append(string.Format("   {0, -10}\t {1, 5} {2} {3}", properties.Code.ToString(),
                            instruction.A, instruction.B, instruction.C));
                        break;
                    case OpMode.iAD:
                        builder.Append(string.Format("   {0, -10}\t {1, 5} {2}", properties.Code.ToString(),
                            instruction.A, instruction.D));
                        break;
                    case OpMode.iE:
                        builder.Append(string.Format("   {0, -10}\t {1, 5}", properties.Code.ToString(),
                            instruction.E));
                        break;
                }

                builder.AppendLine();

                if (properties.HasAux)
                {
                    builder.Append((i + 1).ToString(fmt));
                    builder.Append(string.Format("   {0, -10}\t {1, 5}\n", "   AUX", Instructions[++i].Value));
                }
            }

            builder.Append($"end");

            return builder.ToString();
        }

        /// <summary>
        /// Lifts the current function to a new IL closure.
        /// </summary>
        /// <returns>An IL closure.</returns>
        public Closure Lift()
        {
            var context = GetClosureContext();
            var stack = new Stack();

            var body = LiftBasicBlock(stack, 0);

            return new(context, body);
        }

        public BasicBlock LiftBasicBlock(Stack stack, int startPc, int? endPc = null)
        {
            var pc = startPc;
            var block = new BasicBlock(Context.Empty);

            // We could set up an infinite loop, but I hate them. By setting up a loop like this, we can 
            // easily break once we have processed all instructions.
            for (; pc < (endPc ?? Instructions.Length); ++pc)
            {
                var instruction = Instructions[pc];
                var context = GetContext(pc, pc);

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

                        var ra = stack.Get(instruction.A);

                        if (ra != null && ra.Value.Context.PcScope.Item1 < startPc)
                            ra = stack.Update(ra.Id, value);
                        else
                        {
                            if (ra != null)
                                ra.References--;
                            ra = stack.Set(instruction.A, value);
                        }
                            

                        // Note: loading a value onto the stack can be for both for constant values and environment variables.
                        // It doesn't really matter what kind we have because when we generate our AST they are treated the same.
                        block.Statements.Add(new LoadValue(context, ra, value));
                        break;
                    }
                    case OpCode.FASTCALL:
                    case OpCode.FASTCALL1:
                    case OpCode.FASTCALL2:
                    case OpCode.FASTCALL2K:
                    case OpCode.CALL:
                    {
                        // Unlike the simple CALL instruction, all FASTCALL instructions contain an identifier to a built in function
                        // as the first argument.
                        BasicValue function = instruction.Code == OpCode.CALL
                            ? new Reference(context, stack.Get(instruction.A)!)
                            : new BasicValue<string>(context, Builtin.IdToName(instruction.A));

                        List<BasicValue> arguments = [];

                        if (instruction.Code == OpCode.FASTCALL2K)
                        {
                            // The FASTCALL2K instruction is unique. Unlike the other FASTCALL instructions, you can't just jump to the call
                            // without processing the following instructions. The auxiliary instruction contains the constant index.
                            arguments.Add(new Reference(context, stack.Get(instruction.B)!));
                            arguments.Add(ConstantToBasicValue(context, Constants[Instructions[++pc].Value]));
                        }

                        // The C operand of the FASTCALL instruction is the jump index for the CALL instruction.
                        if (instruction.Code != OpCode.CALL)
                            instruction = Instructions[pc += instruction.C + 1];

                        if (instruction.Code != OpCode.FASTCALL2K)
                        {
                            // When CALL's B operand is 0, the instruction is LUA_MULTRET. This means that its arguments start 
                            // at R(A) and go up to the top of the stack.
                            var argCount = instruction.B > 0 ? instruction.B : (stack.Top.Id - instruction.A) + 1;

                            for (int i = 1; i < argCount; ++i)
                                arguments.Add(new Reference(context, stack.Get(instruction.A + i)!));
                        }

                        // Pop the stack frame (stack are freed and are now 'dead')
                        stack.FreeFrame(instruction.A);

                        // Create our new call result, that we will load onto the stack.
                        var callResult = new CallResult(context, function, [.. arguments]);

                        var results = new List<Slot>();

                        // Load all of the results onto the stack. 
                        for (int slot = 0; slot < instruction.C - 1; ++slot)
                            results.Add(stack.Set(slot + instruction.A, callResult));

                        block.Statements.Add(new Call(context, callResult, [.. results]));
                        break;
                    }
                    case OpCode.GETTABLE:
                    case OpCode.GETTABLEN:
                    case OpCode.GETTABLEKS:
                    case OpCode.NAMECALL:
                    {
                        // This is our indexable value. In Luau its always a table.
                        var table = new Reference(context, stack.Get(instruction.B)!);

                        var value = instruction.Code switch
                        {
                            OpCode.GETTABLE => new Reference(context, stack.Get(instruction.C)!),

                            // The GETTABLEN instruction contains a value (byte) from 1 to 256. Because the C operand can only hold
                            // a value as large as 255, we need to add 1 to it.
                            OpCode.GETTABLEN => new BasicValue<int>(context, instruction.C + 1),

                            // Both GETTABLEKS and NAMECALL contain a constant in the auxiliary instruction. We group them here for 
                            // simplicity.
                            OpCode.GETTABLEKS or OpCode.NAMECALL => ConstantToBasicValue(context, Constants[Instructions[++pc].Value]),

                            // We know this won't ever happen, but the C# compiler will cry if I don't add this.
                            _ => throw new NotSupportedException()
                        };

                        var index = new Index(context, table, value);

                        var ra = stack.Set(instruction.A, index);

                        var getIndex = new GetIndex(context, ra, index);

                        // The NAMECALL instruction is special. It tells the VM that we have a call proceeding and that the call should pass 
                        // a pointer to the instance of this table. We mark this by using a separate instruction.
                        if (instruction.Code == OpCode.NAMECALL)
                        {
                            // Set the next register to the instance to the table that is being invoked.
                            stack.Set(instruction.A + 1, table.Slot.Value);

                            getIndex = new GetIndexSelf(getIndex);
                        }

                        block.Statements.Add(getIndex);
                        break;
                    }
                    case OpCode.MOVE:
                    {
                        var rb = stack.Get(instruction.B)!;
                        var ra = stack.Set(instruction.A, rb.Value);

                        block.Statements.Add(new Move(context, ra, rb));
                        break;
                    }
                    case OpCode.JUMPIFEQ:
                    case OpCode.JUMPIFLE:
                    case OpCode.JUMPIFLT:
                    case OpCode.JUMPIFNOTEQ:
                    case OpCode.JUMPIFNOTLE:
                    case OpCode.JUMPIFNOTLT:
                    case OpCode.JUMPIF:
                    case OpCode.JUMPIFNOT:
                    {
                        // Save the original pc value so that we can conserve the jump offset.
                        var ogPc = pc;

                        var left = new Reference(context, stack.Get(instruction.A)!);

                        var right = instruction.Code switch
                        {
                            // These operations just test a value and have one operand. 
                            OpCode.JUMPIF or OpCode.JUMPIFNOT => null,

                            _ => new Reference(context, stack.Get(Instructions[++pc].Value)!)
                        };

                        // Build the condition based on the current operation code.
                        BasicCondition condition = instruction.Code switch
                        {
                            // Performs a jump if values are not equal to each other.
                            OpCode.JUMPIFNOTEQ => new Equals(context, left, right!),

                            // Jumps if the register is nil or false.
                            OpCode.JUMPIFNOT => new Test(context, left),
                            OpCode.JUMPIF => new NotTest(context, left),

                            // This should never happen, but just in case.
                            _ => throw new NotImplementedException()
                        };

                        // Now we lift the body of the block.
                        var body = LiftBasicBlock(stack, ++pc, pc + instruction.D - (pc - ogPc - 1));

                        block.Statements.Add(new IfBlock(body.Context, condition, body.Statements));
                        break;
                    }
                }
            }

            block.Context = GetContext(startPc, pc);

            return block;
        }

        private BasicValue ConstantToBasicValue(Context context, Constant constant)
        {
            if (constant is StringConstant stringConstant)
                return new BasicValue<string>(context, _symbolTable[stringConstant.Value - 1]);

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
                    names[i] = _symbolTable[importConstant.Value[i].Value - 1];

                return new Global(context, names);
            }

            throw new NotImplementedException();
        }

        private ClosureContext GetClosureContext()
        {
            (int, int) pcScope = (0, Instructions.Length - 1);
            (int, int) lines = (LineDefined, LineInformation?.GetLine(pcScope.Item2) ?? LineDefined);

            List<Variable> parameters = [];

            for (int slot = 0; slot < ParameterCount; ++slot)
                parameters.Add(new Variable(new(pcScope, lines), slot));

            string? name = DebugSymbolIndex is null ? null : _symbolTable[(int)DebugSymbolIndex];

            return new()
            {
                Context = new(pcScope, lines),
                IsVariadic = IsVariadic,
                Parameters = [.. parameters],
                Symbol = name
            };
        }

        private Context GetContext(int startPc, int endPc) => new((startPc, endPc), LineInformation?.GetLines(startPc, endPc));
    }
}
