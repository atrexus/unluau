using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Unluau.IR.Decoders;
using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Constants;
using Unluau.IR.ProtoTypes.Instructions;
using Unluau.IR.Versions;
using Type = Unluau.IR.ProtoTypes.Type;
using Version = Unluau.IR.Versions.Version;

namespace Unluau.IR
{
    /// <summary>
    /// Lifts the raw bytecode to an intermediate representation.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="Lifter"/>.
    /// </remarks>
    /// <param name="input">The input stream.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="source">The source file name.</param>
    public class Lifter(Stream input, ILoggerFactory loggerFactory, string source = "input-file.luau", Decoders.Decoder? decoder = null) : BinaryReader(input)
    {
        /// <summary>
        /// The bit that indicates if a type is optional or not.
        /// </summary>
        private const int TypeOptionalBit = 1 << 7;

        private readonly List<string> _symbolTable = [];
        private Version _version = new();
        private readonly List<ProtoType> _protoTypes = [];
        private readonly Decoders.Decoder _decoder = decoder ?? new();

        private readonly ILogger _logger = loggerFactory.CreateLogger<Lifter>();

        /// <summary>
        /// Creates a new instance of <see cref="Lifter"/>.
        /// </summary>
        /// <param name="fileInfo">The file.</param>
        public Lifter(FileInfo fileInfo, ILoggerFactory loggerFactory) : this(fileInfo.OpenRead(), loggerFactory, fileInfo.Name)
        {
        }

        /// <summary>
        /// Lifts the source from the stream.
        /// </summary>
        /// <returns>The result of the lifting.</returns>
        public LiftResult LiftSource()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var module = LiftModule();

            stopwatch.Stop();

            return new LiftResult
            {
                Module = module,
                ElapsedTime = stopwatch.Elapsed
            };
        }

        /// <summary>
        /// Lifts a <see cref="Module"/> from the stream.
        /// </summary>
        /// <returns>The module.</returns>
        public Module LiftModule()
        {
            // First we compute the checksum of the stream. We will need to reset the position of the stream after we are done.
            var checksum = new Checksum(MD5.Create(), BaseStream, source);
            BaseStream.Position = 0;

            _version = LiftVersion();

            _logger.LogInformation("lifting module with version {}", _version);

            // Now we read the symbol table. We will use this globally to reference strings.
            var stringCount = Read7BitEncodedInt();

            _logger.LogDebug("lifting {} strings", stringCount);

            for (var i = 0; i < stringCount; i++)
                _symbolTable.Add(ReadString());

            // Now we read all of the function prototypes in this module.
            var protoTypeCount = Read7BitEncodedInt();

            _logger.LogDebug("lifting {} prototypes", protoTypeCount);

            for (var i = 0; i < protoTypeCount; i++)
                _protoTypes.Add(LiftProtoType());

            // The entry point is the index of the function prototype that is the entry point.
            var entryPoint = Read7BitEncodedInt();

            _logger.LogDebug("entry point is prototype #{}", entryPoint);

            _protoTypes[entryPoint].IsMain = true;

            return new Module
            {
                Checksum = checksum,
                Version = _version,
                SymbolTable = _symbolTable,
                ProtoTypes = _protoTypes,
                EntryPoint = entryPoint
            };
        }

