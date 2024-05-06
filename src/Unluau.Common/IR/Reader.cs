using System.Text;
using Unluau.Common.IR.ProtoTypes;
using Unluau.Common.IR.ProtoTypes.Instructions;
using Unluau.Common.IR.Versions;
using Type = Unluau.Common.IR.ProtoTypes.Type;
using Version = Unluau.Common.IR.Versions.Version;

namespace Unluau.Common.IR
{
    /// <summary>
    /// This class builds the IR from a Luau bytecode stream.
    /// </summary>
    public class Reader : BinaryReader
    {
        /// <summary>
        /// The bit that indicates if a type is optional or not.
        /// </summary>
        private const int TypeOptionalBit = 1 << 7;

        private readonly List<string> _symbolTable = [];
        private Version _version = new();

        /// <summary>
        /// Creates a new instance of <see cref="Reader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public Reader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Reader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The encoding to use.</param>
        public Reader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Reader"/>.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen">Leaves the stream open.</param>
        public Reader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        /// <summary>
        /// Reads a <see cref="Module"/> from the stream.
        /// </summary>
        /// <returns>The module.</returns>
        public Module ReadModule()
        {
            _version = ReadVersion();

            // Now we read the symbol table. We will use this globally to reference strings.
            var stringCount = Read7BitEncodedInt();
            for (var i = 0; i < stringCount; i++)
                _symbolTable.Add(ReadString());

            // Now we read all of the function prototypes in this module.
            var protoTypeCount = Read7BitEncodedInt();
            var protoTypes = new List<ProtoType>();
            for (var i = 0; i < protoTypeCount; i++)
                protoTypes.Add(ReadProtoType());

            // The entry point is the index of the function prototype that is the entry point.
            var entryPoint = Read7BitEncodedInt();

            return new Module
            {
                Version = _version,
                SymbolTable = _symbolTable,
                ProtoTypes = protoTypes,
                EntryPoint = entryPoint
            };
        }

        /// <summary>
        /// Reads a <see cref="ProtoType"/> from the stream.
        /// </summary>
        /// <returns>The function prototype.</returns>
        public ProtoType ReadProtoType()
        {
            var protoType = new ProtoType
            {
                MaxStackSize = ReadByte(),
                ParameterCount = ReadByte(),
                Upvalues = Enumerable.Repeat(new Upvalue(), ReadByte()).ToList(),
                IsVararg = ReadByte() == 1,
            };

            // Now we read type information of the function prototype if type encoding is enabled.
            if (_version is TypedVersion)
            {
                // Now we read the flags of the function prototype. They contain information about the function.
                protoType.Flags = new(ReadByte());

                var typeSize = Read7BitEncodedInt();

                // As of right now I see no way to use type info unless compiled natively. Therefore, we just skip it.
                FillBuffer(typeSize);
            }

            // Now we read all of the instructions in the function prototype.
            var endPosition = BaseStream.Position + Read7BitEncodedInt() * 4;
            while (BaseStream.Position < endPosition)
                protoType.Instructions.Add(ReadInstruction());

            return protoType;
        }

        /// <summary>
        /// Reads an <see cref="Instruction"/> from the stream.
        /// </summary>
        public Instruction ReadInstruction()
        {
            // Now we will peek the next byte as it will be the opcode of the following instruction.
            var code = (OpCode)PeekChar();
            return code switch
            {
                OpCode.Nop => new Nop(ReadUInt32()),
                OpCode.Break => new Break(ReadUInt32()),
                OpCode.LoadNil => new LoadNil(ReadUInt32()),
                _ => throw new Exception($"invalid opcode {code}")
            };
        }

        /// <summary>
        /// Reads a <see cref="Version"/> from the stream.
        /// </summary>
        public Version ReadVersion()
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

            return Encoding.ASCII.GetString(ReadBytes(length.Value));
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
