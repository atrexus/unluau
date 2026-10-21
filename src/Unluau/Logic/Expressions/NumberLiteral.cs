// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Globalization;

namespace Unluau.Logic.Expressions
{
    public class NumberLiteral : Expression, IComparable<double>
    {
        public double Value { get; protected set; }

        public NumberLiteral(double value)
            => Value = value;

        public override void Write(Output output)
        {
            if (double.IsPositiveInfinity(Value))
                output.Write("math.huge");
            else if (double.IsNegativeInfinity(Value))
                output.Write("-math.huge");
            else
                output.Write(Value.ToString(CultureInfo.InvariantCulture));
        }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }
    }
}
