// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Unluau.Logic.Expressions;

namespace Unluau.Logic.Statements
{
    public class WhileLoop : Statement
    {
        public Expression Condition { get; }
        public Block Body { get; }

        public WhileLoop(Expression condition, Block body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.Write("while ");
            Condition.Write(output);
            output.WriteLine(" do");

            Body.Write(output);

            output.Write("end");
        }
    }
}
