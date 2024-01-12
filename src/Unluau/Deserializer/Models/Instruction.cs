// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unluau
{
    public class Instruction
    {
        private uint _value;
        private OpCodeEncoding _encoding;

        public OpProperties GetProperties()
        {
            if (OpProperties.Map.TryGetValue(Code, out var properties))
                return properties;

            throw new DecompilerException(Stage.Deserializer, $"unhandled operation code ({Code})");
        }

        public uint Value { 
            get { return _value; } 
        }

        public byte A
            => (byte)((_value >> 8) & 0xFF);

        public byte B
            => (byte)((_value >> 16) & 0xFF);

        public byte C
            => (byte)((_value >> 24) & 0xFF);

        public int D
            => (int)_value >> 16;

        public int E
            => (int)(_value >> 8);

        public OpCode Code
        {
            get
            {
                uint rawCode = Value & 0xFF;

                switch (_encoding)
                {
                    case OpCodeEncoding.Client:
                        rawCode *= 203u;
                        break;
                }

                return (OpCode)rawCode;
            }
            private set { }
        }

        public Instruction(uint value, OpCodeEncoding encoding = OpCodeEncoding.None)
        {
            _value = value;
            _encoding = encoding;
        }
    }
}
