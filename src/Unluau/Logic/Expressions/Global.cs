// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class Global : Expression, IComparable<string>
    {
        public string Value { get; protected set; }

        public Global(string value)
            => Value = value;

        public Global(IList<string> value)
            => Value = string.Join(".", value.ToArray());

        public override void Write(Output output)
        {
            output.Write(Value);
        }

        public int CompareTo(string? other) => String.Compare(Value, other, StringComparison.Ordinal);

        public override string[] GetNames() => new[] { Value };
    }
}
