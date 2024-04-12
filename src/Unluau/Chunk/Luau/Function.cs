using System.Text;
using Unluau.IL;
using Unluau.IL.Statements.Instructions;
using Unluau.IL.Statements;
using Unluau.IL.Statements.Blocks;
using Unluau.IL.Values;
using Unluau.IL.Values.Conditions;
using Unluau.Utils;
using Index = Unluau.IL.Values.Index;
using Microsoft.Win32;

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
        /// The list of upvalue slots (aka "up slots").
        /// </summary>
        public List<Slot> UpSlots { get; private set; }

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

            UpSlots = new(UpvalueCount);

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
            DebugSymbolIndex = reader.ReadIndex() - 1;

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
                        var id = reader.ReadUInt32(); // uint, because Negative values error out here
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
                        constants[i] = new VectorConstant(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
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
        public Closure Lift(LuauChunk chunk)
        {
            var context = GetClosureContext();
            var stack = new Stack();

            // Now we load the parameters to this closure onto the stack.
            foreach (var variable in context.Parameters)
                stack.Set(variable.Slot, variable);

            var body = LiftBasicBlock(chunk, stack, 0);

            return new(context, body);
        }

        public BasicBlock LiftBasicBlock(LuauChunk chunk, Stack stack, int startPc, int? endPc = null)
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
                    case OpCode.NEWTABLE:
                    case OpCode.DUPTABLE:
                    case OpCode.CONCAT:
                    case OpCode.LENGTH:
                    case OpCode.MINUS:
                    case OpCode.NOT:
                    case OpCode.NEWCLOSURE:
                    case OpCode.DUPCLOSURE:
                    case OpCode.GETUPVAL:
                    {
                        int aux = Instructions[pc + 1].Value;

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

                            // The size of the table varies. If the auxiliary instruction contains a value that is not
                            // zero, then we use it. Otherwise we switch to the B operand.
                            OpCode.NEWTABLE => new Table(context, aux > 0 ? Instructions[pc + 1].Value : (instruction.B > 0 ? (1 << (instruction.B - 1)) : 0)),
                            OpCode.DUPTABLE => ConstantToBasicValue(context, Constants[instruction.D]),

                            // This instruction concatenates the list of registers together in descending order. 
                            OpCode.CONCAT => new Concat(context, BuildConcatList(context, stack, instruction)),

                            // Unary operations...
                            OpCode.LENGTH or 
                            OpCode.MINUS or 
                            OpCode.NOT => BuildUnaryValue(context, stack, instruction),

                            // Loads an upvalue into a target register, R(A).
                            OpCode.GETUPVAL => new Reference(context, UpSlots[instruction.B]),

                            // Loading closures is a complex task. The NEWCLOSURE instruction creates a new closure from a child function prototype.
                            // The DUPCLOSURE creates a new closure from a pre-created function object (constants).
                            OpCode.NEWCLOSURE => new ClosureValue(context, ClosureTable[instruction.D]),
                            OpCode.DUPCLOSURE => ConstantToBasicValue(context, Constants[instruction.D]),

                            // We know this won't ever happen, but the C# compiler will cry if I don't add this.
                            _ => throw new NotSupportedException()
                        };

                        // If we are loading a closure (DUPCLOSURE or NEWCLOSURE), we need to capture all of the upvalues. These follow 
                        // in a chain of CAPTURE operations.
                        if (instruction.Code == OpCode.NEWCLOSURE || instruction.Code == OpCode.DUPCLOSURE)
                        {
                            ++pc; // Point at the first CAPTURE instruction.
                            CaptureUpvalues(chunk, stack, ref pc, (ClosureValue)value);
                        }

                        // Skip the AUX instruction for the following operation codes
                        if (instruction.Code == OpCode.GETIMPORT || instruction.Code == OpCode.NEWTABLE)
                            pc++;

                        // Here we check to see if the slot we are loading our value into is initialized or not. If it has, then we
                        // check to see if this value has been set within the current block. If not then we reset it, otherwise we 
                        // just update it.
                        var ra = stack.SetScoped(instruction.A, value, startPc);

                        // Note: loading a value onto the stack can be for both for constant values and environment variables.
                        // It doesn't really matter what kind we have because when we generate our AST they are treated the same.
                        block.Statements.Add(new LoadValue(context, ra, value));
                        break;
                    }
                    case OpCode.SETUPVAL:
                    {
                        // We handle the SETUPVAL instruction a little differently. Instead of setting R(A) we set the upvalue slot instead (R(B)).
                        // This way we can minimize the number of IL instructions and keep a clean codebase.
                        block.Statements.Add(new LoadValue(context, UpSlots[instruction.B], new Reference(context, stack.Get(instruction.A)!)));
                        break;
                    }
                    case OpCode.SETGLOBAL:
                    {
                        var value = ConstantToBasicValue(context, Constants[Instructions[++pc].Value]);
                        
                        // Now we convert the constant to a global type. Sometime the SETGLOBAL instruction will invoke a string constant,
                        // so we need to account for that.
                        var global = value is BasicValue<string> stringLiteral 
                            ? new Global(stringLiteral.Context, [stringLiteral.Value!]) 
                            : (Global)value;


                        block.Statements.Add(new SetGlobal(context, global, new Reference(context, stack.Get(instruction.A)!)));
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


                            // FASTCALL2K's C, is meant to jump to CALL, but it skipped it due to +1
                            // ++pc was used before, so it's not needed anymore?
                            instruction = Instructions[pc += instruction.C];
                        }

                        // The C operand of the FASTCALL instruction is the jump index for the CALL instruction.
                        else if (instruction.Code != OpCode.CALL)
                            instruction = Instructions[pc += instruction.C + 1];

                        if (instruction.Code != OpCode.FASTCALL2K)
                        {
                            // When CALL's B operand is 0, the instruction is LUA_MULTRET. This means that its arguments start 
                            // at R(A) and go up to the top of the stack.
                            var argCount = instruction.B > 0 ? instruction.B : stack.Top.Id - instruction.A + 1;

                            for (int i = 1; i < argCount; ++i)
                                arguments.Add(new Reference(context, stack.Get(instruction.A + i)!));
                        }

                        // Pop the stack frame (stack are freed and are now 'dead')
                        stack.FreeFrame(instruction.A);

                        // Create our new call result, that we will load onto the stack.
                        var callResult = new CallResult(context, function, [.. arguments]);

                        var results = new List<Slot>();

                        // Calculate the number of return values
                        var retCount = instruction.C > 0 ? instruction.C - 1 : stack.Top.Id - instruction.A + 1;

                        // Load all of the results onto the stack. 
                        for (int slot = 0; slot < retCount; ++slot)
                            results.Add(stack.SetScoped(slot + instruction.A, callResult, startPc));

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
                    case OpCode.SETTABLEKS:
                    case OpCode.SETTABLE:
                    case OpCode.SETTABLEN:
                    {
                        // This is our indexable value. In Luau its always a table.
                        var table = new Reference(context, stack.Get(instruction.B)!);

                        // Gets the value for the index.
                        var indexValue = instruction.Code switch
                        { 
                            OpCode.SETTABLE => new Reference(context, stack.Get(instruction.C)!),

                            // The SETTABLEN instruction contains a value (byte) from 1 to 256. Because the C operand can only hold
                            // a value as large as 255, we need to add 1 to it.
                            OpCode.SETTABLEN => new BasicValue<int>(context, instruction.C + 1),

                            // The SETTABLEKS instruction contains a constant in the auxiliary instruction. We group them here for 
                            // simplicity.
                            OpCode.SETTABLEKS => ConstantToBasicValue(context, Constants[Instructions[++pc].Value]),

                            // We know this won't ever happen, but the C# compiler will cry if I don't add this.
                            _ => throw new NotSupportedException()
                        };

                        var index = new Index(context, table, indexValue);

                        // Now we get the value of the set instruction (what we are setting the index to).
                        var value = new Reference(context, stack.Get(instruction.A)!);

                        block.Statements.Add(new SetIndex(context, index, value));
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

                            // Performs a jump if values are not equal to each other.
                            OpCode.JUMPIFEQ => new NotEquals(context, left, right!),

                            // Jumps if the register is nil or false.
                            OpCode.JUMPIFNOT => new Test(context, left),
                            OpCode.JUMPIF => new NotTest(context, left),

                            // This should never happen, but just in case.
                            _ => throw new NotImplementedException()
                        };

                        // Now we lift the body of the block.
                        var body = LiftBasicBlock(chunk, stack, pc + 1, pc += instruction.D - (pc - ogPc - 1));

                        // We decrement the program counter because the for loop will increment it on the next pass.
                        --pc;

                        block.Statements.Add(new IfBlock(body.Context, condition, body.Statements));
                        break;
                    }
                    case OpCode.SETLIST:
                    {
                        // This instruction doesn't generate any IL instructions. Instead it initializes a table as an array,
                        // and assigns all of its values to it.
                        var table = (Table)stack.Get(instruction.A)!.Value;

                        for (int i = 0; i < instruction.C - 1; ++i)
                        {
                            var entry = stack.Get(instruction.B + i)!;

                            table.Entries.Add(new()
                            {
                                Context = entry.Value.Context,
                                Value = new Reference(context, entry)
                            });
                        }

                        // Skip the auxiliary instruction
                        ++pc;
                        break;
                    }
                    case OpCode.RETURN:
                    {
                        // Again this instruction uses LUA_MULTRET, just like the call instruction. If the `B` operand is empty,
                        // then we pop all the values off of the stack and use those as our values. Otherwise, `B-1` contains our
                        // value count.
                        var valueCount = instruction.B > 0 ? instruction.B - 1 : stack.Top.Id - instruction.A + 1;

                        var values = new BasicValue[valueCount];

                        for (int i = 0; i < valueCount; ++i)
                            values[i] = new Reference(context, stack.Get(instruction.A + i)!);

                        // Note: We always add a return statement, even if this is the block of the main closure. Whether or not to 
                        // display the `return` of the main block is configurable in the AST builder.
                        block.Statements.Add(new Return(context, values));
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

            else if (constant is TableConstant tableConstant)
            {
                var entries = new TableEntry[tableConstant.Value.Length];

                for (int i = 0; i < tableConstant.Value.Length; ++i)
                    entries[i] = new()
                    {
                        Context = context,
                        Value = ConstantToBasicValue(context, tableConstant.Value[i])
                    };

                return new Table(context, entries);
            }

            else if (constant is ClosureConstant closureConstant)
                return new ClosureValue(context, closureConstant.Value);

            throw new NotImplementedException();
        }

        private static BasicValue[] BuildConcatList(Context context, Stack stack, Instruction instruction)
        {
            var values = new BasicValue[instruction.C - instruction.B + 1];

            for (var i = instruction.B; i < instruction.C + 1; ++i)
                values[i - instruction.B] = new Reference(context, stack.Get(i)!);

            return values;
        }

        private static Unary BuildUnaryValue(Context context, Stack stack, Instruction instruction)
        {
            return new(context, instruction.Code switch
            {
                OpCode.LENGTH => UnaryType.Length,
                OpCode.MINUS => UnaryType.Minus,
                OpCode.NOT => UnaryType.Not,
                _ => throw new NotSupportedException()
            }, new Reference(context, stack.Get(instruction.B)!));
        }

        private void CaptureUpvalues(LuauChunk chunk, Stack stack, ref int pc, ClosureValue value)
        {
            for (var instruction = Instructions[pc]; instruction.Code == OpCode.CAPTURE; instruction = Instructions[++pc])
            {
                var upSlot = (CaptureType)instruction.A switch
                {
                    // These can be used interchangeably when lifting. These are different in the VM however.
                    CaptureType.Reference or CaptureType.Value => stack.MarkUpValue(instruction.B),
                    CaptureType.UpValue => UpSlots[instruction.B],
                    _ => throw new NotImplementedException()
                };

                var function = chunk.Functions[value.Value];
                
                function.UpSlots.Add(upSlot);
            }
            --pc;
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
