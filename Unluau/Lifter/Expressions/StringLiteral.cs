// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;

namespace Unluau
{
    public class StringLiteral : Expression, IComparable<string>
    {
        private string? _value;
        public string Value {
            get => _value!;
            set {
                _value = value;

                _value = _value.Replace("\n", "\\n");
                _value = _value.Replace("\a", "\\a");
                _value = _value.Replace("\b", "\\b");
                _value = _value.Replace("\f", "\\f");
                _value = _value.Replace("\r", "\\r");
                _value = _value.Replace("\t", "\\t");
                _value = _value.Replace("\v", "\\v");
                
                _value = _value.Replace("\"", "\\\"");
                _value = _value.Replace("\'", "\\'");
            } 
        }

        public StringLiteral(string value)
            => Value = value;

        public override void Write(Output output)
        {
            output.Write($"\"{Value}\"");
        }

        public int CompareTo(string? other) => Value.CompareTo(other);

        public override string[] GetNames()
        {
            return new[] { Value };
        }
    }
}
