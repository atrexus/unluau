// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;
using Unluau.Logic.Statements;

namespace Unluau
{
    class StatementExpression : Statement
    {
        public Expression Expression { get; }

        public StatementExpression(Expression expression)
            => Expression = expression;

        public override void Write(Output output)
        {
            Expression.Write(output);
        }
    }
}