        /// <summary>
        /// Lifts a <see cref="ProtoType"/> from the stream.
        /// </summary>
        /// <returns>The function prototype.</returns>
        public ProtoType LiftProtoType()
        {
            var protoType = new ProtoType
            {
                MaxStackSize = ReadByte(),
                ParameterCount = ReadByte(),
                Upvalues = Enumerable.Repeat(new Upvalue(), ReadByte()).ToList(),
                IsVararg = ReadBoolean(),
            };

            using (_logger.BeginScope(protoType))
            {
                // Now we read type information of the function prototype if type encoding is enabled.
                if (_version is TypedVersion)
                {
                    // Now we read the flags of the function prototype. They contain information about the function.
                    protoType.Flags = new(ReadByte());

                    var typeSize = Read7BitEncodedInt();

                    // As of right now I see no way to use type info unless compiled natively. Therefore, we just skip it.
                    if (typeSize > 0)
                    {
                        _logger.LogWarning("skipping {} bytes of type information (not supported)", typeSize);
                        FillBuffer(typeSize);
                    }
                }

                // Now we read all of the instructions in the function prototype.
                var instructionCount = Read7BitEncodedInt();

                _logger.LogDebug("lifting {} instructions", instructionCount);

                for (int i = 0; i < instructionCount; i++)
                {
                    var instruction = LiftInstruction();

                    if (instruction.Aux != null)
                        i++;

                    protoType.Instructions.Add(instruction);
                }

                // Now we read the constants of the function prototype.
                protoType.Constants = LiftConstants();

                // Now we read the function prototypes in the function prototype.
                var protoTypeCount = Read7BitEncodedInt();

                for (var i = 0; i < protoTypeCount; i++)
                {
                    var fid = Read7BitEncodedInt();
                    protoType.ProtoTypes.Insert(i, _protoTypes[fid]);
                }

                protoType.LineDefined = Read7BitEncodedInt();
                protoType.LastLineDefined = null;
                protoType.Name = ReadStringRef();

                // Read the line information flag. If it is set, then we read the line information.
                if (ReadBoolean())
                {
                    _logger.LogDebug("lifting line information for prototype {:x}", protoType.GetHashCode());

                    // Now we read the line information for each instruction. 
                    var lineGapLog2 = ReadByte();

                    int intervals = (instructionCount - 1 >> lineGapLog2) + 1;
                    int absOffset = instructionCount + 3 & ~3;

                    int lineInfoCount = absOffset + intervals * sizeof(int);

                    var lineInfo = new byte[lineInfoCount];
                    var absLineInfo = new int[lineInfoCount - absOffset];

                    byte lastOffset = 0;
                    int lastLine = 0;

                    for (int i = 0; i < instructionCount; i++)
                        lineInfo[i] = lastOffset += ReadByte();

                    for (int i = 0; i < intervals; i++)
                        absLineInfo[i] = lastLine += ReadInt32();

                    // Now we assign the line information to the instructions. Each instruction has a line number.
                    int pc = 0;
                    foreach (var instruction in protoType.Instructions)
                    {
                        instruction.Context.LineDefined = absLineInfo[pc >> lineGapLog2] + lineInfo[pc];
                        instruction.Context.Pc = pc++;

                        // If the instruction has an auxiliary instruction, then we need to skip the next instruction.
                        if (instruction.Aux != null)
                        {
                            instruction.Aux.Context.LineDefined = absLineInfo[pc >> lineGapLog2] + lineInfo[pc];
                            instruction.Aux.Context.Pc = pc++;
                        }
                    }

                    protoType.LastLineDefined = protoType.Instructions.Last().Context.LineDefined;
                }

                // Read the debug information flag. If it is set, then we read the debug information.
                if (ReadBoolean())
                {
                    _logger.LogDebug("lifting debug information for prototype {:x}", protoType.GetHashCode());

                    var localVariableCount = Read7BitEncodedInt();

                    for (int i = 0; i < localVariableCount; i++)
                    {
                        var name = ReadStringRef();
                        var scope = (Read7BitEncodedInt(), Read7BitEncodedInt());
                        var register = ReadByte();

                        protoType.Locals.Add(new Local(register, scope)
                        {
                            Name = name
                        });
                    }

                    var upvalueCount = Read7BitEncodedInt();

                    for (int i = 0; i < upvalueCount; i++)
                    {
                        var name = ReadStringRef();

                        protoType.Upvalues[i].Name = name;
                    }
                }

                protoType.InstructionSize = instructionCount * 4;

            }

            return protoType;
        }

