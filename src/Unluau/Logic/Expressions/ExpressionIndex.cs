// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class ExpressionIndex : Expression
    {
        public Expression Expression { get; set; }
        public Expression Index { get; set; }

        public ExpressionIndex(Expression expression, Expression index)
        {
            Expression = expression;
            Index = index;
        }

        public override void Write(Output output)
        {
            Expression.Write(output);

            output.Write("[");
            Index.Write(output);
            output.Write("]");
        }
    }
}
