// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Numerics;
using Unluau.Logic.Expressions;
using Unluau.Utilities;

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

        public abstract Expression ToExpression();

        public virtual Expression ToExpressionGlobal()
        {
            throw new InvalidOperationException();
        }
    }

	public abstract class Constant<T> : Constant
	{
		public T Value { get; }

		protected Constant(ConstantType type, T value)
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
			: base(ConstantType.Nil, "nil")
		{ }

		public override string ToString()
		{
			return "nil";
		}

        public override Expression ToExpression() => new NilLiteral();
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
        
        public override Expression ToExpression() => new BooleanLiteral(Value);
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
        
        public override Expression ToExpression() => new NumberLiteral(Value);
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

        public override Expression ToExpression() => new StringLiteral(Value);
        public override Expression ToExpressionGlobal() => new Global(Value);
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

        public override Expression ToExpression()
        {
            return new Global(Value.Select(c => c.Value).ToList());
        }

        public override Expression ToExpressionGlobal() => ToExpression();
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
        
        public override Expression ToExpression()
        {
            throw new InvalidOperationException();
        }
	}

	public class ClosureConstant : Constant<int>
	{
		public ClosureConstant(int index)
			: base(ConstantType.Closure, index)
		{ }

        public override Expression ToExpression()
        {
            throw new InvalidOperationException();
        }
    }

    public class VectorConstant : Constant<Vector4>
    {
        public VectorConstant(float x, float y, float z, float w)
            : base(ConstantType.Vector, new Vector4(x, y, z, w))
        { }

        public override Expression ToExpression() => new VectorLiteral(Value);
    }
}
