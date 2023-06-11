// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unluau
{
    public class Instruction
    {
        private int _value;

        public OpProperties GetProperties()
        {
            OpCode code = (OpCode)(Value & 0xFF);
            OpProperties properties;

            if (OpProperties.Map.TryGetValue(code, out properties))
                return properties;

            throw new DecompilerException(Stage.Deserializer, $"unhandled operation code ({code})");
        }

        public int Value { 
            get { return _value; } 
            private set { _value = value; } 
        }

        public byte A
            => (byte)((_value >> 8) & 0xFF);

        public byte B
            => (byte)((_value >> 16) & 0xFF);

        public byte C
            => (byte)((_value >> 24) & 0xFF);

        public int D
            => _value >> 16;

        public int E
            => _value >> 8;

        public Instruction(int value)
            => Value = value;
    }
}
