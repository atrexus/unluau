// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Serilog;

namespace Unluau
{
    public class Deserializer
    {
        private BytecodeReader reader;
        private byte version, typesVersion;
        private OpCodeEncoding encoding;

        private const byte MinVesion = 3, MaxVersion = 5;
        private const byte TypeVersion = 1;

        public Deserializer(Stream stream, OpCodeEncoding encoding)
        {
            reader = new BytecodeReader(stream);

            this.encoding = encoding;
        }

        public Chunk Deserialize()
        {
            Chunk chunk = new Chunk();

            version = reader.ReadByte();

            // The rest of the bytecode is the error message
            if (version == 0)
                throw new DecompilerException(Stage.Deserializer, reader.ReadASCII((int)(reader.Stream.Length - 1)));
            
            // Make sure we have a valid bytecode version (so in range)
            if (version < MinVesion || version > MaxVersion)
                throw new DecompilerException(Stage.Deserializer, $"Bytecode version mismatch, expected version {MinVesion}...{MaxVersion}");

            if (version >= 4)
                typesVersion = reader.ReadByte();

            var strings = ReadStrings();
            chunk.Functions = ReadFunctions(strings);
            chunk.MainIndex = reader.ReadInt32Compressed();

            return chunk;
        }

        private IList<string> ReadStrings()
        {
            int size = reader.ReadInt32Compressed();

            IList<string> strings = new List<string>(size);

            Log.Debug($"Reading {size} strings from the string table");

            while (strings.Count < size)
            {
                int stringSize = reader.ReadInt32Compressed();

                // Really stupid check, but Luau seems to have an issue where '\n' is added before 'GetService'.
/*                if (stringSize == 13 && reader.Peek() == 10)
                    stringSize = reader.ReadInt32Compressed();*/
                

                strings.Add(reader.ReadASCII(stringSize));
            }

            return strings;
        }

        // Function used to read a string from the string table. Not used in the actual string table.
        private string ReadString(IList<string> strings)
        {
            int id = reader.ReadInt32Compressed();

            return id == 0 || id > strings.Count ? string.Empty : strings[id - 1];
        }

        private IList<Function> ReadFunctions(IList<string> strings)
        {
            int size = reader.ReadInt32Compressed();

            IList<Function> functions = new List<Function>(size);

            Log.Debug($"Reading {size} functions from main bytecode pool");

            while (functions.Count < size)
                functions.Add(ReadFunction(functions, strings));

            return functions;
        }

        private Function ReadFunction(IList<Function> functions, IList<string> strings)
        {
            Function function = new Function();

            function.Id = functions.Count;

            Log.Debug("Reading basic function prototype information");

            function.MaxStackSize = reader.ReadByte();
            function.Parameters = reader.ReadByte();
            function.MaxUpvalues = reader.ReadByte();
            function.Upvalues = new List<LocalExpression>(function.MaxUpvalues);
            function.IsVararg = reader.ReadByte() == 1;

            if (version >= 4)
            {
                function.Flags = reader.ReadByte();

                int typesSize = reader.ReadInt32Compressed();

                if (typesSize > 0 && typesVersion == TypeVersion)
                {
                    function.Types = reader.ReadBytes(typesSize);

                    // Todo: remove these retarded assert function defs
                    Debug.Assert(typesSize == 2 + function.Parameters);
                    Debug.Assert(function.Types[0] == (int)BytecodeTypes.Function);
                    Debug.Assert(function.Types[1] == function.Parameters);
                }
            }

            function.Instructions = ReadInstructions();
            function.Constants = ReadConstants(strings);
            function.Functions = ReadFunctions();
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

            Log.Debug($"Reading {size} instructions from function prototype body");

            while (instructions.Count < size) 
            {
                Instruction instruction = new Instruction(reader.ReadUInt32(), encoding);
                OpProperties properties = instruction.GetProperties();

                // Note: Sometimes we get NOPs...
                if (properties.Code == OpCode.NOP)
                    Log.Warning($"Encountered unexpected NOP instruction.");

                instructions.Add(instruction);

                if (properties.HasAux)
                    instructions.Add(new Instruction(reader.ReadUInt32()));
            }
                
            return instructions;
        }

        private IList<Constant> ReadConstants(IList<string> strings)
        {
            int size = reader.ReadInt32Compressed();

            IList<Constant> constants = new List<Constant>(size);

            Log.Debug($"Reading {size} constants from function prototype body");

            while (constants.Count < size)
                constants.Add(ReadConstant(strings, constants));

            return constants;
        }

        private Constant ReadConstant(IList<string> strings, IList<Constant> constants)
        {
            int c = reader.ReadByte();

            if (!Enum.IsDefined(typeof(ConstantType), c))
                throw new DecompilerException(Stage.Deserializer, $"Constant is not defined ({c})");
            
            ConstantType constantType = (ConstantType)c;

            switch (constantType)
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
                case ConstantType.Vector:
                    return new VectorConstant(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }

            // Should never happen
            throw new DecompilerException(Stage.Deserializer, $"No constant returned for type ({constantType})");
        }

        private IList<int> ReadFunctions()
        {
            int size = reader.ReadInt32Compressed();

            IList<int> newFunctions = new List<int>(size);

            while (newFunctions.Count < size)
                newFunctions.Add(reader.ReadInt32Compressed());
            
            return newFunctions;
        }

        private LineInfo? ReadLineInfo(int instructions)
        {
            LineInfo? lineInfo = null;

            // Line info needs to be enabled
            if (reader.ReadBoolean())
            {
                Log.Debug("Line information is enabled, reading.");

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
            else
                Log.Debug("Line information is disabled, skipping.");

            return lineInfo;
        }

        private DebugInfo? ReadDebugInfo(IList<string> strings)
        {
            DebugInfo? debugInfo = null;

            // Line info needs to be enabled
            if (reader.ReadBoolean())
            {
                Log.Debug("Debug information is enabled, reading.");

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
            else
                Log.Debug("Debug information is disabled, skipping.");

            return debugInfo;
        }
    }
}
