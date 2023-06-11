// Copyright (c) societall. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;

namespace Unluau
{
    public class StringLiteral : Expression, IComparable<string>
    {
        public string Value { get; protected set; }

        public StringLiteral(string value)
            => Value = value;

        public override void Write(Output output)
        {
            output.Write($"\"{Value}\"");
        }

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }

        public override string[] GetNames()
        {
            return new[] { Value };
        }
    }
}
