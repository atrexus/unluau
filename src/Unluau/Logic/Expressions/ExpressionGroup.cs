// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class ExpressionGroup : Expression
    {
        public Expression Expression { get; set; }

        public ExpressionGroup(Expression expression)
        {
            Expression = expression;
        }

        public override void Write(Output output)
        {
            output.Write("(");
            Expression.Write(output);
            output.Write(")");
        }

        public override string[] GetNames()
        {
            return Expression.GetNames();
        }
    }
}