        /// <summary>
        /// Lifts an <see cref="Instruction"/> from the stream.
        /// </summary>
        public Instruction LiftInstruction()
        {
            // Now we will peek the next byte as it will be the opcode of the following instruction.
            var code = (OpCode)_decoder.DecodeOpCode(ReadByte());

            BaseStream.Position--;

            return code switch
            {
                OpCode.Nop => new Instruction(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Break => new Instruction(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.LoadNil => new InstructionA(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.LoadB => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.LoadN => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.LoadK => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Move => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.GetGlobal => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.SetGlobal => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.GetUpval => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SetUpval => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.CloseUpvals => new InstructionA(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.GetImport => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.GetTable => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SetTable => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.GetTableKS => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.SetTableKS => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.GetTableN => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SetTableN => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.NewClosure => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.NameCall => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.Call => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Return => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Jump => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.JumpBack => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.JumpIf => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.JumpIfNot => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.JumpIfEq => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpIfLe => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpIfLt => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpIfNotEq => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpIfNotLe => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpIfNotLt => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.Add => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Sub => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Mul => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Div => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Mod => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Pow => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.AddK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SubK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.MulK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.DivK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.ModK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.PowK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.And => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Or => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.AndK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.OrK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Concat => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Not => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Minus => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Length => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.NewTable => new InstructionAB(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.DupTable => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SetList => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.ForNPrep => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.ForNLoop => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.ForGLoop => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.ForGPrepINext => new InstructionA(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.ForGPrepNext => new InstructionA(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.NativeCall => new Instruction(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.GetVarArgs => new InstructionAB(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.DupClosure => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.PrepVarArgs => new InstructionA(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.LoadKX => new InstructionA(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpX => new InstructionE(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.FastCall => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Coverage => new InstructionE(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.Capture => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.SubRK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.DivRK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.FastCall1 => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.FastCall2 => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.FastCall2K => new InstructionABC(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.ForGPrep => new InstructionAD(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.JumpXEqKNil => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpXEqKB => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpXEqKN => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.JumpXEqKS => new InstructionAD(_decoder.DecodeInstruction(ReadUInt64())),
                OpCode.IDiv => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                OpCode.IDivK => new InstructionABC(_decoder.DecodeInstruction(ReadUInt32())),
                _ => throw new Exception($"Invalid opcode {code}")
            };
        }

        /// <summary>
        /// Lifts a list of <see cref="Constant"/>s from the stream.
        /// </summary>
        public List<Constant> LiftConstants()
        {
            var constantCount = Read7BitEncodedInt();

            _logger.LogDebug("lifting {} constants", constantCount);

            var constants = new Constant[constantCount];

            for (int i = 0; i < constantCount; i++)
            {
                byte c = ReadByte();

                if (!Enum.IsDefined(typeof(ConstantType), c))
                    throw new Exception($"Constant is not defined ({c})");

                ConstantType constantType = (ConstantType)c;

                switch (constantType)
                {
                    case ConstantType.Nil:
                        constants[i] = new NilConstant();
                        break;
                    case ConstantType.Boolean:
                        constants[i] = new BooleanConstant(ReadBoolean());
                        break;
                    case ConstantType.Number:
                        constants[i] = new NumberConstant(ReadDouble());
                        break;
                    case ConstantType.String:
                        // In previous versions the string was retrieved from the symbol table for simplicity. Now that
                        // an IL exists, there is no need to do this here.
                        constants[i] = new StringConstant(ReadStringRef()!);
                        break;
                    case ConstantType.Import:
                    {
                        var id = ReadUInt32(); // uint, because Negative values error out here
                        var nameCount = id >> 30;

                        var names = new List<StringConstant>();

                        // Load all of the string constants into the import constant. Luau does this differently. 
                        // I've decided to go with a loop here to conserve space.
                        for (int j = 0; j < nameCount; ++j)
                        {
                            var constantIndex = id >> 20 - j * 10 & 1023;

                            names.Add((StringConstant)constants[constantIndex]);
                        }

                        constants[i] = new ImportConstant(names);
                        break;
                    }
                    case ConstantType.Table:
                    {
                        var keyCount = Read7BitEncodedInt();

                        var keys = new List<Constant>(keyCount);

                        for (int j = 0; j < keyCount; ++j)
                        {
                            var keyIndex = Read7BitEncodedInt();

                            keys.Add(constants[keyIndex]);
                        }

                        constants[i] = new TableConstant(keys);
                        break;
                    }
                    case ConstantType.Closure:
                        constants[i] = new ClosureConstant(Read7BitEncodedInt());
                        break;
                    case ConstantType.Vector:
                        // This is a relatively new constant. It's most likely an optimization for the Roblox engine, since they use
                        // vectors so often.
                        constants[i] = new VectorConstant(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
                        break;
                }
            }

            return new(constants);
        }

        /// <summary>
        /// Lifts a <see cref="Version"/> from the stream.
        /// </summary>
        public Version LiftVersion()
        {
            var num = ReadByte();

            // If the version number is 0, then the rest of the stream is a string.
            // The string contains an error message that the script generated.
            if (num == 0)
                throw new Exception(ReadString((int)(BaseStream.Length - 1)));

            if (!Version.IsSupported(num))
                throw new Exception($"bytecode version mismatch (expected {Version.MinVersion}..{Version.MaxVersion}, got {num})");

            if (Version.IsTyped(num))
                return new TypedVersion(num, (TypedVersionKind)ReadByte());

            return new Version(num);
        }

        /// <summary>
        /// Reads a string with a length of <paramref name="length"/> from the stream.
        /// </summary>
        public string ReadString(int? length = null)
        {
            length ??= Read7BitEncodedInt();

            var s = Encoding.ASCII.GetString(ReadBytes(length.Value));

            var builder = new StringBuilder();

            foreach (var c in s)
            {
                switch (c)
                {
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\'':
                        builder.Append("\\'");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '\v':
                        builder.Append("\\v");
                        break;
                    case '\a':
                        builder.Append("\\a");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    default:
                        builder.Append(c);
                        break;

                }

            }

            return builder.ToString();
        }

        /// <summary>
        /// Reads a string reference from the stream.
        /// </summary>
        /// <returns>The string.</returns>
        public string? ReadStringRef()
        {
            var index = Read7BitEncodedInt();

            return index == 0 ? null : _symbolTable[index - 1];
        }

        /// <summary>
        /// Reads a type from the stream.
        /// </summary>
        public Type ReadType()
        {
            var b = ReadByte();

            byte tag = (byte)(b & ~TypeOptionalBit);

            if (!Enum.IsDefined(typeof(TypeKind), tag))
                throw new Exception($"invalid type tag {tag}");

            return new((TypeKind)tag, (b & TypeOptionalBit) != 0);
        }
    }
}
