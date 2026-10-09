// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Serilog;

namespace Unluau
{
    public class Deserializer
    {
        private readonly BytecodeReader _reader;
        private byte _version, _typesVersion;
        private readonly OpCodeEncoding _encoding;

        private const byte MinVersion = 3, MaxVersion = 6; // [3..6]
        private const byte TypeVersion = 1;

        public Deserializer(Stream stream, OpCodeEncoding encoding)
        {
            _reader = new BytecodeReader(stream);

            this._encoding = encoding;
        }

        public Chunk Deserialize()
        {
            _version = _reader.ReadByte();

            // The rest of the bytecode is the error message
            if (_version == 0)
                throw new DecompilerException(Stage.Deserializer, _reader.ReadASCII((int)(_reader.Stream.Length - 1)));
            
            // Make sure we have a valid bytecode version (so in range)
            if (_version < MinVersion || _version > MaxVersion)
                throw new DecompilerException(Stage.Deserializer, $"Bytecode version mismatch, expected version {MinVersion}...{MaxVersion}, got {_version}");

            if (_version >= 4)
                _typesVersion = _reader.ReadByte();

            var strings = ReadStrings();
            if (_typesVersion == 3) // userdatatype remapping (tv3)
            {
                byte index = _reader.ReadByte();
                while (index != 0)
                {
                    _reader.ReadInt32Compressed();

                    index = _reader.ReadByte();
                }
            }

            return new Chunk(ReadFunctions(strings), _reader.ReadInt32Compressed());
        }

        private IList<string> ReadStrings()
        {
            int size = _reader.ReadInt32Compressed();
            
            Log.Debug("Reading {Size} strings from the string table", size);

            var strings = new List<string>(size);
            for (int i = 0; i < size; i++)
            {
                strings.Add(_reader.ReadASCII(_reader.ReadInt32Compressed()));
            }

            return strings;
        }

        /// <summary>
        /// Function used to read a string from the string table. Not used in the actual string table.
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private string ReadString(IList<string> strings)
        {
            int id = _reader.ReadInt32Compressed();

            return id == 0 || id > strings.Count ? string.Empty : strings[id - 1];
        }

        private IList<Function> ReadFunctions(IList<string> strings)
        {
            int size = _reader.ReadInt32Compressed();

            Log.Debug("Reading {Size} functions from main bytecode pool", size);
            
            var functions = new List<Function>(size);
            while (functions.Count < size)
                functions.Add(ReadFunction(functions, strings));

            return functions;
        }

        private Function ReadFunction(IList<Function> functions, IList<string> strings)
        {
            Log.Debug("Reading basic function prototype information");
            
            Function function = new Function
            {
                Id = functions.Count,
                MaxStackSize = _reader.ReadByte(),
                Parameters = _reader.ReadByte(),
                MaxUpvalues = _reader.ReadByte()
            };

            function.Upvalues = new List<LocalExpression>(function.MaxUpvalues);
            function.IsVararg = _reader.ReadByte() == 1;

            if (_version >= 4)
            {
                function.Flags = _reader.ReadByte();
                int typesSize = _reader.ReadInt32Compressed();
                _reader.Position += typesSize;
            }

            function.Instructions = ReadInstructions();
            function.Constants = ReadConstants(strings);
            function.Functions = ReadFunctions();
            function.GlobalFunctions = functions;

            function.LineDefined = _reader.ReadInt32Compressed();
            function.DebugName = ReadString(strings);

            function.LineInfo = ReadLineInfo(function.Instructions.Count);
            function.DebugInfo = ReadDebugInfo(strings);

            return function;
        }

        private IList<Instruction> ReadInstructions()
        {
            int size = _reader.ReadInt32Compressed();

            Log.Debug("Reading {Size} instructions from function prototype body", size);

            var instructions = new List<Instruction>(size);
            while (instructions.Count < size) 
            {
                Instruction instruction = new Instruction(_reader.ReadUInt32(), _encoding);
                OpProperties properties = instruction.GetProperties();

                // Note: Sometimes we get NOPs...
                if (properties.Code == OpCode.NOP)
                    Log.Warning("Encountered unexpected NOP instruction.");

                instructions.Add(instruction);

                if (properties.HasAux)
                    instructions.Add(new Instruction(_reader.ReadUInt32()));
            }
                
            return instructions;
        }

