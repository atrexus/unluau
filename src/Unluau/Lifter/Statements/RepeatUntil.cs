// Copyright (c) Valence. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

namespace Unluau
{
    internal class RepeatUntil : Statement
    {
        public Expression Condition { get; }
        public Block Body { get; }

        public RepeatUntil(Expression condition, Block body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Write(Output output)
        {
            output.WriteLine("repeat");
            Body.Write(output);
           
            output.Write("until ");
            Condition.Write(output);
        }
    }
}
