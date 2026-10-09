// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Numerics;

namespace Unluau.Format.Models
{
	public enum ConstantType
	{
		Nil,
		Bool,
		Number,
		String,
		Import,
		Table,
		Closure,
        Vector
	}

	public abstract class Constant
	{
		public ConstantType Type { get; protected init; }
		public abstract override string ToString();
	}

	public abstract class Constant<T> : Constant
	{
		public T? Value { get; }

		protected Constant(ConstantType type, T? value)
		{
			Type = type;
			Value = value;
		}

		public override string ToString()
		{
			return Value?.ToString() ?? throw new InvalidOperationException();
		}
	}

	public class NilConstant : Constant<object>
	{
		public NilConstant()
			: base(ConstantType.Nil, null)
		{ }

		public override string ToString()
		{
			return "nil";
		}
	}

	public class BoolConstant : Constant<bool>
	{
		public BoolConstant(bool value)
			: base(ConstantType.Bool, value)
		{ }

		public override string ToString()
		{
			return Value ? "true" : "false";
		}
	}

	public class NumberConstant : Constant<double>
	{
		public NumberConstant(double value)
			: base(ConstantType.Number, value)
		{ }

        public override string ToString()
        {
			return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

	public class StringConstant : Constant<string>
	{
		public StringConstant(string value)
			: base(ConstantType.String, value)
		{ }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
	}

	public class ImportConstant : Constant<IList<StringConstant>>
	{
		public ImportConstant(IList<StringConstant> names)
			: base(ConstantType.Import, names)
		{ }

        public override string ToString()
        {
            return $"[{string.Join(",", Value ?? throw new InvalidOperationException())}]";
        }
	}

	public class TableConstant : Constant<IList<Constant>>
	{
		public TableConstant(IList<Constant> keys)
			: base(ConstantType.Table, keys)
		{ }

        public override string ToString()
        {
            return $"{{{string.Join(", ", Value ?? throw new InvalidOperationException())}}}";
        }
	}

	public class ClosureConstant : Constant<int>
	{
		public ClosureConstant(int index)
			: base(ConstantType.Closure, index)
		{ }
    }

    public class VectorConstant : Constant<Vector4>
    {
        public VectorConstant(float x, float y, float z, float w)
            : base(ConstantType.Vector, new Vector4(x, y, z, w))
        { }
    }
}