        private IList<Constant> ReadConstants(IList<string> strings)
        {
            int size = _reader.ReadInt32Compressed();
            
            Log.Debug("Reading {Size} constants from function prototype body", size);

            var constants = new List<Constant>(size);
            while (constants.Count < size)
                constants.Add(ReadConstant(strings, constants));

            return constants;
        }

        private Constant ReadConstant(IList<string> strings, IList<Constant> constants)
        {
            ConstantType constantType = (ConstantType)_reader.ReadByte();

            switch (constantType)
            {
                case ConstantType.Nil:
                    return new NilConstant();
                case ConstantType.Bool:
                    return new BoolConstant(_reader.ReadBoolean());
                case ConstantType.Number:
                    return new NumberConstant(_reader.ReadDouble());
                case ConstantType.String:
                    return new StringConstant(ReadString(strings));
                case ConstantType.Import:
                    uint id = _reader.ReadUInt32();
                    int count = (int)(id >> 30);

                    var names = new List<StringConstant>(count);

                    if (count > 0)
                        names.Add((StringConstant)constants[(int)(id >> 20) & 0x3FF]);

                    if (count > 1)
                        names.Add((StringConstant)constants[(int)(id >> 10) & 0x3FF]);

                    if (count > 2)
                        names.Add((StringConstant)constants[(int)(id >> 0) & 0x3FF]);

                    return new ImportConstant(names);
                case ConstantType.Table:
                    var keys = new List<Constant>(_reader.ReadInt32Compressed());
                    for (int i = 0; i < keys.Capacity; i++)
                        keys.Add(constants[_reader.ReadInt32Compressed()]);

                    return new TableConstant(keys);
                case ConstantType.Closure:
                    return new ClosureConstant(_reader.ReadInt32Compressed());
                case ConstantType.Vector:
                    return new VectorConstant(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
                default:
                    throw new DecompilerException(Stage.Deserializer, $"Constant type is not defined ({constantType})");
            }
        }

        private IList<int> ReadFunctions()
        {
            var newFunctions = new List<int>(_reader.ReadInt32Compressed());
            for (int i = 0; i < newFunctions.Capacity; i++)
                newFunctions.Add(_reader.ReadInt32Compressed());
            
            return newFunctions;
        }

        private LineInfo? ReadLineInfo(int instructions)
        {
            if (!_reader.ReadBoolean())
            {
                Log.Debug("Line information is disabled, skipping.");
                return null;
            }

            Log.Debug("Line information is enabled, reading.");
            
            var lineGapLog = _reader.ReadByte();
            int intervals = ((instructions - 1) >> lineGapLog) + 1;

            var lineInfoList = new List<byte>(instructions);

            byte lastOffset = 0;
            while (lineInfoList.Count < instructions)
            {
                lastOffset += _reader.ReadByte();
                lineInfoList.Add(lastOffset);
            }

            var absLineInfoList = new List<int>(instructions);

            int lastLine = 0;
            while (absLineInfoList.Count < intervals)
            {
                lastLine += _reader.ReadInt32();
                absLineInfoList.Add(lastLine);
            }

            return new LineInfo(lineInfoList, absLineInfoList)
            {
                LineGapLog = lineGapLog
            };
        }

        private DebugInfo? ReadDebugInfo(IList<string> strings)
        {
            if (!_reader.ReadBoolean())
            {
                Log.Debug("Debug information is disabled, skipping.");
                return null;
            }

            Log.Debug("Debug information is enabled, reading.");
            
            var localVariables = new List<LocalVariable>(_reader.ReadInt32Compressed());
            for (int i = 0; i < localVariables.Capacity; i++)
            {
                LocalVariable variable = new LocalVariable
                {
                    Name = ReadString(strings),
                    StartPc = _reader.ReadInt32Compressed(),
                    EndPc = _reader.ReadInt32Compressed(),
                    Slot = _reader.ReadByte()
                };
                localVariables.Add(variable);
            }

            var upvalues = new List<string>(_reader.ReadInt32Compressed());
            for (int i = 0; i < upvalues.Capacity; i++)
                upvalues.Add(ReadString(strings));

            return new DebugInfo(localVariables, upvalues);
        }
    }
}
