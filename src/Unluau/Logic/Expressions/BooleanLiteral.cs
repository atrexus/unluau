// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class BooleanLiteral : Expression, IComparable<bool>
    {
        public bool Value { get; protected set; }

        public BooleanLiteral(bool value)
            => Value = value;

        public int CompareTo(bool other)
        {
            return Value.CompareTo(other);
        }

        public override void Write(Output output)
        {
            output.Write(Value ? "true" : "false");
        }
    }
}
