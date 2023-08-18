// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau
{
    public class LocalAssignment : Statement
    {
        public Expression Expression { get; private set; }
        public Expression Value { get; private set; }

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

        public bool TryGetVariable(out LocalExpression variable)
        {
            if (Expression is LocalExpression)
            {
                variable = Expression as LocalExpression;
                return true;
            }

            variable = null;
            return false;
        }

        public bool TryGetVariables(out ExpressionList variable)
        {
            if (Expression is ExpressionList)
            {
                variable = Expression as ExpressionList;
                return true;
            }

            variable = null;
            return false;
        }

        public override void Write(Output output)
        {
            output.Write("local ");

            if (Value is Closure)
            {
                output.Write("function ");
                Expression.Write(output);
                Value.Write(output);
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
