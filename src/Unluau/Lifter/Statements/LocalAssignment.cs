// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System.Diagnostics.CodeAnalysis;

namespace Unluau
{
    public class LocalAssignment : Statement
    {
        public Expression Expression { get; }
        public Expression? Value { get; }

        public LocalAssignment(LocalExpression expression)
        {
            Expression = expression;
            Value = expression.Expression;
        }

        public LocalAssignment(Expression expression, Expression value)
        {
            Expression = expression;
            Value = value;
        }

        public bool TryGetVariable([NotNullWhen(true)] out LocalExpression? variable)
        {
            if (Expression is LocalExpression localExpression)
            {
                variable = localExpression;
                return true;
            }

            variable = null;
            return false;
        }

        public bool TryGetVariables([NotNullWhen(true)] out ExpressionList? variable)
        {
            if (Expression is ExpressionList expression)
            {
                variable = expression;
                return true;
            }

            variable = null;
            return false;
        }

        public override void Write(Output output)
        {
            if (Value == null)
            {
                output.Write("-- [Unluau: Failed to decompile local assignment]");
                return;
            }
    
            output.Write("local ");

            if (Value is Closure)
            {
                output.Write("function ");
                Expression.Write(output);
                Value.Write(output);
                output.WriteLine();
            }
            else
            {
                Expression.Write(output);
                output.Write(" = ");
                Value.Write(output);
            }
        }
    }
}
