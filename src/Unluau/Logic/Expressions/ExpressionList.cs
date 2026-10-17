// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau.Logic.Expressions
{
    public class ExpressionList : Expression
    {
        public IList<Expression?> Expressions { get; set; }

        public ExpressionList(int count)
            => Expressions = new List<Expression?>(count);

        public void Append(Expression? expression)
            => Expressions.Add(expression);

        public override void Write(Output output)
        {
            output.Join(", ", Expressions);
        }
    }
}
