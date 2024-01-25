using Microsoft.VisualBasic;
using Unluau.IL;
using Unluau.Utils;

namespace Unluau.Chunk.Luau
{
    /// <summary>
    /// Represents a function in a Luau bytecode chunk.
    /// </summary>
    public class Function
    {
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
        public Function(BinaryReader reader, Version version)
        {
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
    }
}
