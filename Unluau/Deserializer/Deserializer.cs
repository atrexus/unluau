using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unluau
{
    public class Deserializer
    {
        private BytecodeReader reader;

        private const byte MinVesion = 3, MaxVersion = 3;

        public Deserializer(Stream stream)
            => reader = new BytecodeReader(stream);

        public Chunk Deserialize()
        {
            Chunk chunk = new Chunk();

            byte version = reader.ReadByte();

            // The rest of the bytecode is the error message
            if (version == 0)
                throw new DecompilerException(Stage.Deserializer, reader.ReadASCII((int)(reader.Stream.Length - 1)));
            
            // Make sure we have a valid bytecode version (so in range)
            if (version < MinVesion || version > MaxVersion)
                throw new DecompilerException(Stage.Deserializer, "bytecode version mismatch");

            IList<string> strings = ReadStrings();
            chunk.Functions = ReadFunctions(strings);
            chunk.MainIndex = reader.ReadInt32Compressed();

            return chunk;
        }

        private IList<string> ReadStrings()
        {
            int size = reader.ReadInt32Compressed();

            IList<string> strings = new List<string>(size);

            while (strings.Count < size)
            {
                int stringSize = reader.ReadInt32Compressed();

                // Really stupid check, but Luau seems to have an issue where '\n' is added before 'GetService'.
                if (stringSize == 13 && reader.Peek() == 10)
                    stringSize = reader.ReadInt32Compressed();

                strings.Add(reader.ReadASCII(stringSize));
            }

            return strings;
        }

        // Function used to read a string from the string table. Not used in the actual string table.
        private string ReadString(IList<string> strings)
        {
            int id = reader.ReadInt32Compressed();

            return id == 0 || id > strings.Count ? null : strings[id - 1];
        }

        private IList<Function> ReadFunctions(IList<string> strings)
        {
            int size = reader.ReadInt32Compressed();

            IList<Function> functions = new List<Function>(size);

            while (functions.Count < size)
                functions.Add(ReadFunction(functions, strings));

            return functions;
        }

        private Function ReadFunction(IList<Function> functions, IList<string> strings)
        {
            Function function = new Function();

            function.Id = functions.Count;

            function.MaxStackSize = reader.ReadByte();
            function.Parameters = reader.ReadByte();
            function.MaxUpvalues = reader.ReadByte();
            function.Upvalues = new List<LocalExpression>(function.MaxUpvalues);
            function.IsVararg = reader.ReadByte() == 1;

            function.Instructions = ReadInstructions();
            function.Constants = ReadConstants(strings);
            function.Functions = GetFunctions(functions);
            function.GlobalFunctions = functions;

            function.LineDefined = reader.ReadInt32Compressed();
            function.DebugName = ReadString(strings);

            function.LineInfo = ReadLineInfo(function.Instructions.Count);
            function.DebugInfo = ReadDebugInfo(strings);

            return function;
        }

        private IList<Instruction> ReadInstructions()
        {
            int size = reader.ReadInt32Compressed();

            IList<Instruction> instructions = new List<Instruction>(size);

            while (instructions.Count < size) 
            {
                Instruction instruction = new Instruction((int)reader.ReadUInt32());

                // Optimization: check for NOP as everything following it will be screwed up
                if (instruction.GetProperties().Code == OpCode.NOP)
                    throw new DecompilerException(Stage.Deserializer, "Deserializer encountered NOP (no operation) instruction. Unable to proceed.");

                instructions.Add(instruction);
            }
                

            return instructions;
        }

        private IList<Constant> ReadConstants(IList<string> strings)
        {
            int size = reader.ReadInt32Compressed();

            IList<Constant> constants = new List<Constant>(size);

            while (constants.Count < size)
                constants.Add(ReadConstant(strings, constants));

            return constants;
        }

        private Constant ReadConstant(IList<string> strings, IList<Constant> constants)
        {
            switch ((ConstantType)reader.ReadByte())
            {
                case ConstantType.Nil:
                    return new NilConstant();
                case ConstantType.Bool:
                    return new BoolConstant(reader.ReadBoolean());
                case ConstantType.Number:
                    return new NumberConstant(reader.ReadDouble());
                case ConstantType.String:
                    return new StringConstant(ReadString(strings));
                case ConstantType.Import:
                    uint id = reader.ReadUInt32();
                    int count = (int)(id >> 30);

                    IList<StringConstant> names = new List<StringConstant>(count);

                    if (count > 0)
                        names.Add((StringConstant)constants[(int)(id >> 20) & 1023]);

                    if (count > 1)
                        names.Add((StringConstant)constants[(int)(id >> 10) & 1023]);

                    if (count > 2)
                        names.Add((StringConstant)constants[(int)(id >> 0) & 1023]);

                    return new ImportConstant(names);
                case ConstantType.Table:
                    int size = reader.ReadInt32Compressed();

                    IList<Constant> keys = new List<Constant>(size);

                    while (keys.Count < size)
                        keys.Add(constants[reader.ReadInt32Compressed()]);

                    return new TableConstant(keys);
                case ConstantType.Closure:
                    return new ClosureConstant(reader.ReadInt32Compressed());
                default:
                    throw new DecompilerException(Stage.Deserializer, "unexpected constant kind");
            }
        }

        private IList<int> GetFunctions(IList<Function> functions)
        {
            int size = reader.ReadInt32Compressed();

            IList<int> newFunctions = new List<int>(size);

            while (newFunctions.Count < size)
                newFunctions.Add(reader.ReadInt32Compressed());
            
            return newFunctions;
        }

        private LineInfo ReadLineInfo(int instructions)
        {
            LineInfo lineInfo = null;

            // Line info needs to be enabled
            if (reader.ReadBoolean())
            {
                lineInfo = new LineInfo();

                lineInfo.LineGapLog = reader.ReadByte();

                int intervals = ((instructions - 1) >> lineInfo.LineGapLog) + 1;

                lineInfo.LineInfoList = new List<byte>(instructions);

                byte lastOffset = 1;
                while (lineInfo.LineInfoList.Count < instructions)
                {
                    lastOffset += reader.ReadByte();
                    lineInfo.LineInfoList.Add(lastOffset);
                }

                lineInfo.AbsLineInfoList = new List<int>(instructions);

                int lastLine = 0;
                while (lineInfo.AbsLineInfoList.Count < intervals)
                {
                    lastLine += reader.ReadInt32();
                    lineInfo.AbsLineInfoList.Add(lastLine);
                }
            }

            return lineInfo;
        }

        private DebugInfo ReadDebugInfo(IList<string> strings)
        {
            DebugInfo debugInfo = null;

            // Line info needs to be enabled
            if (reader.ReadBoolean())
            {
                debugInfo = new DebugInfo();

                int sizeVars = reader.ReadInt32Compressed();

                IList<LocalVariable> localVariables = new List<LocalVariable>(sizeVars);

                while (localVariables.Count < sizeVars)
                {
                    LocalVariable variable = new LocalVariable();

                    variable.Name = ReadString(strings);
                    variable.StartPc = reader.ReadInt32Compressed();
                    variable.EndPc = reader.ReadInt32Compressed();
                    variable.Slot = reader.ReadByte();

                    localVariables.Add(variable);
                }

                int sizeUpvals = reader.ReadInt32Compressed();

                IList<string> upvalues = new List<string>(sizeUpvals);

                while (upvalues.Count < sizeUpvals)
                    upvalues.Add(ReadString(strings));

                debugInfo.Locals = localVariables;
                debugInfo.Upvalues = upvalues;
            }

            return debugInfo;
        }
    }
}
